using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ActivateAnything;
using TestBase.AdoNet.FakeDb;
using TestBase.HttpClient.Fake;

namespace TestBase.FixtureBase
{
    /// <summary>
    ///     An anchor class suitable as a testfixture, whose subclasses can be decorated
    ///     with <see cref="IActivateAnythingRule" />s to construct a UnitUnderTest.
    /// </summary>
    /// <typeparam name="T">The <see cref="System.Type" /> of the <see cref="UnitUnderTest" /></typeparam>
    public class FixtureBase
    {
        readonly object aalocker=new object();
        bool activateIsStale = true;
        AnythingActivator activator;

        public ObservableCollection<object> Instances { get; } = new ObservableCollection<object>();
        
        public AnythingActivator Activator
        {
            get
            {
                if (activateIsStale || activator==null)lock(aalocker)if (activateIsStale || activator==null)
                {
                    activator = AnythingActivator.FromDefaultAndSearchAnchorRulesAnd(this, new ActivateInstances(Instances));
                    activateIsStale = false;
                }
                return activator;
            }
        }
        protected FixtureBase() { Instances.CollectionChanged += (sender, args) => activateIsStale = true; }
    }

    public class FixtureBaseFor<T> : FixtureBase
    {
        readonly object uutlocker=new object();
        bool uutIsStale = true;
        T uut;

        public T UnitUnderTest
        {
            get
            {
                if (uutIsStale || uut==null)lock(uutlocker)if (uutIsStale || uut==null)
                {
                    uut = Activator.New<T>();
                    uutIsStale = false;
                }
                return uut;
            }
        }
        protected FixtureBaseFor() { Instances.CollectionChanged += (sender, args) => uutIsStale = true; }
    }
    
    /// <inheritdoc cref="FixtureBase"/>
    public class FixtureBaseWithHttpFor<T> : FixtureBaseFor<T>
    {
        public readonly FakeHttpClient HttpClient = new FakeHttpClient();
        
        protected FixtureBaseWithHttpFor():base(){Instances.Add(HttpClient);}
    }
    
    /// <inheritdoc cref="FixtureBase"/>
    public class FixtureBaseWithDbFor<T> : FixtureBaseFor<T>
    {
        public readonly FakeDbConnection FakeDbConnection = new FakeDbConnection();
        
        protected FixtureBaseWithDbFor(){Instances.Add(FakeDbConnection);}
    }
    
    /// <inheritdoc cref="FixtureBase"/>
    public class FixtureBaseWithDbAndHttpFor<T> :FixtureBaseFor<T>
    {
        public readonly FakeDbConnection Db = new FakeDbConnection();
        public readonly FakeHttpClient HttpClient = new FakeHttpClient();
        
        protected FixtureBaseWithDbAndHttpFor(){Instances.Add(Db);Instances.Add(HttpClient);}
    }
}
