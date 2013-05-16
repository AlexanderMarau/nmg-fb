using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using FirebirdSql.Data.FirebirdClient;
using NMG.Core.Domain;

namespace NMG.Core.Reader
{
    /// <summary>
    /// Read Firebird metadata using <see cref="DbConnection.GetSchema()"/>
    /// Get more information about Firebird schema in Firebird .NET Provider source code
    /// <see cref="http://sourceforge.net/p/firebird/code/HEAD/tree/NETProvider/trunk/NETProvider/source/FirebirdSql/Data/Schema/"/>
    /// </summary>
    public class FirebirdMetadataReader : IMetadataReader
    {
        private readonly string connectionStr;

        public FirebirdMetadataReader(string connectionStr)
        {
            this.connectionStr = connectionStr;
        }

        public List<Table> GetTables(string owner)
        {
            var tables = new List<Table>();
            var conn = new FbConnection(connectionStr);
            conn.Open();
            using (conn)
            {
                // Get the list of User Tables
                // Restrictions:
                // TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE
                DataTable userTables = conn.GetSchema("Tables", new string[] {null, null, null, "TABLE"});
                foreach (DataRow row in userTables.Rows)
                {
                    tables.Add(new Table
                        {
                            Name = row.Field<string>("TABLE_NAME"),
                            Owner = row.Field<string>("OWNER_NAME")
                        });
                }
            }
            return tables;
        }

        internal class ForeignRelation
        {
            public string Name ;
            public string Column;
            public string ReferencedTable;
            public string ReferencedColumn;
        }

        public IList<Column> GetTableDetails(Table table, string owner)
        {
            var columns = new List<Column>();
            var m = new DataTypeMapper();

            var conn = new FbConnection(connectionStr);
            conn.Open();
            using (conn)
            {
                // Get primary keys 
                // TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME
                DataTable tablePK = conn.GetSchema("primarykeys", new string[] { null, null, table.Name });
                List<string> primaryKeyColumns = (from DataRow row in tablePK.Rows select row.Field<string>("COLUMN_NAME")).ToList();

                // Get foreign keys
                // TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, CONSTRAINT_NAME 
                DataTable tableFK = conn.GetSchema("foreignkeys", new string[] { null, null, table.Name });
                var foreignKeyNames = from DataRow row in tableFK.Rows select row.Field<string>("CONSTRAINT_NAME");
                
                // Get foregign columns and reference tables
                var foreignKeyColumns = new List<ForeignRelation>();
                foreach (var fk in foreignKeyNames)
                {
                    // TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, CONSTRAINT_NAME, COLUMN_NAME
                    DataTable tableFKColumns = conn.GetSchema("foreignkeycolumns",
                                                              new string[] {null, null, table.Name, fk});
                    foreach (DataRow row in tableFKColumns.Rows)
                    {
                        foreignKeyColumns.Add(new ForeignRelation()
                            {
                                Name = fk,
                                Column = row.Field<string>("COLUMN_NAME"),
                                ReferencedTable = row.Field<string>("REFERENCED_TABLE_NAME"),
                                ReferencedColumn = row.Field<string>("REFERENCED_COLUMN_NAME"),
                            });

                    }
                }
                //List<string> foreignKeyNamens = (from DataRow row in tableFK.Rows select row.Field<string>("COLUMN_NAME")).ToList();
                
                // Get Table Columns
                // Restrictions:
                // TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME
                DataTable tableColumns = conn.GetSchema("columns", new string[] {null, null, table.Name});


                foreach (DataRow row in tableColumns.Rows)
                {
                    string name = row.Field<string>("COLUMN_NAME");
                    string dataType = row.Field<string>("COLUMN_DATA_TYPE").ToUpperInvariant();

                    int? dataLength = row.IsNull("COLUMN_SIZE") ? (int?) null : row.Field<int>("COLUMN_SIZE");
                    int? dataPrecision = row.IsNull("NUMERIC_PRECISION")
                                             ? (int?) null
                                             : row.Field<int>("NUMERIC_PRECISION");
                    int? dataScale = row.IsNull("NUMERIC_SCALE") ? (int?) null : row.Field<int>("NUMERIC_SCALE");
                    
                    var fk = foreignKeyColumns.FirstOrDefault(c => c.Column.Equals(name));

                    var column = new Column
                        {
                            Name = name,
                            DataType = dataType,
                            IsNullable = row.Field<bool>("IS_NULLABLE"),
                            IsPrimaryKey = primaryKeyColumns.Contains(name),
                            IsForeignKey = fk != null,
                            IsUnique = false, //TODO Implement IsUnique
                            MappedDataType =
                                m.MapFromDBType(ServerType.Firebird, dataType, dataLength, dataPrecision, dataScale)
                                 .ToString(),
                            DataLength = dataLength,
                            DataPrecision = dataPrecision,
                            DataScale = dataScale
                        };

                    if ((column.IsForeignKey) && (fk != null))
                    {
                        column.ConstraintName = fk.Name;
                        column.ForeignKeyTableName = fk.ReferencedTable;
                        column.ForeignKeyColumnName = fk.ReferencedColumn;
                    }
                    columns.Add(column);
                }

                table.Owner = owner;
                table.Columns = columns;
                table.PrimaryKey = DeterminePrimaryKeys(table);

                table.ForeignKeys = DetermineForeignKeyReferences(table);
                table.HasManyRelationships = DetermineHasManyRelationships(table); 
                
            }
            return columns;
        }

	    public IList<string> GetOwners()
	    {
            var owners = new List<string>();
            var conn = new FbConnection(connectionStr);

            conn.Open();
            try
            {
                using (conn)
                {
                    var tableCommand = conn.CreateCommand();
                    tableCommand.CommandText = @"select  rdb$user
from rdb$user_privileges
where rdb$user_type = 8
group by 1";
                    var reader = tableCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read())
                    {
                        var ownerName = reader.GetString(0).Trim();
                        owners.Add(ownerName);
                    }
                }
            }
            finally
            {
                conn.Close();
            }

            return owners;
	    }

	    public List<string> GetSequences(string owner)
	    {
            var generators = new List<string>();
            var conn = new FbConnection(connectionStr);
            conn.Open();
            using (conn)
            {
                // Generators schema
                // Restrictions:
                // GENERATOR_CATALOG, GENERATOR_SCHEMA, GENERATOR_NAME, IS_SYSTEM_GENERATOR
                DataTable tableGenerators = conn.GetSchema("generators", new string[] { null, null, null, "0" });
                generators.AddRange(from DataRow row in tableGenerators.Rows select row.Field<string>("GENERATOR_NAME"));
            }
	        return generators;
	    }

	    public PrimaryKey DeterminePrimaryKeys(Table table)
	    {
            var primaryKeys = table.Columns.Where(x => x.IsPrimaryKey.Equals(true)).ToList();

            if (primaryKeys.Count() == 1)
            {
                var c = primaryKeys.First();
                var key = new PrimaryKey
                {
                    Type = PrimaryKeyType.PrimaryKey,
                    Columns = { c }
                };
                return key;
            }

            if (primaryKeys.Count() > 1)
            {
                // Composite key
                var key = new PrimaryKey
                {
                    Type = PrimaryKeyType.CompositeKey,
                    Columns = primaryKeys
                };

                return key;
            }

            return null;
	    }

	    public IList<ForeignKey> DetermineForeignKeyReferences(Table table)
	    {
            var foreignKeys = (from c in table.Columns
                               where c.IsForeignKey
                               group c by new { c.ConstraintName, c.ForeignKeyTableName } into g
                               select new ForeignKey
                               {
                                   Name = g.Key.ConstraintName,
                                   References = g.Key.ForeignKeyTableName,
                                   Columns = g.ToList(),
                                   UniquePropertyName = g.Key.ForeignKeyTableName
                               }).ToList();

            Table.SetUniqueNamesForForeignKeyProperties(foreignKeys);

            return foreignKeys;
	    }

        public IList<HasMany> DetermineHasManyRelationships(Table table)
        {
            // TODO Implement DetermineHasManyRelationships
            return null;
        }
    }
}