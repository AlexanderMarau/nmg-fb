using FirebirdSql.Data.FirebirdClient;

namespace NMG.Core.Util
{
    internal static class FirebirdExtensions
    {
        public static string GetString(this FbDataReader reader, string columnName)
        {
            return reader.GetString(reader.GetOrdinal(columnName));
        }

        public static int GetInt32(this FbDataReader reader, string columnName)
        {
            return reader.GetInt32(reader.GetOrdinal(columnName));
        }

        public static int GetInt32(this FbDataReader reader, string columnName, int nullValue)
        {
            int index = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(index))
                return nullValue;

            return reader.GetInt32(columnName);
        }

        public static bool IsDBNull(this FbDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName));
        }
    }
}
