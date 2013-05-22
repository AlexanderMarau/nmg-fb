using FirebirdSql.Data.FirebirdClient;

namespace NMG.Core.Util
{
    internal static class FirebirdExtensions
    {
        public static string GetString(this FbDataReader reader, string columnName)
        {
            return reader.GetString(reader.GetOrdinal(columnName)).Trim();
        }

        public static string GetString(this FbDataReader reader, string columnName, string nullValue)
        {
            int index = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(index))
                return nullValue;

            return reader.GetString(index).Trim();
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

            return reader.GetInt32(index);
        }

        public static int? GetNullableInt32(this FbDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(index))
                return null;

            return reader.GetInt32(index);
        }

        public static bool IsDBNull(this FbDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName));
        }
    }
}
