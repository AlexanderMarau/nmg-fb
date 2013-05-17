using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NMG.Core.Reader;
using NUnit.Framework;

namespace NMG.Tests.Reader
{
    [TestFixture]
    class SqlServerMetadataReaderTest
    {
        const string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=NMG_TEST;Integrated Security=True";
        const string dbowner = "dbo";

        private SqlServerMetadataReader metadataReader;

        [SetUp]
        public void SetUp()
        {
            metadataReader = new SqlServerMetadataReader(connectionString);
        }

        [Test()]
        public void GetOwnersTest()
        {
            var owners = metadataReader.GetOwners();
            Assert.IsNotNull(owners);
            Assert.IsTrue(owners.Any());
            Assert.IsTrue(owners.Contains(dbowner));
        }

        [Test]
        public void GetTableTest()
        {
            var tables = metadataReader.GetTables(dbowner);
            Assert.IsNotNull(tables);
            Assert.IsNotEmpty(tables);
            Assert.IsTrue(tables.Any(t => string.Equals(t.Name, "PRODUCTS", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(tables.Any(t => string.Equals(t.Name, "STORES", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(tables.Any(t => string.Equals(t.Name, "CATEGORIES", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(tables.Any(t => string.Equals(t.Name, "INVENTORIES", StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public void GetTableDetailsTest()
        {
            var tables = metadataReader.GetTables(dbowner);
            var tableInv = tables.Single(t => string.Equals(t.Name, "INVENTORIES", StringComparison.OrdinalIgnoreCase));
            var invColumns = metadataReader.GetTableDetails(tableInv, dbowner);
            Assert.IsNotNull(invColumns);
            Assert.IsTrue(invColumns.Any());
            Assert.AreEqual(invColumns.Count, 7);
            Assert.AreEqual(tableInv.PrimaryKey.Type, NMG.Core.Domain.PrimaryKeyType.PrimaryKey);
            Assert.AreEqual(tableInv.ForeignKeys.Count, 2);
            var columnId = invColumns.Single(s => string.Equals(s.Name, "ID"));
            var columnStoreId = invColumns.Single(s => string.Equals(s.Name, "STORE_ID"));

            Assert.IsTrue(string.Equals(columnId.DataType, "BIGINT", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(string.Equals(columnId.MappedDataType, typeof(Int64).ToString()), "Invalid id mapped data type");

            Assert.IsTrue(string.Equals(columnStoreId.DataType, "INT", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(string.Equals(columnStoreId.MappedDataType, typeof(Int32).ToString()), "Invalid store id mapped data type");
        }

        [Test]
        public void GetSequencesTest()
        {
            var sequences = metadataReader.GetSequences(dbowner);
            Assert.IsNotNull(sequences);
            Assert.IsEmpty(sequences);
        }
    }
}
