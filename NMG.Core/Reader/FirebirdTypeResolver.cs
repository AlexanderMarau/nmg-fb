using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NMG.Core.Reader
{
    internal static class FirebirdTypeResolver
    {
        internal enum DbDataType
        {
            Array,
            BigInt,
            Binary,
            Boolean,
            Char,
            Date,
            Decimal,
            Double,
            Float,
            Guid,
            Integer,
            Numeric,
            SmallInt,
            Text,
            Time,
            TimeStamp,
            VarChar,
            Null
        }

        internal sealed class IscCodes
        {
            public const int blr_text = 14;
            public const int blr_text2 = 15;
            public const int blr_short = 7;
            public const int blr_long = 8;
            public const int blr_quad = 9;
            public const int blr_int64 = 16;
            public const int blr_float = 10;
            public const int blr_double = 27;
            public const int blr_d_float = 11;
            public const int blr_timestamp = 35;
            public const int blr_varying = 37;
            public const int blr_varying2 = 38;
            public const int blr_blob = 261;
            public const int blr_cstring = 40;
            public const int blr_cstring2 = 41;
            public const int blr_blob_id = 45;
            public const int blr_sql_date = 12;
            public const int blr_sql_time = 13;

            public const int blr_null = 45;
        }

        public static DbDataType GetDbDataType(int blrType, int subType, int scale)
        {
            switch (blrType)
            {
                case IscCodes.blr_varying:
                case IscCodes.blr_varying2:
                    return DbDataType.VarChar;

                case IscCodes.blr_text:
                case IscCodes.blr_text2:
                    return DbDataType.Char;

                case IscCodes.blr_cstring:
                case IscCodes.blr_cstring2:
                    return DbDataType.Text;

                case IscCodes.blr_short:
                    if (subType == 2)
                    {
                        return DbDataType.Decimal;
                    }
                    else if (subType == 1)
                    {
                        return DbDataType.Numeric;
                    }
                    else if (scale < 0)
                    {
                        return DbDataType.Decimal;
                    }
                    else
                    {
                        return DbDataType.SmallInt;
                    }

                case IscCodes.blr_long:
                    if (subType == 2)
                    {
                        return DbDataType.Decimal;
                    }
                    else if (subType == 1)
                    {
                        return DbDataType.Numeric;
                    }
                    else if (scale < 0)
                    {
                        return DbDataType.Decimal;
                    }
                    else
                    {
                        return DbDataType.Integer;
                    }

                case IscCodes.blr_quad:
                case IscCodes.blr_int64:
                case IscCodes.blr_blob_id:
                    if (subType == 2)
                    {
                        return DbDataType.Decimal;
                    }
                    else if (subType == 1)
                    {
                        return DbDataType.Numeric;
                    }
                    else if (scale < 0)
                    {
                        return DbDataType.Decimal;
                    }
                    else
                    {
                        return DbDataType.BigInt;
                    }

                case IscCodes.blr_double:
                case IscCodes.blr_d_float:
                    return DbDataType.Double;

                case IscCodes.blr_float:
                    return DbDataType.Float;

                case IscCodes.blr_sql_date:
                    return DbDataType.Date;

                case IscCodes.blr_sql_time:
                    return DbDataType.Time;

                case IscCodes.blr_timestamp:
                    return DbDataType.TimeStamp;

                case IscCodes.blr_blob:
                    if (subType == 1)
                    {
                        return DbDataType.Text;
                    }
                    else
                    {
                        return DbDataType.Binary;
                    }

                default:
                    throw new ArgumentException("Invalid data type");
            }
        }

        public static string GetDataTypeName(DbDataType dataType)
        {
            switch (dataType)
            {
                case DbDataType.Array:
                    return "ARRAY";

                case DbDataType.Binary:
                    return "BLOB";

                case DbDataType.Text:
                    return "BLOB SUB_TYPE 1";

                case DbDataType.Char:
                case DbDataType.Guid:
                    return "CHAR";

                case DbDataType.VarChar:
                    return "VARCHAR";

                case DbDataType.SmallInt:
                    return "SMALLINT";

                case DbDataType.Integer:
                    return "INTEGER";

                case DbDataType.Float:
                    return "FLOAT";

                case DbDataType.Double:
                    return "DOUBLE PRECISION";

                case DbDataType.BigInt:
                    return "BIGINT";

                case DbDataType.Numeric:
                    return "NUMERIC";

                case DbDataType.Decimal:
                    return "DECIMAL";

                case DbDataType.Date:
                    return "DATE";

                case DbDataType.Time:
                    return "TIME";

                case DbDataType.TimeStamp:
                    return "TIMESTAMP";

                default:
                    return null;
            }
        }
    }
}
