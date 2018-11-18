using ActivateAnything;

namespace TestBase.FixtureBase
{
    /// <summary>
    ///     An anchor class suitable as a testfixture, whose subclasses can be decorated
    ///     with <see cref="IActivateAnythingRule" />s to construct a UnitUnderTest.
    /// </summary>
    /// <typeparam name="T">The <see cref="System.Type" /> of the <see cref="UnitUnderTest" /></typeparam>
    public class TestBaseFor<T>
    {
        protected internal T UnitUnderTest;
        protected readonly AnythingActivator Activator;

        protected TestBaseFor()
        {
            Activator = new AnythingActivator(this);
            UnitUnderTest = Activator.New<T>();
        }

        protected TestBaseFor(params object[] useInstances)
        {
            var instances= new ActivateInstances(useInstances);
            Activator= AnythingActivator.FromDefaultAndSearchAnchorRulesAnd(this, instances);
        }
    }
    public class FixtureBase
    {
        
    }
    
    public class FixtureBaseWithHttpFor<T> :FixtureBase
    {
        
    }
}
