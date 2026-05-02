using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TestBase.Tests.ShouldsCorrectnessTests;

[TestFixture]
public class ShouldThrowAsyncTests
{
    [Test]
    public async Task Throw_TaskAction_CatchesExpectedException()
    {
        // Arrange
        Task<Action> taskAction = Task.FromResult<Action>(() => throw new InvalidOperationException("Test"));

        // Act
        var exception = await Should.ThrowAsync<InvalidOperationException>(taskAction);

        // Assert
        exception.ShouldBeOfType<InvalidOperationException>();
        exception.Message.ShouldBe("Test");
    }

    [Test]
    public void Throw_TaskAction_ThrowsIfNoException()
    {
        // Arrange
        Task<Action> taskAction = Task.FromResult<Action>(() => { });

        // Act & Assert
        NUnit.Framework.Assert.ThrowsAsync<NUnit.Framework.AssertionException>(async () => await Should.ThrowAsync<InvalidOperationException>(taskAction));
    }

    [Test]
    public void Throw_TaskAction_ThrowsIfDifferentException()
    {
        // Arrange
        Task<Action> taskAction = Task.FromResult<Action>(() => throw new ArgumentException("Test"));

        // Act & Assert
        NUnit.Framework.Assert.ThrowsAsync<NUnit.Framework.AssertionException>(async () => await Should.ThrowAsync<InvalidOperationException>(taskAction));
    }

    [Test]
    public async Task Throw_TaskPredicate_CatchesExpectedException()
    {
        // Arrange
        var actual = "test";
        Func<string, bool> func = s => s == "test" ? throw new InvalidOperationException("Test") : true;
        Expression<Func<string, bool>> predicate = s => func(s);
        Task<Expression<Func<string, bool>>> taskPredicate = Task.FromResult(predicate);

        // Act
        var result = await Should.ThrowAsync<string, InvalidOperationException>(actual, taskPredicate, null, null);

        // Assert
        result.ShouldBe(actual);
    }

    [Test]
    public void Throw_TaskPredicate_ThrowsIfNoException()
    {
        // Arrange
        var actual = "test";
        Task<Expression<Func<string, bool>>> taskPredicate = Task.FromResult<Expression<Func<string, bool>>>(s => s == "test");

        // Act & Assert
        NUnit.Framework.Assert.ThrowsAsync<NUnit.Framework.AssertionException>(async () => await Should.ThrowAsync<string, InvalidOperationException>(actual, taskPredicate, null, null));
    }

    [Test]
    public void Throw_TaskPredicate_ThrowsIfDifferentException()
    {
        // Arrange
        var actual = "test";
        Func<string, bool> func = s => s == "test" ? throw new ArgumentException("Test") : true;
        Expression<Func<string, bool>> predicate = s => func(s);
        Task<Expression<Func<string, bool>>> taskPredicate = Task.FromResult(predicate);

        // Act & Assert
        NUnit.Framework.Assert.ThrowsAsync<NUnit.Framework.AssertionException>(async () => await Should.ThrowAsync<string, InvalidOperationException>(actual, taskPredicate, null, null));
    }

    // WIP
    // [Test]
    // public async Task NotThrowAsync_TaskAction_CatchesExpectedException()
    // {
    //     async Task ThrowWhenAwaited()
    //     {
    //         await Task.Yield();
    //         throw new InvalidOperationException("Test");
    //     }
    //
    //     // Act
    //     try
    //     {
    //         await Should.NotThrowAsync(ThrowWhenAwaited);
    //
    //         Assert.Fail("Should have thrown a ShouldNotThrowException but didn't");
    //     }
    //     catch (ShouldNotThrowException ex)
    //     {
    //         ex.Message.ShouldContain("Test","Threw a ShouldNotThrowException, but it should have contained the thrown InvalidOperationException");
    //     }
    // }

}
