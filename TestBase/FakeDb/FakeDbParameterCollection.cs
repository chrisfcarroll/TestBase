using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace TestBase.FakeDb
{
    public class FakeDbParameterCollection : DbParameterCollection
    {
        private object syncRoot = new object();
        private List<DbParameter> parameters= new List<DbParameter>();

        public override int Add(object value)
        {
            parameters.Add(AsDbParameterOrThrow(value));
            return parameters.Count;
        }

        public override bool Contains(object value)
        {
            return parameters.Contains(AsDbParameterOrThrow(value));
        }

        public override void Clear()
        {
            parameters.Clear();
        }

        public override int IndexOf(object value)
        {
            return parameters.IndexOf(AsDbParameterOrThrow(value));
        }

        public override void Insert(int index, object value)
        {
            parameters.Insert(index, AsDbParameterOrThrow(value));
        }

        public override void Remove(object value)
        {
            parameters.Remove(AsDbParameterOrThrow(value));
        }

        public override void RemoveAt(int index)
        {
            parameters.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            var toRemove=parameters.First(x => x.ParameterName == parameterName);
            parameters.Remove(toRemove);
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            parameters[index]=value;
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            parameters[IndexOf(parameterName)] = value;
        }

        public override int Count
        {
            get { return parameters.Count; }
        }

        public override object SyncRoot
        {
            get { return syncRoot; }
        }

        public override bool IsFixedSize
        {
            get { return false; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSynchronized
        {
            get { return false; }
        }

        public override int IndexOf(string parameterName)
        {
            return parameters.Where(x => x.ParameterName == parameterName).Select((x, i) => i).First();
        }

        public override IEnumerator GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        protected override DbParameter GetParameter(int index)
        {
            return parameters[index];
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            return parameters.First(x => x.ParameterName.ToLower() == parameterName.ToLower());
        }

        public override bool Contains(string value)
        {
            return parameters.Any(x => value == (string) x.Value);
        }

        public override void CopyTo(Array array, int index)
        {
            if( array as DbParameter[] == null) throw new ArgumentException("array must be a DbParameter array","array");
            parameters.CopyTo((DbParameter[]) array, index);
        }

        public override void AddRange(Array values)
        {
            throw new NotImplementedException();
        }

        private static DbParameter AsDbParameterOrThrow(object value)
        {
            var param = value as DbParameter;
            if (param == null)
                throw new ArgumentNullException("value",
                                                string.Format(
                                                    "Expected a DbParameter to be added to DbParameter collection, not {0}",
                                                    value));
            return param;
        }
    }
}