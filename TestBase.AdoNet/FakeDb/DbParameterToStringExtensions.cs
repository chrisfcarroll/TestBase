using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace TestBase.AdoNet
{
    public static class DbParameterToStringExtensions
    {
        public static string DbParameterFormatString = "{{{2}:@{0}='{1}'}}";

        public static string PrintInvocations(
            this IEnumerable<DbCommand> invocations,
            string                      header       = "\n\nInvocations:\n",
            int                         printMaxRows = 9)
        {
            var sb = new StringBuilder(header ?? "");
            foreach (var inv in invocations.Take(printMaxRows)) sb.AppendLine(inv.ToStringTextAndParams());
            return sb.ToString();
        }

        public static string ToString1Line(this DbParameterCollection dbParameters)
        {
            var str = string.Join(", ",
                                  dbParameters.Cast<DbParameter>()
                                              .Select(
                                                      p => string.Format(DbParameterFormatString,
                                                                         p.ParameterName,
                                                                         p.Value ?? "null",
                                                                         p.DbType)
                                                     )
                                              .ToList());
            return str;
        }

        public static string ToStringPerLine(this DbParameterCollection dbParameters)
        {
            var str = string.Join("\n",
                                  dbParameters.Cast<DbParameter>()
                                              .Select(
                                                      p => string.Format(DbParameterFormatString,
                                                                         p.ParameterName,
                                                                         p.Value ?? "null",
                                                                         p.DbType)
                                                     )
                                              .ToList());
            return str;
        }

        public static string ToStringTextAndParams(this DbCommand invocation)
        {
            return invocation.CommandText + "\n\nWith Parameter Values:\n\n" + invocation.Parameters.ToStringPerLine();
        }
    }
}
