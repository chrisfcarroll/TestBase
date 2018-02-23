//
// WIP
//
//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text.RegularExpressions;
//using Mono.Linq.Expressions;
//using NUnit.Framework;
//using TestBase.Shoulds;

//namespace TestBase.FakeDb.V2
//{
//    public static class FakeDbSqlShouldsByRegexMatching
//    {
//        static readonly RegexOptions sqlRegexOpts = RegexOptions.IgnoreCase | RegexOptions.Singleline;
//        public static readonly Regex SelectListRegex = new Regex(@"(?<=" + @select + ")" + dotStar + @"(?=\s+From)", sqlRegexOpts);
//        public const string whereClausePatternFor = @"Where " + optPrefix + optDelim + "{0}" + optDelim + @"\s*\=\s*@" + "{0}";
//        public const string optDelim = @"(\[|\]|"")?";
//        public const string optPrefix = @"((\w|[\[\]\""])+\.)*";
//        public const string openBracket = @"\s*\(\s*";
//        public const string closeBracket = @"\s*\)\s*";
//        public const string comma = @"\s*,\s*";
//        public const string set = @"\s+Set\s+";
//        public const string dotStar = ".*";
//        public const string select = @"Select\s+";
//        public const string fromOrJoin = @"(From|Join)\s+";
//        const string optInto = @"(Into\s+)?";
//        const string optFrom = @"(From\s+)?";

//        /// <summary>
//        /// Verifies that a command was invoked on <paramref name="fakeDbConnection"/> which satisfied <paramref name="predicate"/>
//        /// </summary>
//        /// <returns>The matching command</returns>
//        public static IEnumerable<DbCommand> ShouldHaveInvoked(this FakeDbConnection fakeDbConnection, 
//                                                    Expression<Func<DbCommand, bool>> predicate, string message=null, params object[] args)
//        {
//            var invocations = fakeDbConnection.Invocations.Where(predicate.Compile());
//            if (invocations.Any() ) { return invocations; }
//            //
//            message = (message == null) 
//                    ? "Expected to invoke a SQL command matching " + predicate.ToCSharpCode() 
//                    : string.Format(message, args);

//            throw new AssertionException(message + invocations.PrintInvocations());
//        }

//        /// <summary>
//        /// Verifies that a Select command was invoked on <paramref name="fakeDbConnection"/> which
//        /// selected from table <paramref name="tableName"/>.
//        /// 
//        /// Optionally verifies that columns named in <paramref name="fieldList"/> were selected
//        /// Optionally verifies that the where clause included "<paramref name="whereClauseField"/> =  <paramref name="expectedWhereClauseValue"/>"
//        /// </summary>
//        /// <returns>The matching command</returns>
//        public static IEnumerable<DbCommand> ShouldHaveSelected(this FakeDbConnection fakeDbConnection,
//                                                string tableName,
//                                                IEnumerable<string> columnList = null,
//                                                string whereClauseField = null, object expectedWhereClauseValue = null)
//        {
//            var verbpattern = select;
//            var frompattern = fromOrJoin + optPrefix + optDelim + tableName;
//            var invocations = fakeDbConnection.ShouldHaveInvoked(
//                        i => i.CommandText.Matches(verbpattern, sqlRegexOpts)
//                          && i.CommandText.Matches(frompattern, sqlRegexOpts),
//                      "Expected to Select from table {0} but didn't see a matching Select ... From ...{0}...", tableName);

//            columnList = columnList ?? new string[0];
//            if (columnList != null & columnList.Any())
//            {
//                Func<DbCommand, bool> hasAllSelectedFields = cmd =>
//                {
//                    var selectListMatches = SelectListRegex.Matches(cmd.CommandText);
//                    if (selectListMatches.Count == 0) { return false; }

//                    var selectList = selectListMatches[0].Value;

//                    return columnList.All(field =>
//                    {
//                        var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", field);
//                        var fieldOrQuotedFieldRegex = new Regex(string.Format(@"({0}|{1}){2}", openBracket, comma, fieldOrQuotedField), sqlRegexOpts);
//                        return fieldOrQuotedFieldRegex.IsMatch(selectList);
//                    });
//                };
//                invocations = invocations.Where(hasAllSelectedFields);

//                invocations.ShouldNotBeEmpty(
//                        "Found Select From table {0} but not with a Select list matching {1}", 
//                        tableName, string.Join(",",columnList));
//            }

//            if (whereClauseField != null)
//            {
//                invocations.ShouldContain(i =>
//                {
//                    expectedWhereClauseValue = expectedWhereClauseValue ?? i.Parameters[whereClauseField].Value;
//                    var whereClauseRegex = new Regex(string.Format(whereClausePatternFor,whereClauseField), sqlRegexOpts);
//                    return whereClauseRegex.IsMatch(i.CommandText) &&
//                           i.Parameters.Cast<DbParameter>()
//                                       .Any(p => p.ParameterName.Equals(whereClauseField, StringComparison.InvariantCultureIgnoreCase)
//                                                 && p.Value.Equals(expectedWhereClauseValue ?? DBNull.Value));
//                },
//                "Found Select {0} From ...{1}... but didn't match expected Where clause {2}={3}",tableName, columnList, whereClauseField, expectedWhereClauseValue);
//            }
//            return invocations;
//        }

//        /// <summary>
//        /// Verifies that an Insert command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
//        /// 
//        /// Verifies that the column names, parameter names, and parameter values matche the Property names and values of <paramref name="updateSource"/>,
//        /// i.e. that the command is something like 
//        /// "Insert tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
//        /// 
//        /// And the Parameters are {Name="PropertyName1", Value=updateSource.PropertyName1} [, {Name="PropertyNameN", Value=updateSource.PropertyNameN} ...] 
//        /// </summary>
//        /// <returns>The matching command</returns>
//        public static DbCommand ShouldHaveInserted<T>(this FakeDbConnection fakeDbConnection, 
//                                                        string tableName, T updateSource, IEnumerable<string> exceptProperties=null)
//        {
//            if (updateSource is IEnumerable<string> && exceptProperties==null)
//            {
//                return ShouldHaveInsertedColumns(fakeDbConnection,tableName, updateSource as IEnumerable<string>);
//            }
//            exceptProperties = exceptProperties ?? new string[0];
//            var dbParameterisablePropertyNames = 
//                typeof(T).GetDbRehydratablePropertyNames().Where(s => !exceptProperties.Contains(s, StringComparer.InvariantCultureIgnoreCase));
//            return ShouldHaveInserted(fakeDbConnection, tableName, dbParameterisablePropertyNames, updateSource);
//        }

//        /// <summary>
//        /// Verifies that an Insert command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
//        /// 
//        /// Verifies that the column names in <paramref name="columnList"/> were all updated
//        /// i.e. that the command is something like 
//        /// "Insert tableName Set PropertyName1 = _ [, PropertyNameN = _ ...]"
//        /// And the Parameters also have names Name="PropertyName1" [, Name="PropertyNameN" ...] 
//        /// </summary>
//        /// <returns>The matching command</returns>
//        public static DbCommand ShouldHaveInsertedColumns(this FakeDbConnection fakeDbConnection, string tableName, IEnumerable<string> columnList)
//        {
//            return ShouldHaveInserted(fakeDbConnection, tableName, columnList, updateSource:null);
//        }

//        /// <summary>
//        /// Verifies that an Insert command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
//        /// 
//        /// Verifies that the column names in <paramref name="columnList"/> were all updated
//        /// Optionally verifies that the parameter names and values matche the Property names and values of <paramref name="updateSource"/>,
//        /// i.e. that the command is something like 
//        /// "Insert tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
//        /// And the Parameters are {Name="PropertyName1", Value=updateSource.PropertyName1} [, {Name="PropertyNameN", Value=updateSource.PropertyNameN} ...] 
//        /// </summary>
//        /// <param name="columnList">This should be a subset of the names of Properties of <paramref name="updateSource"/></param>
//        /// <returns>The matching command</returns>
//        public static DbCommand ShouldHaveInserted(this FakeDbConnection fakeDbConnection, string tableName, IEnumerable<string> columnList, object updateSource)
//        {
//            var verbandtablepattern = @"Insert\s+" + optInto + optPrefix + optDelim + tableName;
//            var cmd = fakeDbConnection.ShouldHaveInvoked(c => c.CommandText.Matches(verbandtablepattern, sqlRegexOpts),"Expected to Insert table {0} but didn't see it",tableName);
//            cmd.CommandText.ShouldMatch(verbandtablepattern + optDelim + openBracket, sqlRegexOpts);
//            var foundColumnList = new Regex(openBracket + dotStar, sqlRegexOpts).Matches(cmd.CommandText)[0].Value;
//            var valuesList = new Regex(closeBracket + @"Values" + openBracket + dotStar, sqlRegexOpts).Matches(cmd.CommandText)[0].Value;
//            foreach (var field in columnList)
//            {
//                var fieldName = field;
//                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", fieldName);
//                foundColumnList.ShouldMatch(string.Format(@"({0}|{1}){2}", openBracket, comma, fieldOrQuotedField),
//                    sqlRegexOpts,
//                    "Expected to Insert column {0} but didn't see it", field);
//                valuesList.ShouldMatch(string.Format(@"(\(\s*|,\s*)@{0}\s*", field),
//                    sqlRegexOpts,
//                    "Expected to Insert value {0} but didn't see it", field);
//                if (updateSource != null) {
//                    cmd.ShouldHaveParameter(fieldName, updateSource.GetType().GetProperty(fieldName).GetValue(updateSource, null));
//                }
//                else
//                {
//                    cmd.Parameters
//                        .Cast<FakeDbParameter>()
//                        .SingleOrAssertFail(p => p.ParameterName.Equals(fieldName,StringComparison.InvariantCultureIgnoreCase),
//                                "Found Insert command {0} but with parameters\n:{1}\n\nExpected: Parameter named {2}",
//                                cmd.CommandText,
//                                cmd.Parameters.ToStringPerLine(), 
//                            fieldName);
//                }
//            }
//            return cmd;
//        }

//        ///<summary>
//        /// Verifies that a Delete command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
//        /// </summary>
//        /// <returns>The matching command</returns>
//        public static IEnumerable<DbCommand> ShouldHaveDeleted(this FakeDbConnection fakeDbConnection, string tableName)
//        {
//            var verbandtablepattern = @"Delete\s+" + optFrom + optPrefix + optDelim + tableName;

//            return fakeDbConnection
//                    .ShouldHaveInvoked(
//                            ii => ii.CommandText.Matches(verbandtablepattern, sqlRegexOpts),
//                            "Expected to Delete {0} but found no matching Delete command.",
//                            tableName);
//        }

//        /// <summary>
//        /// Verifies that a Delete command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
//        /// Verifies that the where clause of the Delete command includes <paramref name="whereClauseFieldName"/> = <paramref name="expectedWhereClauseValue"/>
//        /// </summary>
//        /// <returns>The matching command</returns>
//        public static DbCommand ShouldHaveDeleted(this FakeDbConnection fakeDbConnection, string tableName, string whereClauseFieldName, object expectedWhereClauseValue)
//        {
//            var verbandtablepattern = @"Delete\s+" + optFrom + optPrefix + optDelim + tableName;

//            var invocation = fakeDbConnection.ShouldHaveInvoked(
//                        ii => ii.CommandText.Matches(verbandtablepattern, sqlRegexOpts)
//                              && ii.Parameters.Cast<FakeDbParameter>().Any(p => p.ParameterName == whereClauseFieldName && p.Value.Equals(expectedWhereClauseValue)),
//                        "Expected to Delete {0} but found no matching Delete command.",
//                        tableName);
//            ShouldHaveWhereClauseWithFieldEqualsExpected(invocation, whereClauseFieldName, expectedWhereClauseValue);
//            return invocation;
//        }

//        /// <summary>
//        /// Verifies that an Update command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
//        /// 
//        /// Verifies that the column names and parameter names match the Property names and values of <paramref name="updateSource"/>,
//        /// i.e. that the command is something like 
//        /// "Update tableName Set PropertyName1 = _ [, PropertyNameN = _ ...]"
//        /// 
//        /// Verifies that the update command was called <paramref name="times"/> 
//        /// (i.e. the same command called with <paramref name="times"/>) different sets of parameters)
//        /// 
//        /// NOTE This overload does not verify parameter values
//        /// You can do that with the <see cref="DbCommand.DbParameterCollection"/> property of the returned <see cref="FakeDbCommand.Invocations"/>
//        /// Or by using <see cref="ShouldHaveUpdated{T}(TestBase.FakeDb.FakeDbConnection,string,T,string)"/> instead.
//        /// </summary>
//        /// <returns>The matching command</returns>
//        public static FakeDbCommand ShouldHaveExecutedNTimes<T>(this FakeDbConnection fakeDbConnection, 
//                                                                        string verb, string tableName, T updateSource, int times = 1)
//        {
//            var dbRehydratablePropertyNamesExIdFields =
//                updateSource.GetType().GetDbRehydratablePropertyNames().Where(s => !s.EndsWith("id", true, null));

//            return ShouldHaveExecutedNTimes(fakeDbConnection, verb, tableName, dbRehydratablePropertyNamesExIdFields, times);
//        }

//        /// <summary>
//        /// Verifies that an Update command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
//        /// 
//        /// Verifies that the column names and parameter names match the Property names and values of <paramref name="updateSource"/>,
//        /// i.e. that the command is something like 
//        /// "Update tableName Set PropertyName1 = _ [, PropertyNameN = _ ...]"
//        /// 
//        /// Verifies that the update command was called <paramref name="times"/> 
//        /// (i.e. the same command called with <paramref name="times"/>) different sets of parameters)
//        /// 
//        /// NOTE This overload does not verify parameter values
//        /// You can do that with the <see cref="DbCommand.DbParameterCollection"/> property of the returned <see cref="FakeDbCommand.Invocations"/>
//        /// Or by using <see cref="ShouldHaveUpdated{T}(TestBase.FakeDb.FakeDbConnection,string,T,string)"/> instead.
//        /// </summary>
//        /// <returns>The matching command</returns>
//        public static FakeDbCommand ShouldHaveExecutedNTimes(this FakeDbConnection fakeDbConnection, 
//                                            string verb, string tableName, IEnumerable<string> fieldList, int times = 1)
//        {
//            var verbandtablepattern = verb + @"\s+" + optPrefix + optDelim + tableName;

//            var possiblyRelevantCommands = fakeDbConnection.Invocations.Where(c => c.CommandText.Matches(verbandtablepattern + optDelim , sqlRegexOpts));

//            var matches = 0;

//            foreach (var cmd in possiblyRelevantCommands)
//            {
//                try
//                {
//                    var afterSet = new Regex(set + dotStar, sqlRegexOpts).Matches(cmd.CommandText)[0].Value;
//                    foreach (var field in fieldList)
//                    {
//                        var field_ = field;
//                        var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", field_);
//                        afterSet.ShouldMatch(
//                                    string.Format(@"({0}|{1}){2}\s*=\s*\@{3}", set, comma, fieldOrQuotedField, field),
//                                    sqlRegexOpts,
//                                    "Expected to {0} field {1} but didn't see it", verb, field);
//                        cmd.Parameters.Cast<DbParameter>().SingleOrAssertFail(p => p.ParameterName == field_);
//                    }
//                    matches++;
//                }
//                catch (AssertionException ae)
//                {
//                    //swallow and don't increment matches count;
//                }
//            }
//            matches.ShouldBe(times,"Expected to invoke {0} update commands against table {1} but got {2}", times, tableName, matches);
//            return possiblyRelevantCommands.FirstOrDefault();
//        }

//        public static DbCommand ShouldHaveExecutedNTimes(this DbCommand dbCommand, int times = 1)
//        {
//            if (times != 1)
//            {
//                (dbCommand as FakeDbCommand)
//                            .ShouldNotBeNull("ShouldHaveUpdatedNRows(times != 1) can only be asserted against a {0}", typeof(FakeDbCommand))
//                            .Invocations.Count.ShouldBe(times);
//            }
//            return dbCommand;
//        }

//        /// <summary>
//        /// Verifies that an Update command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
//        /// 
//        /// Verifies that the column names, parameter names and property values match the Property names and values of <paramref name="updateSource"/>,
//        /// i.e. that the command is something like 
//        /// "Update tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
//        /// </summary>
//        /// <returns>The matching command</returns>
//        public static DbCommand ShouldHaveUpdated<T>(this FakeDbConnection fakeDbConnection, string tableName, T updateSource, string whereClauseProperty)
//        {
//            var dbParameterisablePropertyNamesExWhereClause =
//                    updateSource.GetType().GetDbRehydratablePropertyNames().Where(s => s != whereClauseProperty);

//            var whereClausePropInfo = updateSource.GetType().GetProperty(whereClauseProperty);
//            var expectedWhereValue = (whereClausePropInfo != null) ? whereClausePropInfo.GetPropertyValue(updateSource, whereClauseProperty) : null;

//            return ShouldHaveUpdated(fakeDbConnection, tableName, dbParameterisablePropertyNamesExWhereClause, whereClauseProperty, expectedWhereValue);
//        }

//        /// <summary>
//        /// Verifies that an Update command was invoked on <paramref name="fakeDbConnection"/> on table <paramref name="tableName"/>.
//        /// 
//        /// Verifies that the column names, parameter names and property values match the Property names and values of <paramref name="updateSource"/>,
//        /// i.e. that the command is something like 
//        /// "Update tableName Set PropertyName1 = updateSource.PropertyName1 [, PropertyNameN = updateSource.PropertyNameN ...]"
//        /// 
//        /// Verifies that the where clause includes "<paramref name="whereClauseProperty"/> = <paramref name="whereClausePropertyExpectedValue"/>"
//        /// 
//        /// </summary>
//        /// <returns>The matching command</returns>
//        public static DbCommand ShouldHaveUpdated<T>(
//            this FakeDbConnection fakeDbConnection, string tableName, IEnumerable<string> fieldList, string whereClauseProperty, T whereClausePropertyExpectedValue)
//        {
//            var verbandtablepattern = "Update" + @"\s+" + optPrefix + optDelim + tableName;

//            var invocation = fakeDbConnection.ShouldHaveInvoked(
//                    ii => ii.CommandText.Matches(verbandtablepattern, sqlRegexOpts)
//                       && ii.Parameters.Cast<FakeDbParameter>().Any(p => p.ParameterName == whereClauseProperty && p.Value.Equals(whereClausePropertyExpectedValue)),
//                    "Expected to Update {0} but found no matching update command.", tableName);

//            var commandText = invocation.CommandText;
//            invocation.CommandText.ShouldMatch(verbandtablepattern + optDelim + set, sqlRegexOpts);
//            var afterSet = new Regex(set + dotStar, sqlRegexOpts).Matches(commandText)[0].Value;
//            foreach (var field in fieldList)
//            {
//                var fieldName = field;
//                var fieldOrQuotedField = string.Format(@"({0}|\[{0}\]|""{0}"")", fieldName);
//                afterSet.ShouldMatch(
//                    string.Format(@"({0}|{1}){2}\s*=\s*\@{3}", set, comma, fieldOrQuotedField, field),
//                    sqlRegexOpts,
//                    "Expected to update field {0} but didn't see it", field);
//                invocation.Parameters.Cast<DbParameter>().SingleOrAssertFail(p => p.ParameterName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase));
//            }
//            ShouldHaveWhereClauseWithFieldEqualsExpected(invocation, whereClauseProperty, whereClausePropertyExpectedValue);
//            return invocation;
//        }

//        public static DbCommand ShouldHaveWhereClauseWithFieldEqualsExpected<T>(this DbCommand invocation, string columnName, T expectedValue)
//        {
//            invocation.CommandText.ShouldMatch(@"Where " + optPrefix + optDelim + columnName + optDelim + @"\s*\=\s*@" + columnName, sqlRegexOpts);
//            invocation.ShouldHaveParameter(columnName, expectedValue);
//            return invocation;
//        }

//        /// <summary>
//        /// Verifies that the <paramref name="dbCommand"/>'s <see cref="DbCommand.DbParameterCollection"/> includes a parameter 
//        /// with the given <paramref name="parameterName"/> and <paramref name="expectedValue"/>
//        /// </summary>
//        /// <returns><paramref name="dbCommand"/></returns>
//        public static DbCommand ShouldHaveParameter(this DbCommand dbCommand, string parameterName, object expectedValue)
//        {
//            var found = dbCommand.Parameters.Cast<DbParameter>()
//                .Any(
//                    p => p.ParameterName.Equals(parameterName, StringComparison.InvariantCultureIgnoreCase)
//                         && p.Value.Equals(expectedValue ?? DBNull.Value)
//                );
//            if (!found)
//            {
//                throw new AssertionException(
//                    string.Format(
//                    "\n\nExpected:\n\nclause: '{0} = @{0}'\n\nwith actual value: @{0}='{1}'\n\nBut was:\n\n{2}",
//                    parameterName, expectedValue, dbCommand.ToStringTextAndParams()));
//            }
//            return dbCommand;
//        }
//    }
//}
