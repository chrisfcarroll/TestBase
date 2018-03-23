using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using TestBase.Mono.Linq.Expressions;

namespace TestBase.AdoNet
{
    public static class FakeDbSqlShouldsByRegexMatching
    {
        public static readonly RegexOptions SqlRegexOpts = RegexOptions.IgnoreCase | RegexOptions.Singleline;
        public const string optDelim = @"(\[|\]|"")?";
        public const string optPrefix = @"((\w|[\[\]\""])+\.)*";
        public const string openBracket = @"\s*\(\s*";
        public const string closeBracket = @"\s*\)\s*";
        public const string ColumnNameDelimiterBefore = @"(\s|\.|,|^)" ;
        public const string ColumnNameDelimiterAfter = @"(\s|\.|,|$)";
        public const string comma = @"\s*,\s*";
        public const string set = @"\s+Set\s+";
        public const string restofline = ".*";
        public const string select = @"Select\s+";
        public const string fromOrJoin = @"(From|Join)\s+";
        const string optInto = @"(Into\s+)?";
        const string optFrom = @"(From\s+)?";

        /// <summary>
        /// Verifies that a command was invoked on <paramref name="fakeDbConnection"/> which satisfied <paramref name="predicate"/>
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveInvoked(this FakeDbConnection fakeDbConnection, 
                                                    Expression<Func<DbCommand, bool>> predicate, string message=null, params object[] args)
        {
            var invocations = fakeDbConnection.Invocations;
            var invocation = invocations.FirstOrDefault(predicate.Compile());
            if (invocation != null) { return invocation; }
            //
            message = (message == null) 
                    ? "Expected to invoke a SQL command matching " + predicate.ToCSharpCode() 
                    : string.Format(message, args);

            throw new ShouldHaveThrownException(message + invocations.PrintInvocations());
        }

        /// <summary>
        /// Verifies that a Select command was invoked on <paramref name="fakeDbConnection"/> which
        /// selected from table <paramref name="tableName"/>.
        /// 
        /// Optionally verifies that columns named in <paramref name="fieldList"/> were selected
        /// Optionally verifies that the where clause included "<paramref name="whereClauseField"/> =  <paramref name="expectedWhereClauseValue"/>"
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveSelected(this FakeDbConnection fakeDbConnection,
                                                string tableName,
                                                IEnumerable<string> fieldList = null,
                                                string whereClauseField = null, object expectedWhereClauseValue = null)
        {
            var verbpattern = select;
            var frompattern = fromOrJoin + optPrefix + optDelim + tableName;
            var invocation = fakeDbConnection.ShouldHaveInvoked(
                        i => i.CommandText.Matches(verbpattern, SqlRegexOpts)
                          && i.CommandText.Matches(frompattern, SqlRegexOpts),
                      "Expected to Select from table {0} but didn't see it.", tableName);

            var columnList = new Regex(@"(?<=" + @select + ")" + restofline + @"(?=\s+From)", SqlRegexOpts).Matches(invocation.CommandText)[0].Value;
            foreach (var columnName in fieldList ?? new string[] { })
            {
                var columnNameMaybeQuoted = string.Format(@"({0}|\[{0}\]|""{0}"")", columnName);
                columnList.ShouldMatch(
                    $@"{ColumnNameDelimiterBefore}{columnNameMaybeQuoted}{ColumnNameDelimiterAfter}",
                    SqlRegexOpts, $"Expected to Select column {columnName} but didn't see it");
            }
            if (whereClauseField != null)
            {
                expectedWhereClauseValue = expectedWhereClauseValue ?? invocation.Parameters[whereClauseField].Value;
                ShouldHaveWhereClauseWithColumnEqualsExpected(invocation, whereClauseField, expectedWhereClauseValue);
            }
            return invocation;
        }

        /// <summary>
        /// Verifies that a Select command was invoked on <paramref name="fakeDbConnection"/> which
        /// selected from table <paramref name="tableName"/>.
        /// 
        /// Verifies that the select clause includes columns named for the public Properties of <paramref name="withColumnsForEachRehydratablePropertyOf"/> which are <seealso cref="DbRehydrationExtensions.GetDbRehydratablePropertyNames"/> 
        /// Optionally verifies that the where clause included "<paramref name="whereClauseField"/> =  <paramref name="expectedWhereClauseValue"/>"
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveSelected<T>(this FakeDbConnection fakeDbConnection,
                                                   string tableName,
                                                   T withColumnsForEachRehydratablePropertyOf,
                                                   IEnumerable<string> exceptProperties = null,
                                                   string whereClauseField = null,
                                                   object expectedWhereClauseValue = null)
        {
            var fieldList = typeof(T).GetDbRehydratablePropertyNames().Except(exceptProperties ?? new string[0]);
            return ShouldHaveSelected(fakeDbConnection, tableName, fieldList, whereClauseField, expectedWhereClauseValue);
        }
        /// <summary>
        /// Verifies that a Select command was invoked on <paramref name="fakeDbConnection"/> which
        /// selected from table <paramref name="tableName"/>.
        /// 
        /// Verifies that the select clause includes columns named for the public Properties of <paramref name="withColumnsForEachRehydratablePropertyOf"/> which are <seealso cref="DbRehydrationExtensions.GetDbRehydratablePropertyNames"/> 
        /// Optionally verifies that the where clause included "<paramref name="whereClauseField"/> =  <paramref name="expectedWhereClauseValue"/>"
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveSelected<T>(this FakeDbConnection fakeDbConnection,
                                                      string tableName,
                                                      IEnumerable<string> exceptProperties = null,
                                                      string whereClauseField = null,
                                                      object expectedWhereClauseValue = null)
        {
            var fieldList = typeof(T).GetDbRehydratablePropertyNames().Except(exceptProperties ?? new string[0]);
            return ShouldHaveSelected(fakeDbConnection, tableName, fieldList, whereClauseField, expectedWhereClauseValue);
        }

        /// <summary>
        /// Verifies that an Insert command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names, parameter names, and parameter values matche the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Insert tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
        /// 
        /// And the Parameters are {Name="PropertyName1", Value=updateSource.PropertyName1} [, {Name="PropertyNameN", Value=updateSource.PropertyNameN} ...] 
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveInserted<T>(this FakeDbConnection fakeDbConnection, 
                                                        string tableName, T updateSource, IEnumerable<string> exceptProperties=null)
        {
            if (updateSource is IEnumerable<string> && exceptProperties==null)
            {
                return ShouldHaveInsertedColumns(fakeDbConnection,tableName, updateSource as IEnumerable<string>);
            }
            exceptProperties = exceptProperties ?? new string[0];
            var dbParameterisablePropertyNames = 
                typeof(T).GetDbRehydratablePropertyNames().Where(s => !exceptProperties.Contains(s, StringComparer.CurrentCultureIgnoreCase));
            return ShouldHaveInserted(fakeDbConnection, tableName, dbParameterisablePropertyNames, updateSource);
        }

        /// <summary>
        /// Verifies that an Insert command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>
        /// and returns the first such command recorded.
        /// 
        /// Verifies that the column names in <paramref name="columnList"/> were all updated
        /// i.e. that the command is something like 
        /// "Insert tableName Set PropertyName1 = _ [, PropertyNameN = _ ...]"
        /// And the Parameters also have names Name="PropertyName1" [, Name="PropertyNameN" ...] 
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveInsertedColumns(this FakeDbConnection fakeDbConnection, string tableName, IEnumerable<string> columnList)
        {
            return ShouldHaveInserted(fakeDbConnection, tableName, columnList, updateSource:null);
        }

        /// <summary>
        /// Verifies that an Insert command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/> 
        /// and returns the first such command recorded.
        /// </summary>
        /// <returns>The matching <see cref="DbCommand"/></returns>
        public static DbCommand ShouldHaveInserted(this FakeDbConnection fakeDbConnection, string tableName)
        {
            return ShouldHaveInserted(fakeDbConnection, tableName, new string[0]);
        }


        /// <summary>
        /// Verifies that an Insert command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>
        /// and returns the first such command recorded.
        /// 
        /// Verifies that the column names in <paramref name="columnList"/> were all updated
        /// Optionally verifies that the parameter names and values matche the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Insert tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
        /// And the Parameters are {Name="PropertyName1", Value=updateSource.PropertyName1} [, {Name="PropertyNameN", Value=updateSource.PropertyNameN} ...] 
        /// </summary>
        /// <param name="columnList">This should be a subset of the names of Properties of <paramref name="updateSource"/></param>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveInserted(this FakeDbConnection fakeDbConnection, string tableName, IEnumerable<string> columnList, object updateSource)
        {
            var verbandtablepattern = @"Insert\s+" + optInto + optPrefix + optDelim + tableName;
            var cmd = fakeDbConnection.ShouldHaveInvoked(c => c.CommandText.Matches(verbandtablepattern, SqlRegexOpts),"Expected to Insert table {0} but didn't see it",tableName);
            cmd.CommandText.ShouldMatch(verbandtablepattern + optDelim + openBracket, SqlRegexOpts);
            var foundColumnList = new Regex(openBracket + restofline, SqlRegexOpts).Matches(cmd.CommandText)[0].Value;
            var valuesList = new Regex(closeBracket + @"Values" + openBracket + restofline, SqlRegexOpts).Matches(cmd.CommandText)[0].Value;
            foreach (var field in columnList)
            {
                var fieldName = field;
                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", fieldName);
                foundColumnList.ShouldMatch(string.Format(@"({0}|{1}){2}", openBracket, comma, fieldOrQuotedField),
                    SqlRegexOpts,
                    "Expected to Insert column {0} but didn't see it", field);
                valuesList.ShouldMatch(string.Format(@"(\(\s*|,\s*)@{0}\s*", field),
                    SqlRegexOpts,
                    "Expected to Insert value {0} but didn't see it", field);
                if (updateSource != null) {
                    cmd.ShouldHaveParameter(fieldName, updateSource.GetType().GetProperty(fieldName).GetValue(updateSource, null));
                }
                else
                {
                    cmd.Parameters
                        .Cast<FakeDbParameter>()
                        .SingleOrAssertFail(p => p.ParameterName.Equals(fieldName,StringComparison.CurrentCultureIgnoreCase),
                                "Found Insert command {0} but with parameters\n:{1}\n\nExpected: Parameter named {2}",
                                cmd.CommandText,
                                cmd.Parameters.ToStringPerLine(), 
                            fieldName);
                }
            }
            return cmd;
        }

        ///<summary>
        /// Verifies that a Delete command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveDeleted(this FakeDbConnection fakeDbConnection, string tableName)
        {
            var verbandtablepattern = @"Delete\s+" + optFrom + optPrefix + optDelim + tableName;

            return fakeDbConnection
                    .ShouldHaveInvoked(
                            ii => ii.CommandText.Matches(verbandtablepattern, SqlRegexOpts),
                            "Expected to Delete {0} but found no matching Delete command.",
                            tableName);
        }

        /// <summary>
        /// Verifies that a Delete command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
        /// Verifies that the where clause of the Delete command includes <paramref name="whereClauseFieldName"/> = <paramref name="expectedWhereClauseValue"/>
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveDeleted(this FakeDbConnection fakeDbConnection, string tableName, string whereClauseFieldName, object expectedWhereClauseValue)
        {
            var verbandtablepattern = @"Delete\s+" + optFrom + optPrefix + optDelim + tableName;

            var invocation = fakeDbConnection.ShouldHaveInvoked(
                        ii => ii.CommandText.Matches(verbandtablepattern, SqlRegexOpts)
                              && ii.Parameters.Cast<FakeDbParameter>().Any(p => p.ParameterName == whereClauseFieldName && p.Value.Equals(expectedWhereClauseValue)),
                        "Expected to Delete {0} but found no matching Delete command.",
                        tableName);
            ShouldHaveWhereClauseWithColumnEqualsExpected(invocation, whereClauseFieldName, expectedWhereClauseValue);
            return invocation;
        }

        /// <summary>
        /// Verifies that an Update command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names and parameter names match the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Update tableName Set PropertyName1 = _ [, PropertyNameN = _ ...]"
        /// 
        /// Verifies that the update command was called <paramref name="times"/> 
        /// (i.e. the same command called with <paramref name="times"/>) different sets of parameters)
        /// 
        /// NOTE This overload does not verify parameter values
        /// You can do that with the <see cref="DbCommand.DbParameterCollection"/> property of the returned <see cref="FakeDbCommand.Invocations"/>
        /// Or by using <see cref="ShouldHaveUpdated{T}(TestBase.AdoNet.FakeDbConnection,string,T,string)"/> instead.
        /// </summary>
        /// <returns>The matching command</returns>
        public static FakeDbCommand ShouldHaveExecutedNTimes<T>(this FakeDbConnection fakeDbConnection, 
                                                                        string verb, string tableName, T updateSource, int times = 1)
        {
            var dbRehydratablePropertyNamesExIdFields =
                updateSource.GetType().GetDbRehydratablePropertyNames().Where(s => !s.EndsWith("id",StringComparison.CurrentCultureIgnoreCase));

            return ShouldHaveExecutedNTimes(fakeDbConnection, verb, tableName, dbRehydratablePropertyNamesExIdFields, times);
        }

        /// <summary>
        /// Verifies that an Update command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names and parameter names match the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Update tableName Set PropertyName1 = _ [, PropertyNameN = _ ...]"
        /// 
        /// Verifies that the update command was called <paramref name="times"/> 
        /// (i.e. the same command called with <paramref name="times"/>) different sets of parameters)
        /// 
        /// NOTE This overload does not verify parameter values
        /// You can do that with the <see cref="DbCommand.DbParameterCollection"/> property of the returned <see cref="FakeDbCommand.Invocations"/>
        /// Or by using <see cref="ShouldHaveUpdated{T}(FakeDbConnection,string,T,string)"/> instead.
        /// </summary>
        /// <returns>The matching command</returns>
        public static FakeDbCommand ShouldHaveExecutedNTimes(this FakeDbConnection fakeDbConnection, 
                                            string verb, string tableName, IEnumerable<string> fieldList, int times = 1)
        {
            var verbandtablepattern = verb + @"\s+" + optPrefix + optDelim + tableName;

            var possiblyRelevantCommands = fakeDbConnection.Invocations.Where(c => c.CommandText.Matches(verbandtablepattern + optDelim , SqlRegexOpts));

            var matches = 0;

            foreach (var cmd in possiblyRelevantCommands)
            {
                try
                {
                    var afterVerbAndTable = new Regex(@"(?<=" + @verbandtablepattern + ")" + restofline, SqlRegexOpts).Matches(cmd.CommandText)[0].Value;
                    foreach (var field in fieldList)
                    {
                        var field_ = field;
                        var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", field_);
                        afterVerbAndTable.ShouldMatch(
                                    string.Format(@"({0}|{1}){2}\s*=\s*\@{3}", set, comma, fieldOrQuotedField, field),
                                    SqlRegexOpts,
                                    "Expected to {0} field {1} but didn't see it", verb, field);
                        cmd.Parameters.Cast<DbParameter>().SingleOrAssertFail(p => p.ParameterName == field_);
                    }
                    matches++;
                }
                catch (ShouldHaveThrownException)
                {
                    //swallow because at this point we are just counting. Throw below.
                }
            }
            matches.ShouldBe(
                times,
                string.Format("Expected to invoke {0} {1} commands against table {2} but got {3}", times, verb, tableName, matches));
            return possiblyRelevantCommands.FirstOrDefault();
        }

        public static DbCommand ShouldHaveExecutedNTimes(this DbCommand dbCommand, int times = 1)
        {
            if (times != 1)
            {
                var fakeDbCommand = dbCommand as FakeDbCommand;
                if (fakeDbCommand == null)
                {
                    throw new ShouldHaveThrownException($"ShouldHaveUpdatedNRows(times != 1) can only be asserted against a {typeof(FakeDbCommand)} not a {dbCommand.GetType()}");
                }
                else
                {
                    fakeDbCommand.Invocations.Count.ShouldBe(times);
                }
            }
            return dbCommand;
        }

        /// <summary>
        /// Verifies that an Update command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names, parameter names and property values match the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Update tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveUpdated<T>(this FakeDbConnection fakeDbConnection, string tableName, T updateSource, string whereClauseColumnName)
        {
            var whereClausePropInfo = updateSource.GetType().GetProperty(whereClauseColumnName);
            var expectedWhereValue = whereClausePropInfo?.GetPropertyValue(updateSource, whereClauseColumnName);

            return ShouldHaveUpdated(fakeDbConnection, tableName, updateSource, whereClauseColumnName, expectedWhereValue);
        }
        /// <summary>
        /// Verifies that an Update command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names, parameter names and property values match the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Update tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveUpdated<TSource,TWhere>(this FakeDbConnection fakeDbConnection, string tableName, TSource updateSource, string whereClauseColumnName, TWhere whereClauseColumnExpectedValue)
        {
            var dbParameterisablePropertyNamesExWhereClause = updateSource.GetType().GetDbRehydratablePropertyNames().Where(s => s != whereClauseColumnName);
            return ShouldHaveUpdated(fakeDbConnection, tableName, dbParameterisablePropertyNamesExWhereClause, whereClauseColumnName, whereClauseColumnExpectedValue);
        }

        /// <summary>
        /// Verifies that an Update command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names, parameter names and property values match the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Update tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
        /// 
        /// Verifies that the where clause includes "<paramref name="whereClauseColumnName"/> = <paramref name="whereClausePropertyExpectedValue"/>"
        /// 
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveUpdated<T>(this FakeDbConnection fakeDbConnection, string tableName, IEnumerable<string> fieldList, string whereClauseColumnName, T whereClausePropertyExpectedValue)
        {
            var verbandtablepattern = "Update" + @"\s+" + optPrefix + optDelim + tableName;

            var invocation = fakeDbConnection.ShouldHaveInvoked(
                    ii => ii.CommandText.Matches(verbandtablepattern, SqlRegexOpts)
                       && ii.Parameters.Cast<FakeDbParameter>().Any(p => p.ParameterName == whereClauseColumnName && p.Value.Equals(whereClausePropertyExpectedValue)),
                    "Expected to Update {0} but found no matching update command.", tableName);

            invocation.CommandText.ShouldMatch(verbandtablepattern + optDelim + set, SqlRegexOpts);
            ShouldHaveUpdatedFields(invocation, fieldList);
            ShouldHaveWhereClauseWithColumnEqualsExpected(invocation, whereClauseColumnName, whereClausePropertyExpectedValue);
            return invocation;
        }

        public static void ShouldHaveUpdatedFields(this DbCommand invocation, IEnumerable<string> fieldList, bool fromIdenticallyNamedParameters=false)
        {
            var SetClause = new Regex(set + restofline, SqlRegexOpts).Matches(invocation.CommandText)[0].Value;
            foreach (var field in fieldList)
            {
                var fieldName = field;
                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", fieldName);
                var setFieldToValueClaus = fromIdenticallyNamedParameters
                                        ? string.Format(@"({0}|{1}){2}\s*=\s*\@{3}", set, comma, fieldOrQuotedField, field)
                                        : string.Format(@"({0}|{1}){2}\s*=", set, comma, fieldOrQuotedField);
                SetClause.ShouldMatch(
                                     setFieldToValueClaus,
                                     SqlRegexOpts,
                                     "Expected to update field {0} but didn't see it",
                                     field);
                if (Regex.IsMatch(SetClause, string.Format(@"({0}|{1}){2}\s*=\s*\@{3}", set, comma, fieldOrQuotedField, field)))
                {
                    invocation.Parameters.Cast<DbParameter>().SingleOrAssertFail(p => p.ParameterName.Equals(fieldName, StringComparison.CurrentCultureIgnoreCase));
                }
            }
        }

        /// <summary>
        /// Verifies that an Update command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
        /// 
        /// Verifies that the column names, parameter names and property values match the Property names and values of <paramref name="updateSource"/>,
        /// i.e. that the command is something like 
        /// "Update tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
        /// 
        /// Verifies that the where clause includes "<paramref name="whereClauseColumnName"/> = <paramref name="whereClausePropertyExpectedValue"/>"
        /// 
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveUpdatedFieldsFromIdenticallyNamedParameters<T>(this FakeDbConnection fakeDbConnection, string tableName, IEnumerable<string> fieldList, string whereClauseColumnName, T whereClausePropertyExpectedValue)
        {
            var verbandtablepattern = "Update" + @"\s+" + optPrefix + optDelim + tableName;

            var invocation = fakeDbConnection.ShouldHaveInvoked(
                    ii => ii.CommandText.Matches(verbandtablepattern, SqlRegexOpts)
                       && ii.Parameters.Cast<FakeDbParameter>().Any(p => p.ParameterName == whereClauseColumnName && p.Value.Equals(whereClausePropertyExpectedValue)),
                    "Expected to Update {0} but found no matching update command.", tableName);

            invocation.CommandText.ShouldMatch(verbandtablepattern + optDelim + set, SqlRegexOpts);
            invocation.ShouldHaveUpdatedFields(fieldList,true);
            ShouldHaveWhereClauseWithColumnEqualsExpected(invocation, whereClauseColumnName, whereClausePropertyExpectedValue);
            return invocation;
        }

        public static DbCommand ShouldHaveWhereClauseWithColumnEqualsExpected<T>(this DbCommand invocation, string expectedWhereClauseColumnName, T expectedValue)
        {
            invocation.CommandText.ShouldMatch(@"Where\s+.*" + optPrefix + optDelim + expectedWhereClauseColumnName + optDelim + @"\s*\=\s*@" + expectedWhereClauseColumnName, SqlRegexOpts, $"ShouldHaveWhereClauseWith {expectedWhereClauseColumnName} = @{expectedWhereClauseColumnName}");
            invocation.ShouldHaveParameter(expectedWhereClauseColumnName, expectedValue);
            return invocation;
        }

        public static DbCommand ShouldHaveWhereClauseMatching(this DbCommand invocation, string expectedWhereClausePattern)
        {
            if (!Regex.IsMatch(expectedWhereClausePattern, @"^\s*Where\s+", SqlRegexOpts)) { expectedWhereClausePattern = @"Where\s+.*" + expectedWhereClausePattern;}
            invocation.CommandText.ShouldMatch(expectedWhereClausePattern);
            return invocation;
        }
        public static DbCommand ShouldHaveWhereClauseContaining(this DbCommand invocation, string expectedWhereClauseSubstring)
        {
            if (!Regex.IsMatch(expectedWhereClauseSubstring, @"^\s*Where\s+", SqlRegexOpts)) { expectedWhereClauseSubstring = @"Where\s+.*" + expectedWhereClauseSubstring; }
            invocation.CommandText.ToLower().ShouldContain(expectedWhereClauseSubstring.ToLower());
            return invocation;
        }


        /// <summary>
        /// Verifies that the <paramref name="dbCommand"/>'s <see cref="DbCommand.DbParameterCollection"/> includes a parameter 
        /// with the given <paramref name="parameterName"/> and <paramref name="expectedValue"/>
        /// </summary>
        /// <returns><paramref name="dbCommand"/></returns>
        public static DbCommand ShouldHaveParameter(this DbCommand dbCommand, string parameterName, object expectedValue)
        {
            dbCommand
                .Parameters.Cast<DbParameter>()
                .ShouldContain(p => p.ParameterName.Equals(parameterName, StringComparison.CurrentCultureIgnoreCase)
                                 && p.Value.Equals(expectedValue ?? DBNull.Value),
                               $"Should have parameter {parameterName}={expectedValue}\n\nfor command\n{dbCommand.CommandText}");
            return dbCommand;
        }
    }
}
