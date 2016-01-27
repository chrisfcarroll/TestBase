using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace TestBase.FakeDb
{
    public static class FakeDbToStrings
    {
        public static StringBuilder PrintInvocations(this IEnumerable<DbCommand> invocations, int printMaxRows = 9)
        {
            var sb = new StringBuilder("Invocations:\n");
            foreach (var inv in invocations.Take(printMaxRows))
            {
                sb.AppendLine(inv.ToStringTextAndParams());
            }
            return sb;
        }

        public static string ToString1Line(this DbParameterCollection dbParameters)
        {
            var str = String.Join(", ",
                        dbParameters.Cast<DbParameter>().Select(
                                    p => String.Format("{{{2}:@{0}='{1}'}}", p.ParameterName, p.Value ?? "null", p.DbType)
                                    ).ToList());
            return str;
        }

        public static string ToStringPerLine(this DbParameterCollection dbParameters)
        {
            var str = String.Join("\n",
                        dbParameters.Cast<DbParameter>().Select(
                                    p => String.Format("{{{2}:@{0}='{1}'}}", p.ParameterName, p.Value ?? "null", p.DbType)
                                    ).ToList());
            return str;
        }

        public static string ToStringTextAndParams(this DbCommand invocation)
        {
            return invocation.CommandText + "\n\nWith Parameter Values:\n\n" + invocation.Parameters.ToStringPerLine();
        }
    }
}