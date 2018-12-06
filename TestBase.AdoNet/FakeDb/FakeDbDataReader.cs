using System;

namespace TestBase.AdoNet
{
    public class FakeDbResultSet
    {
        public object[,] Data;
        public MetaData[] metaData;

        public int recordsAffected;

        public struct MetaData
        {
            public MetaData(string name, Type type) : this()
            {
                Name    = name;
                Type    = type;
                MaxSize = 0;
            }

            public string Name    { get; }
            public Type   Type    { get; }
            public int    MaxSize { get; }

            public override bool Equals(object obj) { return obj is MetaData && AreEqual(this, (MetaData) obj); }

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    var hash = 17;
                    // Suitable nullity checks etc, of course :)
                    hash = hash * 23 + (Name ?? "").GetHashCode();
                    hash = hash * 23 + (Type ?? typeof(object)).GetHashCode();
                    hash = hash * 23 + MaxSize.GetHashCode();
                    return hash;
                }
            }

            public static bool operator ==(MetaData x, MetaData y) { return AreEqual(x, y); }

            static bool AreEqual(MetaData x, MetaData y)
            {
                return x.Name    == y.Name
                    && x.Type    == y.Type
                    && x.MaxSize == y.MaxSize;
            }

            public static bool operator !=(MetaData x, MetaData y) { return !(x == y); }

            public override string ToString()
            {
                return string.Format("{{ Name:{0}, Type:{1}, MaxSize:{2} }}", Name, Type, MaxSize);
            }
        }
    }
}
