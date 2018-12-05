using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace TestBase.AdoNet
{
    /// <summary>
    /// Maintains a list of Commands + parameters sent to the fake db in unit testing
    /// </summary>
    public class DbCommandInvocationList : List<DbCommandInvocation>
    {
        public void Add(DbCommand cmd, DbParameterCollection parameters, CommandBehavior behavior)
        {
            Add(new DbCommandInvocation(cmd, parameters, behavior));
        }
    }

    /// <summary>
    /// Encapsulates a DbCommand together with the Parameters with which it was invoked.
    /// </summary>
    public class DbCommandInvocation : Tuple<DbCommand, DbParameterCollection>
    {
        
        public DbCommandInvocation(DbCommand command, DbParameterCollection parameterCollection, CommandBehavior behavior) : base(command,parameterCollection)
        {
            Behavior = behavior;
            InvokedAtTime=DateTime.Now;
        }
        public DbCommand             Command => Item1;
        public DbParameterCollection Parameters => Item2;
        public DateTime InvokedAtTime { get; }
        public CommandBehavior Behavior { get; }
        public DateTime? CancelledAtTime { get; set; }
        public bool WasCancelled => CancelledAtTime != null;
    }
}