using System;

namespace TestBase
{
    public class IntegrationAttribute : Attribute
    {
        readonly DependsOn[] _dependencies;
        public IntegrationAttribute(params DependsOn[] dependencies){ _dependencies = dependencies; }

    }

    public enum DependsOn
    {
        Database
    }
}