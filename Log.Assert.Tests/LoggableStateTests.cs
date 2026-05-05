namespace Log.Assert.Tests;

[TestFixture]
public class LoggableStateTests
{
    [Test]
    public void ToLoggableState_Null_ReturnsNull()
    {
        object? x = null;
        NUnit.Framework.Assert.That(x.ToLoggableState(), Is.Null);
    }

    [Test]
    public void ToLoggableState_ValueType_ReturnsSelf()
    {
        var result = 42.ToLoggableState();
        NUnit.Framework.Assert.That(result, Is.EqualTo(42));
    }

    [Test]
    public void ToLoggableState_String_ReturnsToString()
    {
        var result = "hello".ToLoggableState();
        NUnit.Framework.Assert.That(result, Is.EqualTo("hello"));
    }

    [Test]
    public void ToLoggableState_ObjectWithMethod_CallsMethod()
    {
        var obj = new WithToLoggableState();
        var result = obj.ToLoggableState();
        NUnit.Framework.Assert.That(result, Is.EqualTo("custom-output"));
    }

    [Test]
    public void ToLoggableState_ObjectWithoutMethod_ReturnsToString()
    {
        var obj = new WithoutToLoggableState();
        var result = obj.ToLoggableState();
        NUnit.Framework.Assert.That(result, Is.EqualTo("WithoutToLoggableState.ToString"));
    }

    [Test]
    public void ToLoggableState_ILoggable_CallsToLoggableStateNotForLogging()
    {
        // ILoggable.ForLogging() is not called by ToLoggableState — the interface
        // is just a reminder. But if the class also has ToLoggableState(), that IS called.
        var obj = new LoggableWithMethod();
        var result = obj.ToLoggableState();
        NUnit.Framework.Assert.That(result, Is.EqualTo("from-method"));
    }

    class WithToLoggableState
    {
        public object ToLoggableState() => "custom-output";
    }

    class WithoutToLoggableState
    {
        public override string ToString() => "WithoutToLoggableState.ToString";
    }

    class LoggableWithMethod : ILoggable
    {
        public object? ForLogging() => "from-interface";
        public object ToLoggableState() => "from-method";
    }
}
