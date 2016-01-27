using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TestBase.Shoulds;

namespace TestBase.FakeDb
{
    public static class FakeDbSqlShouldsByRegexMatching
    {
        static readonly RegexOptions sqlRegexOpts = RegexOptions.IgnoreCase | RegexOptions.Singleline;
        public const string optDelim = @"(\[|\]|"")?";
        public const string optPrefix = @"((\w|[\[\]\""])+\.)*";
        public const string openBracket = @"\s*\(\s*";
        public const string closeBracket = @"\s*\)\s*";
        public const string comma = @"\s*,\s*";
        public const string set = @"\s+Set\s+";
        public const string restofline = ".*";
        public const string @select = @"Select\s+";

        public static DbCommand ShouldHaveInvoked(
            this FakeDbConnection fakeDbConnection, Expression<Func<DbCommand, bool>> predicate, string message, params object[] args)
        {
            var invocations = fakeDbConnection.Invocations;
            var invocation = invocations.FirstOrDefault(predicate.Compile());
            if (invocation != null) { return invocation; }

            Assert.Contains(predicate, fakeDbConnection.Invocations, message + "\n\n" + invocations.PrintInvocations(), args);
            throw new AssertionException(string.Format(message, args));
        }

        public static DbCommand ShouldHaveSelected(this FakeDbConnection fakeDbConnection,
                                                string tableName,
                                                IEnumerable<string> fieldList = null,
                                                string whereClauseField = null, object expectedWhereClauseValue = null)
        {
            var verbpattern = @select;
            var frompattern = @"(From|Join)\s+" + optPrefix + optDelim + tableName;
            var invocation = fakeDbConnection.ShouldHaveInvoked(
                        i => i.CommandText.Matches(verbpattern, sqlRegexOpts)
                          && i.CommandText.Matches(frompattern, sqlRegexOpts),
                      "Expected to Select from table {0} but didn't see it.", tableName);

            var columnList = new Regex(@"(?<=" + @select + ")" + restofline + @"(?=\s+From)", sqlRegexOpts).Matches(invocation.CommandText)[0].Value;
            foreach (var field in fieldList ?? new string[] { })
            {
                var field_ = field;
                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", field_);
                columnList.ShouldMatch(string.Format(@"({0}|{1}){2}", openBracket, comma, fieldOrQuotedField),
                    sqlRegexOpts, "Expected to select column {0} but didn't see it", field);
            }
            if (whereClauseField != null)
            {
                expectedWhereClauseValue = expectedWhereClauseValue ?? invocation.Parameters[whereClauseField].Value;
                ShouldHaveWhereClauseWithFieldEqualsExpected(invocation, whereClauseField, expectedWhereClauseValue);
            }
            return invocation;
        }

        public static FakeDbCommand ShouldHaveInserted<T>(this FakeDbConnection fakeDbConnection, string tableName, T updateSource)
        {
            var dbRehydratablePropertyNamesExIdFields = updateSource.GetType().GetDbRehydratablePropertyNames().Where(s => !s.EndsWith("id", true, null));
            return ShouldHaveInserted(fakeDbConnection, tableName, dbRehydratablePropertyNamesExIdFields);
        }

        public static FakeDbCommand ShouldHaveInserted(this FakeDbConnection fakeDbConnection, string tableName, IEnumerable<string> fieldList)
        {
            var verbandtablepattern = "Insert" + @"\s+" + @"(Into\s+)?" + optPrefix + optDelim + tableName;
            var cmd = fakeDbConnection.Invocations.First(c => c.CommandText.Matches(verbandtablepattern, sqlRegexOpts));
            cmd.CommandText.ShouldMatch(verbandtablepattern + optDelim + openBracket, sqlRegexOpts);
            var columnList = new Regex(openBracket + restofline, sqlRegexOpts).Matches(cmd.CommandText)[0].Value;
            var valuesList = new Regex(closeBracket + @"Values" + openBracket + restofline, sqlRegexOpts).Matches(cmd.CommandText)[0].Value;
            foreach (var field in fieldList)
            {
                var field_ = field;
                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", field_);
                columnList.ShouldMatch(string.Format(@"({0}|{1}){2}", openBracket, comma, fieldOrQuotedField),
                    sqlRegexOpts,
                    "Expected to insert column {0} but didn't see it", field);
                valuesList.ShouldMatch(string.Format(@"(\(\s+|,\s*)@{0}\s*", field),
                    sqlRegexOpts,
                    "Expected to insert value {0} but didn't see it", field);
                cmd.Parameters.Cast<FakeDbParameter>().SingleOrAssertFail(p => p.ParameterName == field_);
            }
            return cmd;
        }

        public static DbCommand ShouldHaveDeleted(this FakeDbConnection fakeDbConnection, string tableName)
        {
            var verbandtablepattern = "Delete" + @"\s+" + optPrefix + optDelim + tableName;

            return fakeDbConnection
                    .ShouldHaveInvoked(
                            ii => ii.CommandText.Matches(verbandtablepattern, sqlRegexOpts),
                            "Expected to Delete {0} but found no matching update command.",
                            tableName);
        }

        public static DbCommand ShouldHaveDeleted(this FakeDbConnection fakeDbConnection, string tableName, string whereClauseFieldName, object expectedId)
        {
            var verbandtablepattern = "Delete" + @"\s+" + optPrefix + optDelim + tableName;

            var invocation = fakeDbConnection.ShouldHaveInvoked(
                        ii => ii.CommandText.Matches(verbandtablepattern, sqlRegexOpts)
                              && ii.Parameters.Cast<FakeDbParameter>().Any(p => p.ParameterName == whereClauseFieldName && p.Value.Equals(expectedId)),
                        "Expected to Delete {0} but found no matching Delete command.",
                        tableName);
            ShouldHaveWhereClauseWithFieldEqualsExpected(invocation, whereClauseFieldName, expectedId);
            return invocation;
        }

        public static FakeDbCommand ShouldHaveUpdatedNRows<T>(this FakeDbConnection fakeDbConnection, string tableName, T updateSource, int times = 1)
        {
            var dbRehydratablePropertyNamesExIdFields =
                updateSource.GetType().GetDbRehydratablePropertyNames().Where(s => !s.EndsWith("id", true, null));

            return ShouldHaveUpdatedNRows(fakeDbConnection, tableName, dbRehydratablePropertyNamesExIdFields, times);
        }

        public static FakeDbCommand ShouldHaveUpdatedNRows(
                            this FakeDbConnection fakeDbConnection, string tableName, IEnumerable<string> fieldList, int times = 1)
        {
            var verbandtablepattern = "Update" + @"\s+" + optPrefix + optDelim + tableName;
            var cmd = fakeDbConnection.Invocations.First(c => c.CommandText.Matches(verbandtablepattern, sqlRegexOpts));
            var commandText = cmd.CommandText;
            cmd.CommandText.ShouldMatch(verbandtablepattern + optDelim + set, sqlRegexOpts);
            var afterSet = new Regex(set + restofline, sqlRegexOpts).Matches(commandText)[0].Value;
            foreach (var field in fieldList)
            {
                var field_ = field;
                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", field_);
                afterSet.ShouldMatch(
                    string.Format(@"({0}|{1}){2}\s*=\s*\@{3}", set, comma, fieldOrQuotedField, field),
                    sqlRegexOpts,
                    "Expected to update field {0} but didn't see it", field);
                cmd.Parameters.Cast<DbParameter>().SingleOrAssertFail(p => p.ParameterName == field_);
            }
            cmd.Invocations.Count.ShouldBe(times);
            return cmd;
        }

        public static DbCommand ShouldHaveUpdated<T>(this FakeDbConnection fakeDbConnection, string tableName, T updateSource, string whereClauseField)
        {
            var dbRehydratablePropertyNamesExIdFields =
                updateSource.GetType().GetDbRehydratablePropertyNames().Where(s => s != whereClauseField);
            var expectedWhereValue = updateSource.GetType().GetProperty(whereClauseField).GetPropertyValue(updateSource, whereClauseField);

            return ShouldHaveUpdated(fakeDbConnection, tableName, dbRehydratablePropertyNamesExIdFields, whereClauseField, expectedWhereValue);
        }

        public static DbCommand ShouldHaveUpdated<T>(
            this FakeDbConnection fakeDbConnection, string tableName, IEnumerable<string> fieldList, string whereClauseField, T whereClauseFieldExpectedValue)
        {
            var verbandtablepattern = "Update" + @"\s+" + optPrefix + optDelim + tableName;

            var invocation = fakeDbConnection.ShouldHaveInvoked(
                    ii => ii.CommandText.Matches(verbandtablepattern, sqlRegexOpts)
                       && ii.Parameters.Cast<FakeDbParameter>().Any(p => p.ParameterName == whereClauseField && p.Value.Equals(whereClauseFieldExpectedValue)),
                    "Expected to Update {0} but found no matching update command.", tableName);

            var commandText = invocation.CommandText;
            invocation.CommandText.ShouldMatch(verbandtablepattern + optDelim + set, sqlRegexOpts);
            var afterSet = new Regex(set + restofline, sqlRegexOpts).Matches(commandText)[0].Value;
            foreach (var field in fieldList)
            {
                var field_ = field;
                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", field_);
                afterSet.ShouldMatch(
                    string.Format(@"({0}|{1}){2}\s*=\s*\@{3}", set, comma, fieldOrQuotedField, field),
                    sqlRegexOpts,
                    "Expected to update field {0} but didn't see it", field);
                invocation.Parameters.Cast<DbParameter>().SingleOrAssertFail(p => p.ParameterName == field_);
            }
            ShouldHaveWhereClauseWithFieldEqualsExpected(invocation, whereClauseField, whereClauseFieldExpectedValue);
            return invocation;
        }

        public static DbCommand ShouldHaveWhereClauseWithFieldEqualsExpected<T>(this DbCommand invocation, string columnName, T expectedValue)
        {
            invocation.CommandText.ShouldMatch(@"Where " + optPrefix + optDelim + columnName + optDelim + @"\s*\=\s*@" + columnName, sqlRegexOpts);
            invocation.ShouldHaveParameter(columnName, expectedValue);
            return invocation;
        }

        public static DbCommand ShouldHaveParameter<T>(this DbCommand invocation, string parameterName, T expectedValue)
        {
            var found = invocation.Parameters.Cast<DbParameter>()
                .Any(
                    p => p.ParameterName.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase)
                         && p.Value.Equals(expectedValue)
                );
            if (!found)
            {
                throw new AssertionException(
                    string.Format(
                    "\n\nExpected:\n\nclause: '{0} = @{0}'\n\nwith actual value: @{0}='{1}'\n\nBut was:\n\n{2}",
                    parameterName, expectedValue, invocation.ToStringTextAndParams()));
            }
            return invocation;
        }
    }
}
