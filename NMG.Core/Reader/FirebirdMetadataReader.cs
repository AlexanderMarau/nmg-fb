using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using NMG.Core.Domain;
using NMG.Core.Util;

namespace NMG.Core.Reader
{
    /// <summary>
    /// Reads Firebird metadata
    /// </summary>
    public class FirebirdMetadataReader : IMetadataReader
    {
        private readonly string connectionStr;

        public FirebirdMetadataReader(string connectionStr)
        {
            this.connectionStr = connectionStr;
        }

        
        private class FbTable
        {
            public String TableName { get; set; }
            public String Owner { get; set; }
        }

        private class FbPrimaryKey
        {
            public String ConstraintName { get; set; }
            public String TableName { get; set; }
            public String ColumnName { get; set; }
            public int OrdinalPosition { get; set; }
        }

        private class FbForeignKey
        {
            public string ConstraintName { get; set; }
            public string TableName { get; set; }
            public string ColumnName { get; set; }
            public int OrdinalPosition { get; set; }
            public string ReferencedTable { get; set; }
            public string ReferencedColumn { get; set; }
        }


        private List<FbTable> GetTables(FbConnection conn)
        {
            var sql = new StringBuilder(@"SELECT
    rdb$relation_name AS TABLE_NAME,					
    rdb$system_flag AS IS_SYSTEM_TABLE,					
    rdb$owner_name AS OWNER_NAME,
    rdb$description AS DESCRIPTION,
    rdb$view_source AS VIEW_SOURCE
FROM rdb$relations");

            // Only tables
            sql.Append(" WHERE rdb$view_source IS NULL and rdb$system_flag = 0");

            var tables = new List<FbTable>();
            using (var cmd = new FbCommand(sql.ToString(), conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var pk = new FbTable();
                        pk.TableName = reader.GetString(0).Trim();
                        pk.Owner = reader.GetString(2).Trim();
                        tables.Add(pk);
                    }
                }
            }
            return tables;
        }

        private List<FbPrimaryKey> GetPrimaryKeyColumns(FbConnection conn, string tableName)
        {
            var sql = new StringBuilder(@"SELECT
	rel.rdb$relation_name AS TABLE_NAME,
	seg.rdb$field_name AS COLUMN_NAME,
	seg.rdb$field_position AS ORDINAL_POSITION,
	rel.rdb$constraint_name AS PK_NAME
FROM rdb$relation_constraints rel
	LEFT JOIN rdb$indices idx ON rel.rdb$index_name = idx.rdb$index_name
	LEFT JOIN rdb$index_segments seg ON idx.rdb$index_name = seg.rdb$index_name
WHERE rel.rdb$constraint_type = 'PRIMARY KEY'");

            if (!string.IsNullOrEmpty(tableName))
                sql.Append(" AND rel.rdb$relation_name = ").Append(tableName.Quoted());

            var primaryKeyColumns = new List<FbPrimaryKey>();
            using (var cmd = new FbCommand(sql.ToString(), conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var pk = new FbPrimaryKey();
                        pk.TableName = reader.GetString(0).Trim();
                        pk.ColumnName = reader.GetString(1).Trim();
                        pk.OrdinalPosition = reader.GetInt32(2);
                        pk.ConstraintName = reader.GetString(3).Trim();
                        primaryKeyColumns.Add(pk);
                    }
                }
            }
            return primaryKeyColumns;
        }

        private List<FbForeignKey> GetForeignKeyColumns(FbConnection conn, string tableName = "", string referencedTable = "")
        {
            var sql = new StringBuilder(@"SELECT
	co.rdb$constraint_name AS CONSTRAINT_NAME,
	co.rdb$relation_name AS TABLE_NAME,
	coidxseg.rdb$field_name AS COLUMN_NAME,
	refidx.rdb$relation_name as REFERENCED_TABLE_NAME,
	refidxseg.rdb$field_name AS REFERENCED_COLUMN_NAME,
	coidxseg.rdb$field_position as ORDINAL_POSITION
FROM rdb$relation_constraints co
	INNER JOIN rdb$ref_constraints ref ON co.rdb$constraint_name = ref.rdb$constraint_name
	INNER JOIN rdb$indices tempidx ON co.rdb$index_name = tempidx.rdb$index_name
	INNER JOIN rdb$index_segments coidxseg ON co.rdb$index_name = coidxseg.rdb$index_name
	INNER JOIN rdb$indices refidx ON refidx.rdb$index_name = tempidx.rdb$foreign_key
	INNER JOIN rdb$index_segments refidxseg ON refidxseg.rdb$index_name = refidx.rdb$index_name
    AND refidxseg.rdb$field_position = coidxseg.rdb$field_position
WHERE co.rdb$constraint_type = 'FOREIGN KEY'");

            if (!string.IsNullOrEmpty(tableName))
                sql.Append(" AND co.rdb$relation_name = ").Append(tableName.Quoted());

            if (!string.IsNullOrEmpty(referencedTable))
                sql.Append(" AND refidx.rdb$relation_name = ").Append(referencedTable.Quoted());

            sql.Append("ORDER BY co.rdb$constraint_name, coidxseg.rdb$field_position");

            var foreignKeys = new List<FbForeignKey>();
            using (var cmd = new FbCommand(sql.ToString(), conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var fk = new FbForeignKey();
                        fk.ConstraintName = reader.GetString(0).Trim();
                        fk.TableName = reader.GetString(1).Trim();
                        fk.ColumnName = reader.GetString(2).Trim();
                        fk.ReferencedTable = reader.GetString(3).Trim(); 
                        fk.ReferencedColumn = reader.GetString(4).Trim(); 
                        fk.OrdinalPosition = reader.GetInt32(5);

                        foreignKeys.Add(fk);
                    }
                }
            }
            return foreignKeys;
        }

        private List<Column> GetTableColumns(FbConnection conn, string tableName)
        {
            var m = new DataTypeMapper();

            var sql = new StringBuilder();
                sql.Append(@"SELECT
					rfr.rdb$relation_name AS TABLE_NAME,
					rfr.rdb$field_name AS COLUMN_NAME,
				    fld.rdb$field_sub_type AS COLUMN_SUB_TYPE,
					CAST(fld.rdb$field_length AS integer) AS COLUMN_SIZE,
					CAST(fld.rdb$field_precision AS integer) AS NUMERIC_PRECISION,
					CAST(fld.rdb$field_scale AS integer) AS NUMERIC_SCALE,
					CAST(fld.rdb$character_length AS integer) AS CHARACTER_MAX_LENGTH,
					CAST(fld.rdb$field_length AS integer) AS CHARACTER_OCTET_LENGTH,
					rfr.rdb$field_position AS ORDINAL_POSITION,
					rfr.rdb$field_source AS DOMAIN_NAME,
					rfr.rdb$default_source AS COLUMN_DEFAULT,
				    fld.rdb$computed_source AS COMPUTED_SOURCE,
					fld.rdb$dimensions AS COLUMN_ARRAY,
					coalesce(fld.rdb$null_flag, rfr.rdb$null_flag) AS COLUMN_NULLABLE,
				    0 AS IS_READONLY,
					fld.rdb$field_type AS FIELD_TYPE,
					cs.rdb$character_set_name AS CHARACTER_SET_NAME,
					coll.rdb$collation_name AS COLLATION_NAME,
					rfr.rdb$description AS DESCRIPTION
				FROM rdb$relation_fields rfr
				    LEFT JOIN rdb$fields fld ON rfr.rdb$field_source = fld.rdb$field_name
				    LEFT JOIN rdb$character_sets cs ON cs.rdb$character_set_id = fld.rdb$character_set_id
				    LEFT JOIN rdb$collations coll ON (coll.rdb$collation_id = fld.rdb$collation_id AND coll.rdb$character_set_id = fld.rdb$character_set_id)");
            
            if (!string.IsNullOrEmpty(tableName))
                sql.Append(" WHERE rfr.rdb$relation_name = ").Append(tableName.Quoted());

            sql.Append(" ORDER BY rfr.rdb$relation_name, rfr.rdb$field_position");

            var columns = new List<Column>();
            using (var cmd = new FbCommand(sql.ToString(), conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString("COLUMN_NAME").Trim();
                        int fieldType = Convert.ToInt32(reader.GetString("FIELD_TYPE").Trim(), CultureInfo.InvariantCulture);
                        int subType = reader.GetInt32("COLUMN_SUB_TYPE", 0);
                        int scale = reader.GetInt32("NUMERIC_SCALE", 0);

                        int textLength = reader.GetInt32("CHARACTER_MAX_LENGTH", 0);
                        int columnSize = reader.GetInt32("COLUMN_SIZE", 0);
                        int numericPrecision = reader.GetInt32("NUMERIC_PRECISION", 0);
                        bool isNullable = reader.IsDBNull("COLUMN_NULLABLE");
                        bool isArray =  !reader.IsDBNull("COLUMN_ARRAY");

                        FbDbType dbType = (FbDbType) FirebirdTypeResolver.GetDbDataType(fieldType, subType, scale);
                        string dataType =
                            FirebirdTypeResolver.GetDataTypeName((FirebirdTypeResolver.DbDataType) dbType)
                                                .ToUpper(CultureInfo.InvariantCulture);

                        if (dbType == FbDbType.Binary || dbType == FbDbType.Text)
                        {
                            columnSize = Int32.MaxValue;
                        }

                        if (dbType == FbDbType.Char || dbType == FbDbType.VarChar)
                        {
                            columnSize = textLength;
                        }

                        if ((dbType == FbDbType.Decimal || dbType == FbDbType.Numeric) && (numericPrecision == 0))
                        {
                            numericPrecision = columnSize;
                        }

                        scale = (-1)*scale;

                        string domainName = reader.GetString("DOMAIN_NAME", null);
                        if (domainName != null && domainName.StartsWith("RDB$"))
                        {
                            domainName = null;
                        }


                        var column = new Column
                        {
                            Name = name,
                            DataType = dataType,
                            IsNullable = isNullable,
                            IsPrimaryKey = false,
                            IsForeignKey = false,
                            IsUnique = false,
                            MappedDataType = m.MapFromDBType(ServerType.Firebird, dataType, columnSize, numericPrecision, scale)
                                 .ToString(),
                            DataLength = columnSize,
                            DataPrecision = numericPrecision,
                            DataScale = scale
                        };
                        columns.Add(column);
                    }
                }
            }
            return columns;
        }

        #region IMetadataReader

        public List<Table> GetTables(string owner)
        {
            var tables = new List<Table>();
            var conn = new FbConnection(connectionStr);
            conn.Open();
            using (conn)
            {
                // Get the list of User Tables
                var fbTables = GetTables(conn).Where(t => t.Owner.Equals(owner, StringComparison.InvariantCultureIgnoreCase));
                foreach (var fbTable in fbTables)
                {
                    tables.Add(new Table
                        {
                            Name = fbTable.TableName,
                            Owner = fbTable.Owner
                        });
                }
            }
            return tables;
        }

        public IList<Column> GetTableDetails(Table table, string owner)
        {
            var conn = new FbConnection(connectionStr);
            conn.Open();
            using (conn)
            {
                // Get constraints
                var primaryKeyColumns = GetPrimaryKeyColumns(conn, table.Name);
                var foreignKeyColumns = GetForeignKeyColumns(conn, table.Name);
                // Get Table Columns
                var columns = GetTableColumns(conn, table.Name);

                foreach (Column column in columns)
                {
                    var fk = foreignKeyColumns.FirstOrDefault(c => c.ColumnName.Equals(column.Name));
                    
                    column.IsPrimaryKey = primaryKeyColumns.Count(c => c.ColumnName.Equals(column.Name)) > 0;
                    column.IsForeignKey = fk != null;
                    column.IsUnique = false; //TODO Implement IsUnique
                            
                    if (fk != null)
                    {
                        column.ConstraintName = fk.ConstraintName;
                        column.ForeignKeyTableName = fk.ReferencedTable;
                        column.ForeignKeyColumnName = fk.ReferencedColumn;
                    }
                }

                table.Owner = owner;
                table.Columns = columns;
                table.PrimaryKey = DeterminePrimaryKeys(table);
                table.ForeignKeys = DetermineForeignKeyReferences(table);
                table.HasManyRelationships = DetermineHasManyRelationships(table);

                return columns;
            }
        }

        public IList<string> GetOwners()
        {
            string sql =  @"select  rdb$user
from rdb$user_privileges
where rdb$user_type = 8
group by 1";

            var owners = new List<string>();
            using (var conn = new FbConnection(connectionStr))
            {
                conn.Open();
                using (var cmd = new FbCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var ownerName = reader.GetString(0).Trim();
                            owners.Add(ownerName);
                        }
                    }
                }
            }
            return owners;
        }


        public List<string> GetSequences(string owner)
        {
            string sql = @"SELECT
    rdb$generator_name AS GENERATOR_NAME,
    rdb$system_flag AS IS_SYSTEM_GENERATOR,
    rdb$generator_id AS GENERATOR_ID
FROM rdb$generators
WHERE rdb$system_flag = 0";

            var generators = new List<string>();
            using (var conn = new FbConnection(connectionStr))
            {
                conn.Open();
                using (var cmd = new FbCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            generators.Add(reader.GetString(0).Trim());
                        }
                    }
                }
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
                        Columns = {c}
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
                               group c by new {c.ConstraintName, c.ForeignKeyTableName}
                               into g
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
            List<FbForeignKey> foreignKeys = null;
            var hasManyRelationships = new List<HasMany>();
            using (var conn = new FbConnection(connectionStr))
            {
                conn.Open();
                foreignKeys = GetForeignKeyColumns(conn, "", table.Name);
            }

            if (foreignKeys == null)
                return hasManyRelationships;

            foreach (var fk in foreignKeys)
            {
                var existing = hasManyRelationships.FirstOrDefault(hm => hm.ConstraintName == fk.ConstraintName);
                if (existing == null)
                {
                    var newHasManyItem = new HasMany
                        {
                            ConstraintName = fk.ConstraintName,
                            Reference = fk.TableName
                        };
                    newHasManyItem.AllReferenceColumns.Add(fk.ColumnName);
                    hasManyRelationships.Add(newHasManyItem);
                }
                else
                {
                    existing.AllReferenceColumns.Add(fk.ColumnName);
                }
            }

            return hasManyRelationships;
        }

        #endregion
    }
}