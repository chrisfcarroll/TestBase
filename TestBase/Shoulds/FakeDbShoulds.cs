using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TestBase.FakeDb;

namespace TestBase.Shoulds
{
    public static class FakeDbShoulds
    {
        /// <summary>
        /// Verifies that <paramref name="fakeDbConnection"/> has run a DbCommand with command text of the form
        /// "Update <paramref name="tableName"/> 
        ///     Set {fieldname}=@{fieldname} 
        ///         [, {fieldname}=@{fieldname}]...n 
        ///     Where <paramref name="whereClauseIdColumnName"/> = <paramref name="updateSource"/>.<paramref name="whereClauseIdColumnName"/> "
        /// 
        /// where the {fieldname}s are obtained by reflection on public properties of <paramref name="updateSource"/>.
        /// 
        /// Property names ending with "id" are EXCLUDED from the verification since they are often not updated. Verify 
        /// those separately using e.g. <see cref="ShouldHaveUpdated{T}(TestBase.FakeDb.FakeDbConnection,string,IEnumerable&lt;string&gt;,string,T)"/>
        /// 
        /// </summary>
        /// <param name="updateSource">
        /// An object having <see cref="FakeDbRehydrationExtensions.GetDbRehydratableProperties"/> Properties matching the column names of <paramref name="tableName"/>
        /// 
        /// 
        /// </param>
        /// <param name="whereClauseIdColumnName"></param>
        public static void ShouldHaveUpdated<T>(this FakeDbConnection fakeDbConnection, string tableName, T updateSource, string whereClauseIdColumnName)
        {
            var dbRehydratablePropertyNamesExIdFields = updateSource.GetType().GetDbRehydratablePropertyNames().Where(s => !s.EndsWith("id", true, null));
            var expectedId = updateSource.GetType().GetProperty(whereClauseIdColumnName).GetPropertyValue(updateSource, whereClauseIdColumnName);

            ShouldHaveUpdated(fakeDbConnection, tableName, dbRehydratablePropertyNamesExIdFields, whereClauseIdColumnName, expectedId);
        }

        /// <summary>
        /// Verifies that <paramref name="fakeDbConnection"/> has run a DbCommand with command text of the form
        /// "Update <paramref name="tableName"/> 
        ///     Set <paramref name="fieldList"/>[i]=@<paramref name="fieldList"/>[i] 
        ///         [, <paramref name="fieldList"/>[i]=@<paramref name="fieldList"/>[i]]...n 
        ///     Where <paramref name="whereClauseIdColumnName"/> = <paramref name="expectedWhereClauseId"/> "
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fakeDbConnection"></param>
        /// <param name="tableName"></param>
        /// <param name="fieldList"></param>
        /// <param name="whereClauseIdColumnName"></param>
        /// <param name="expectedWhereClauseId"></param>
        public static void ShouldHaveUpdated<T>(this FakeDbConnection fakeDbConnection, string tableName, IEnumerable<string> fieldList, string whereClauseIdColumnName, T expectedWhereClauseId)
        {
            var sqlRegexOpts = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            var optDelim = @"(\[|\]|"")?";
            var optPrefix = @"((\w|[\[\]\""])+\.)*";
            var updatetablepattern = @"Update\s+" + optPrefix + optDelim + tableName;
            var cmd = fakeDbConnection.Invocations.First(c => c.CommandText.Matches(updatetablepattern, sqlRegexOpts));
            cmd.CommandText.ShouldMatch(updatetablepattern + optDelim + @"\s+Set\s+", sqlRegexOpts);
            cmd.CommandText.ShouldMatch(@"Where " + optDelim + whereClauseIdColumnName + optDelim + @"\s*\=\s*@" + whereClauseIdColumnName, sqlRegexOpts);
            var afterSet = new Regex(@"Set\s+(.*)", sqlRegexOpts).Matches(cmd.CommandText)[0].Value;
            foreach (var field in fieldList)
            {
                var field_ = field;
                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", field_);
                afterSet.ShouldMatch(
                    string.Format(@"(Set\s+|,\s*){0}\s*=\s*\@{1}", fieldOrQuotedField, field),
                    sqlRegexOpts,
                    "Expected to update field {0} but didn't see it", field);
                cmd.Parameters.Cast<FakeDbParameter>().SingleOrAssertFail(p => p.ParameterName == field_);
            }
            cmd.Parameters.Cast<FakeDbParameter>().SingleOrAssertFail(p => p.ParameterName == whereClauseIdColumnName && p.Value.Equals(expectedWhereClauseId));
        }
    }
}
