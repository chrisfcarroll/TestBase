using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using ExpressionToCodeLib;

namespace TestBase.AdoNet.RecordingDb
{
    public static class RecordingDbSqlShouldsByRegexMatching
    {
        static readonly RegexOptions sqlRegexOpts = RegexOptions.IgnoreCase | RegexOptions.Singleline;
        public const string optDelim = @"(\[|\]|"")?";
        public const string optPrefix = @"((\w|[\[\]\""])+\.)*";
        public const string openBracket = @"\s*\(\s*";
        public const string closeBracket = @"\s*\)\s*";
        public const string comma = @"\s*,\s*";
        public const string set = @"\s+Set\s+";
        public const string restofline = ".*";
        public const string select = @"Select\s+";
        public const string fromOrJoin = @"(From|Join)\s+";

        /// <summary>
        /// Verifies that a command was invoked on <paramref name="recordingDbConnection"/> which satisfied <paramref name="predicate"/>
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveInvoked(this RecordingDbConnection recordingDbConnection, Expression<Func<DbCommand, bool>> predicate, string message=null, params object[] args)
        {
            var invocations = recordingDbConnection.Invocations;
            var invocation = invocations.FirstOrDefault(predicate.Compile());
            if (invocation != null) { return invocation; }
            //
            message = (message == null) 
                    ? "Expected to invoke a SQL command matching " + ExpressionToCode.ToCode((Expression) predicate) 
                    : string.Format(message, args);

            throw new Assertion<List<FakeDbCommand>>(invocations,
                        x => false,
                        message
                       );
        }

        /// <summary>
        /// Verifies that a Select command was invoked on <paramref name="recordingDbConnection"/> which
        /// selected from table <paramref name="tableName"/>.
        /// 
        /// Optionally verifies that columns named in <paramref name="fieldList"/> were selected
        /// Optionally verifies that the where clause included "<paramref name="whereClauseField"/> =  <paramref name="expectedWhereClauseValue"/>"
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveSelected(this RecordingDbConnection recordingDbConnection,
                                                string tableName,
                                                IEnumerable<string> fieldList = null,
                                                string whereClauseField = null, object expectedWhereClauseValue = null)
        {
            var verbpattern = select;
            var frompattern = fromOrJoin + optPrefix + optDelim + tableName;
            var invocation = recordingDbConnection.ShouldHaveInvoked(
                        i => i.CommandText.Matches(verbpattern, sqlRegexOpts)
                          && i.CommandText.Matches(frompattern, sqlRegexOpts),
                      "Expected to Select from table {0} but didn't see it.", tableName);

            var columnList = new Regex(@"(?<=" + @select + ")" + restofline + @"(?=\s+From)", sqlRegexOpts).Matches(invocation.CommandText)[0].Value;
            foreach (var field in fieldList ?? new string[] { })
            {
                var field_ = field;
                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", field_);
                StringShoulds.ShouldMatch(columnList,
                                       string.Format(@"({0}|{1}){2}", openBracket, comma, fieldOrQuotedField),
                    sqlRegexOpts, "Expected to Select column {0} but didn't see it", field);
            }
            if (whereClauseField != null)
            {
                expectedWhereClauseValue = expectedWhereClauseValue ?? invocation.Parameters[whereClauseField].Value;
                ShouldHaveWhereClauseWithFieldEqualsExpected(invocation, whereClauseField, expectedWhereClauseValue);
            }
            return invocation;
        }

        /// <summary>
        /// Verifies that an Insert command was invoked on <paramref name="recordingDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names, parameter names, and parameter values matche the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Insert tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
        /// 
        /// And the Parameters are {Name="PropertyName1", Value=updateSource.PropertyName1} [, {Name="PropertyNameN", Value=updateSource.PropertyNameN} ...] 
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveInserted<T>(this RecordingDbConnection recordingDbConnection, string tableName, T updateSource, IEnumerable<string> exceptProperties = null)
        {
            if (updateSource is IEnumerable<string> && exceptProperties == null)
            {
                return ShouldHaveInsertedColumns(recordingDbConnection, tableName, updateSource as IEnumerable<string>);
            }
            exceptProperties = exceptProperties ?? new string[0];
            var dbParameterisablePropertyNames =
            typeof(T).GetDbRehydratablePropertyNames().Where(s => !exceptProperties.Contains(s, StringComparer.InvariantCultureIgnoreCase));
            return ShouldHaveInserted(recordingDbConnection, tableName, dbParameterisablePropertyNames, updateSource);
        }

        /// <summary>
        /// Verifies that an Insert command was invoked on <paramref name="recordingDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names in <paramref name="columnList"/> were all updated
        /// i.e. that the command is something like 
        /// "Insert tableName Set PropertyName1 = _ [, PropertyNameN = _ ...]"
        /// And the Parameters also have names Name="PropertyName1" [, Name="PropertyNameN" ...] 
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveInsertedColumns(this RecordingDbConnection recordingDbConnection, string tableName, IEnumerable<string> columnList)
        {
            return ShouldHaveInserted(recordingDbConnection, tableName, columnList, updateSource: null);
        }

        /// <summary>
        /// Verifies that an Insert command was invoked on <paramref name="recordingDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names in <paramref name="columnList"/> were all updated
        /// Optionally verifies that the parameter names and values matche the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Insert tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
        /// And the Parameters are {Name="PropertyName1", Value=updateSource.PropertyName1} [, {Name="PropertyNameN", Value=updateSource.PropertyNameN} ...] 
        /// </summary>
        /// <param name="columnList">This should be a subset of the names of Properties of <paramref name="updateSource"/></param>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveInserted(this RecordingDbConnection recordingDbConnection, string tableName, IEnumerable<string> columnList, object updateSource)
        {
            var verbandtablepattern = "Insert" + @"\s+" + @"(Into\s+)?" + optPrefix + optDelim + tableName;
            var cmd = recordingDbConnection.ShouldHaveInvoked(c => c.CommandText.Matches(verbandtablepattern, sqlRegexOpts));
            StringShoulds.ShouldMatch(cmd.CommandText, verbandtablepattern + optDelim + openBracket, sqlRegexOpts);
            var foundColumnList = new Regex(openBracket + restofline, sqlRegexOpts).Matches(cmd.CommandText)[0].Value;
            var valuesList = new Regex(closeBracket + @"Values" + openBracket + restofline, sqlRegexOpts).Matches(cmd.CommandText)[0].Value;
            foreach (var field in columnList)
            {
                var fieldName = field;
                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", fieldName);
                StringShoulds.ShouldMatch(foundColumnList,
                                            string.Format(@"({0}|{1}){2}", openBracket, comma, fieldOrQuotedField),
                    sqlRegexOpts,
                    "Expected to Insert column {0} but didn't see it", field);
                StringShoulds.ShouldMatch(valuesList,
                                       string.Format(@"(\(\s*|,\s*)@{0}\s*", field),
                    sqlRegexOpts,
                    "Expected to Insert value {0} but didn't see it", field);
                if (updateSource != null)
                {
                    cmd.ShouldHaveParameter(fieldName, updateSource.GetType().GetProperty(fieldName).GetValue(updateSource, null));
                }
                else
                {
                    cmd.Parameters
                        .Cast<FakeDbParameter>()
                        .SingleOrAssertFail(p => p.ParameterName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase),
                                "Found Insert command {0} but with parameters\n:{1}\n\nExpected: Parameter named {2}",
                                cmd.CommandText,
                                DbParameterToStringExtensions.ToStringPerLine(cmd.Parameters),
                            fieldName);
                }
            }
            return cmd;
        }


        ///<summary>
        /// Verifies that a Delete command was invoked on <paramref name="recordingDbConnection"/> on table <paramref name="tableName"/>.
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveDeleted(this RecordingDbConnection recordingDbConnection, string tableName)
        {
            var verbandtablepattern = "Delete" + @"\s+" + optPrefix + optDelim + tableName;

            return recordingDbConnection
                    .ShouldHaveInvoked(
                            ii => ii.CommandText.Matches(verbandtablepattern, sqlRegexOpts),
                            "Expected to Delete {0} but found no matching Delete command.",
                            tableName);
        }

        /// <summary>
        /// Verifies that a Delete command was invoked on <paramref name="recordingDbConnection"/> on table <paramref name="tableName"/>.
        /// Verifies that the where clause of the Delete command includes <paramref name="whereClauseFieldName"/> = <paramref name="expectedWhereClauseValue"/>
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveDeleted(this RecordingDbConnection recordingDbConnection, string tableName, string whereClauseFieldName, object expectedWhereClauseValue)
        {
            var verbandtablepattern = "Delete" + @"\s+" + optPrefix + optDelim + tableName;

            var invocation = recordingDbConnection.ShouldHaveInvoked(
                        ii => ii.CommandText.Matches(verbandtablepattern, sqlRegexOpts)
                              && ii.Parameters.Cast<DbParameter>().Any(p => p.ParameterName == whereClauseFieldName && p.Value.Equals(expectedWhereClauseValue)),
                        "Expected to Delete {0} but found no matching Delete command.",
                        tableName);
            ShouldHaveWhereClauseWithFieldEqualsExpected(invocation, whereClauseFieldName, expectedWhereClauseValue);
            return invocation;
        }

        /// <summary>
        /// Verifies that an Update command was invoked on <paramref name="recordingDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names and parameter names match the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Update tableName Set PropertyName1 = _ [, PropertyNameN = _ ...]"
        /// 
        /// Verifies that the update command was called <paramref name="times"/> 
        /// (i.e. the same command called with <paramref name="times"/>) different sets of parameters)
        /// 
        /// NOTE This overload does not verify parameter values
        /// You can do that with the <see cref="DbCommand.DbParameterCollection"/> property of the returned <see cref="RecordingDbCommand.Invocations"/>
        /// Or by using <see cref="ShouldHaveUpdated{T}(RecordingDbConnection,string,T,string)"/> instead.
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveUpdatedNRows<T>(this RecordingDbConnection recordingDbConnection, string tableName, T updateSource, int times = 1)
        {
            var dbRehydratablePropertyNamesExIdFields =
            updateSource.GetType().GetDbRehydratablePropertyNames().Where(s => !s.EndsWith("id", true, null));

            return ShouldHaveUpdatedNRows(recordingDbConnection, tableName, dbRehydratablePropertyNamesExIdFields, times);
        }

        public static DbCommand ShouldHaveUpdatedNRows(
                            this RecordingDbConnection recordingDbConnection, string tableName, IEnumerable<string> fieldList, int times = 1)
        {
            var verbandtablepattern = "Update" + @"\s+" + optPrefix + optDelim + tableName;
            var invocations = recordingDbConnection.Invocations.Where(i => i.CommandText.Matches(verbandtablepattern, sqlRegexOpts));
            var cmd = invocations.First();
            var commandText = cmd.CommandText;
            StringShoulds.ShouldMatch(cmd.CommandText, verbandtablepattern + optDelim + set, sqlRegexOpts);
            var afterSet = new Regex(set + restofline, sqlRegexOpts).Matches(commandText)[0].Value;
            foreach (var field in fieldList)
            {
                var field_ = field;
                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", field_);
                StringShoulds.ShouldMatch(afterSet,
                                     string.Format(@"({0}|{1}){2}\s*=\s*\@{3}", set, comma, fieldOrQuotedField, field),
                    sqlRegexOpts,
                    "Expected to Update field {0} but didn't see it", field);
                cmd.Parameters.Cast<DbParameter>().SingleOrAssertFail(p => p.ParameterName == field_);
            }
            BasicShoulds.ShouldBe(cmd.Invocations.Count(), times);
            return cmd;
        }

        /// <summary>
        /// Verifies that an Update command was invoked on <paramref name="recordingDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names, parameter names and property values match the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Update tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveUpdated<T>(this RecordingDbConnection recordingDbConnection, string tableName, T updateSource, string whereClauseProperty)
        {
            var dbParameterisablePropertyNamesExWhereClause =
                    updateSource.GetType().GetDbRehydratablePropertyNames().Where(s => s != whereClauseProperty);

            var whereClausePropInfo = updateSource.GetType().GetProperty(whereClauseProperty);
            var expectedWhereValue = (whereClausePropInfo != null) ? whereClausePropInfo.GetPropertyValue(updateSource, whereClauseProperty) : null;

            return ShouldHaveUpdated(recordingDbConnection, tableName, dbParameterisablePropertyNamesExWhereClause, whereClauseProperty, expectedWhereValue);
        }

        /// <summary>
        /// Verifies that an Update command was invoked on <paramref name="recordingDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names, parameter names and property values match the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Update tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
        /// 
        /// Verifies that the where clause includes "<paramref name="whereClauseProperty"/> = <paramref name="whereClausePropertyExpectedValue"/>"
        /// 
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveUpdated<T>(
            this RecordingDbConnection recordingDbConnection, string tableName, IEnumerable<string> fieldList, string whereClauseProperty, T whereClausePropertyExpectedValue)
        {
            var verbandtablepattern = "Update" + @"\s+" + optPrefix + optDelim + tableName;

            var invocation = recordingDbConnection.ShouldHaveInvoked(
                    ii => ii.CommandText.Matches(verbandtablepattern, sqlRegexOpts)
                       && ii.Parameters.Cast<FakeDbParameter>().Any(p => p.ParameterName == whereClauseProperty && p.Value.Equals(whereClausePropertyExpectedValue)),
                    "Expected to Update {0} but found no matching update command.", tableName);

            var commandText = invocation.CommandText;
            StringShoulds.ShouldMatch(invocation.CommandText, verbandtablepattern + optDelim + set, sqlRegexOpts);
            var afterSet = new Regex(set + restofline, sqlRegexOpts).Matches(commandText)[0].Value;
            foreach (var field in fieldList)
            {
                var fieldName = field;
                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", fieldName);
                StringShoulds.ShouldMatch(afterSet,
                                     string.Format(@"({0}|{1}){2}\s*=\s*\@{3}", set, comma, fieldOrQuotedField, field),
                    sqlRegexOpts,
                    "Expected to update field {0} but didn't see it", field);
                invocation.Parameters.Cast<DbParameter>().SingleOrAssertFail(p => p.ParameterName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase));
            }
            ShouldHaveWhereClauseWithFieldEqualsExpected(invocation, whereClauseProperty, whereClausePropertyExpectedValue);
            return invocation;
        }

        public static DbCommand ShouldHaveWhereClauseWithFieldEqualsExpected<T>(this DbCommand invocation, string columnName, T expectedValue)
        {
            StringShoulds.ShouldMatch(invocation.CommandText, @"Where " + optPrefix + optDelim + columnName + optDelim + @"\s*\=\s*@" + columnName, sqlRegexOpts);
            invocation.ShouldHaveParameter(columnName, expectedValue);
            return invocation;
        }

        /// <summary>
        /// Verifies that the <paramref name="dbCommand"/>'s <see cref="DbCommand.DbParameterCollection"/> includes a parameter 
        /// with the given <paramref name="parameterName"/> and <paramref name="expectedValue"/>
        /// </summary>
        /// <returns><paramref name="dbCommand"/></returns>
        public static DbCommand ShouldHaveParameter(this DbCommand dbCommand, string parameterName, object expectedValue)
        {
            Expression<Func<DbCommand,bool>> found = x=>x.Parameters.Cast<DbParameter>()
                .Any(
                    p => p.ParameterName.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase)
                         && p.Value.Equals(expectedValue ?? DBNull.Value)
                );
            Assert.That(
                    dbCommand,
                    found,
                    string.Format(
                    "\n\nExpected:\n\nclause: '{0} = @{0}'\n\nwith actual value: @{0}='{1}'\n\nBut was:\n\n{2}",
                    parameterName, expectedValue, DbParameterToStringExtensions.ToStringTextAndParams(dbCommand)));
            return dbCommand;
        }
    }
}
