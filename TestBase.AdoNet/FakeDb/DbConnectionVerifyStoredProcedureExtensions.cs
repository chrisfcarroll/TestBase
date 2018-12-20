using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace TestBase.AdoNet
{
    public static class FakeDbVerifyStoredProcedureExtensions
    {
        /// <summary>
        ///     Verifies that a command was invoked on <paramref name="fakeDbConnection" /> which called
        ///     a stored procedure called <paramref name="procedureName"/>
        ///     <paramref name="procedureName" />
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveExecutedStoredProcedure(
            this FakeDbConnection             fakeDbConnection,
            string  procedureName,
            string                            message = null,
            params object[]                   args)
        {
            Expression<Func<DbCommand, bool>> predicateCalledSproc = 
                i=>i.CommandType==CommandType.StoredProcedure && i.CommandText==procedureName;
            message = message == null
                ? "Expected to execute stored procedure " + procedureName
                : string.Format(message, args);
            return fakeDbConnection.ShouldHaveInvoked(predicateCalledSproc, message);
        }

        /// <summary>
        ///     Verifies that a command was invoked on <paramref name="fakeDbConnection" /> which called
        ///     a stored procedure called <paramref name="procedureName"/> and which further satisfies
        ///     <paramref name="predicate"/>
        ///     satisfying <paramref name="predicate"/>
        ///     <paramref name="procedureName" />
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveExecutedStoredProcedure(
            this FakeDbConnection             fakeDbConnection,
            string  procedureName,
            Expression<Func<DbCommand,bool>> predicate,
            string                            message = null,
            params object[]                   args)
        {
            Expression<Func<DbCommand, bool>> predicateCalledSprocWithParam = 
                i=>i.CommandType==CommandType.StoredProcedure && i.CommandText==procedureName
                                                              && predicate.Compile()(i);

            message = message == null
                ? "Expected to execute stored procedure " + procedureName + " and satisfy " + predicate.ToCodeString()
                : string.Format(message, args);

            return fakeDbConnection.ShouldHaveInvoked(predicateCalledSprocWithParam, message);
        }

        /// <summary>
        ///     Verifies that a command was invoked on <paramref name="fakeDbConnection" /> which called
        ///     a stored procedure called <paramref name="procedureName"/> and had at least one parameter
        ///     satisfying <paramref name="predicate"/>
        ///     <paramref name="procedureName" />
        /// </summary>
        /// <returns>The matching command</returns>
        public static DbCommand ShouldHaveExecutedStoredProcedureWithParameter(
            this FakeDbConnection             fakeDbConnection,
            string  procedureName,
            Expression<Func<DbParameter,bool>> predicate,
            string                            message = null,
            params object[]                   args)
        {
            Expression<Func<DbCommand, bool>> predicateCalledSprocWithParam = 
                i=>i.CommandType==CommandType.StoredProcedure && i.CommandText==procedureName
                  && i.Parameters.Cast<DbParameter>() .Any(p=> predicate.Compile()(p));

            message = message == null
                ? "Expected to execute stored procedure " + procedureName 
                + " with parameter satisfying " + predicate.ToCodeString()
                : string.Format(message, args);

            return fakeDbConnection.ShouldHaveInvoked(predicateCalledSprocWithParam, message);
        }

    }

}
