using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TestBase.Tests.ShouldsCorrectnessTests;

[TestFixture]
public class ShouldThrowAsyncTests
{
    // -- ThrowAsync(Func<Task>) --

    [Test]
    public async Task ThrowAsync_FuncTask_ReturnsException()
    {
        var ex = await Should.ThrowAsync(async () =>
        {
            await Task.Yield();
            throw new InvalidOperationException("boom");
        });

        ex.ShouldBeOfType<InvalidOperationException>();
        ex.Message.ShouldBe("boom");
    }

    [Test]
    public void ThrowAsync_FuncTask_ThrowsWhenNoException()
    {
        NUnit.Framework.Assert.ThrowsAsync<NUnit.Framework.AssertionException>(
            async () => await Should.ThrowAsync(async () => await Task.FromResult(0)));
    }

    // -- ThrowAsync(Task) --

    [Test]
    public async Task ThrowAsync_Task_ReturnsException()
    {
        static async Task Faulted()
        {
            await Task.Yield();
            throw new ArgumentException("bad arg");
        }

        var ex = await Should.ThrowAsync(Faulted());

        ex.ShouldBeOfType<ArgumentException>();
        ex.Message.ShouldBe("bad arg");
    }

    [Test]
    public void ThrowAsync_Task_ThrowsWhenNoException()
    {
        NUnit.Framework.Assert.ThrowsAsync<NUnit.Framework.AssertionException>(
            async () => await Should.ThrowAsync(Task.FromResult(0)));
    }

    // -- Throw(Func<Task>) blocking --

    [Test]
    public void Throw_FuncTask_ReturnsException()
    {
        var ex = Should.Throw(async () =>
        {
            await Task.Yield();
            throw new InvalidOperationException("sync boom");
        });

        ex.ShouldBeOfType<InvalidOperationException>();
        ex.Message.ShouldBe("sync boom");
    }

    [Test]
    public void Throw_FuncTask_ThrowsWhenNoException()
    {
        NUnit.Framework.Assert.Throws<NUnit.Framework.AssertionException>(
            () => Should.Throw(async () => await Task.FromResult(0)));
    }

    // -- Throw(Task) blocking --

    [Test]
    public void Throw_Task_ReturnsException()
    {
        static async Task Faulted()
        {
            await Task.Yield();
            throw new ArgumentException("sync bad arg");
        }

        var ex = Should.Throw(Faulted());

        ex.ShouldBeOfType<ArgumentException>();
        ex.Message.ShouldBe("sync bad arg");
    }

    [Test]
    public void Throw_Task_ThrowsWhenNoException()
    {
        NUnit.Framework.Assert.Throws<NUnit.Framework.AssertionException>(
            () => Should.Throw(Task.FromResult(0)));
    }

    // -- NotThrowAsync(Func<Task>) --

    [Test]
    public async Task NotThrowAsync_FuncTask_PassesWhenNoException()
    {
        await Should.NotThrowAsync(async () => await Task.FromResult(0));
    }

    [Test]
    public async Task NotThrowAsync_FuncTask_ThrowsShouldNotThrowOnException()
    {
        try
        {
            await Should.NotThrowAsync(async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("oops");
            });
            NUnit.Framework.Assert.Fail("Expected ShouldNotThrowException");
        }
        catch (Exception ex)
        {
            var inner = ex is ShouldNotThrowException ? ex : ex.InnerException;
            inner.ShouldBeAssignableTo<ShouldNotThrowException>();
            inner.Message.ShouldContain("oops");
        }
    }

    // -- NotThrowAsync(Task) --

    [Test]
    public async Task NotThrowAsync_Task_PassesWhenNoException()
    {
        await Should.NotThrowAsync(Task.FromResult(0));
    }

    [Test]
    public async Task NotThrowAsync_Task_ThrowsShouldNotThrowOnException()
    {
        static async Task Faulted()
        {
            await Task.Yield();
            throw new InvalidOperationException("task oops");
        }

        try
        {
            await Should.NotThrowAsync(Faulted());
            NUnit.Framework.Assert.Fail("Expected ShouldNotThrowException");
        }
        catch (Exception ex)
        {
            var inner = ex is ShouldNotThrowException ? ex : ex.InnerException;
            inner.ShouldBeAssignableTo<ShouldNotThrowException>();
            inner.Message.ShouldContain("task oops");
        }
    }
}
