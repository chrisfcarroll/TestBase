using System;
using NUnit.Framework;

namespace TestBase.Tests.ShouldsCorrectnessTests;

[TestFixture]
class WhenComparingDateTimesWithTolerance
{
    static readonly DateTime BaseDate = new DateTime(2025, 6, 15, 12, 30, 45, DateTimeKind.Utc);
    const long OneMillisecondNs = 1_000_000;
    const long OneSecondNs = 1_000_000_000;

    // ── ShouldEqualWithTolerance ──

    [Test]
    public void ShouldEqualWithTolerance__Should_Pass_when_within_tolerance()
    {
        var tolerance = OneMillisecondNs;
        var withinTolerance = BaseDate.AddTicks(tolerance / 100 - 1);
        BaseDate.ShouldEqualWithTolerance(withinTolerance, tolerance);
        withinTolerance.ShouldEqualWithTolerance(BaseDate, tolerance);
    }

    [Test]
    public void ShouldEqualWithTolerance__Should_Pass_when_exactly_equal()
    {
        BaseDate.ShouldEqualWithTolerance(BaseDate);
    }

    [Test]
    public void ShouldEqualWithTolerance__Should_Fail_when_outside_tolerance()
    {
        var tolerance = OneMillisecondNs;
        var outside = BaseDate.AddTicks(tolerance / 100 + 100);
        Should.Throw<Assertion>(() => BaseDate.ShouldEqualWithTolerance(outside, tolerance));
        Should.Throw<Assertion>(() => outside.ShouldEqualWithTolerance(BaseDate, tolerance));
    }

    [Test]
    public void ShouldEqualWithTolerance__Uses_default_1ms_tolerance()
    {
        var almostSame = BaseDate.AddTicks(9_999); // 0.9999ms in ticks
        BaseDate.ShouldEqualWithTolerance(almostSame);

        var tooFar = BaseDate.AddTicks(10_001); // 1.0001ms in ticks
        Should.Throw<Assertion>(() => BaseDate.ShouldEqualWithTolerance(tooFar));
    }

    // ── ShouldNotEqualWithMargin ──

    [Test]
    public void ShouldNotEqualWithMargin__Should_Pass_when_different_by_margin()
    {
        var tolerance = OneMillisecondNs;
        var farEnough = BaseDate.AddTicks(tolerance / 100 + 100);
        BaseDate.ShouldNotEqualWithMargin(farEnough, tolerance);
    }

    [Test]
    public void ShouldNotEqualWithMargin__Should_Fail_when_within_margin()
    {
        var tolerance = OneMillisecondNs;
        var tooClose = BaseDate.AddTicks(tolerance / 100 - 1);
        Should.Throw<Assertion>(() => BaseDate.ShouldNotEqualWithMargin(tooClose, tolerance));
    }

    // ── ShouldBeBetweenWithTolerance ──

    [Test]
    public void ShouldBeBetweenWithTolerance__Should_Pass_when_inside_range()
    {
        var left  = BaseDate;
        var right = BaseDate.AddSeconds(10);
        var middle = BaseDate.AddSeconds(5);
        middle.ShouldBeBetweenWithTolerance(left, right);
    }

    [Test]
    public void ShouldBeBetweenWithTolerance__Should_Pass_when_just_outside_range_but_within_tolerance()
    {
        var left  = BaseDate;
        var right = BaseDate.AddSeconds(10);
        var justBefore = BaseDate.AddTicks(-5_000); // 0.5ms before left
        justBefore.ShouldBeBetweenWithTolerance(left, right, OneMillisecondNs);
    }

    [Test]
    public void ShouldBeBetweenWithTolerance__Should_Fail_when_outside_range_and_tolerance()
    {
        var left  = BaseDate;
        var right = BaseDate.AddSeconds(10);
        var wayBefore = BaseDate.AddSeconds(-1);
        Should.Throw<Assertion>(() => wayBefore.ShouldBeBetweenWithTolerance(left, right, OneMillisecondNs));
    }

    // ── ShouldBeGreaterThanWithTolerance ──

    [Test]
    public void ShouldBeGreaterThanWithTolerance__Should_Pass_when_greater()
    {
        var later = BaseDate.AddSeconds(1);
        later.ShouldBeGreaterThanWithTolerance(BaseDate);
    }

    [Test]
    public void ShouldBeGreaterThanWithTolerance__Should_Pass_when_slightly_less_but_within_tolerance()
    {
        var slightlyBefore = BaseDate.AddTicks(-5_000); // 0.5ms before
        slightlyBefore.ShouldBeGreaterThanWithTolerance(BaseDate, OneMillisecondNs);
    }

    [Test]
    public void ShouldBeGreaterThanWithTolerance__Should_Fail_when_less_beyond_tolerance()
    {
        var wellBefore = BaseDate.AddSeconds(-1);
        Should.Throw<Assertion>(() => wellBefore.ShouldBeGreaterThanWithTolerance(BaseDate, OneMillisecondNs));
    }

    // ── ShouldBeGreaterThanOrEqualToWithTolerance ──

    [Test]
    public void ShouldBeGreaterThanOrEqualToWithTolerance__Should_Pass_when_equal()
    {
        BaseDate.ShouldBeGreaterThanOrEqualToWithTolerance(BaseDate);
    }

    [Test]
    public void ShouldBeGreaterThanOrEqualToWithTolerance__Should_Pass_when_slightly_less_but_within_tolerance()
    {
        var slightlyBefore = BaseDate.AddTicks(-5_000);
        slightlyBefore.ShouldBeGreaterThanOrEqualToWithTolerance(BaseDate, OneMillisecondNs);
    }

    [Test]
    public void ShouldBeGreaterThanOrEqualToWithTolerance__Should_Fail_when_less_beyond_tolerance()
    {
        var wellBefore = BaseDate.AddSeconds(-1);
        Should.Throw<Assertion>(() => wellBefore.ShouldBeGreaterThanOrEqualToWithTolerance(BaseDate, OneMillisecondNs));
    }

    // ── ShouldBeLessThanWithTolerance ──

    [Test]
    public void ShouldBeLessThanWithTolerance__Should_Pass_when_less()
    {
        var earlier = BaseDate.AddSeconds(-1);
        earlier.ShouldBeLessThanWithTolerance(BaseDate);
    }

    [Test]
    public void ShouldBeLessThanWithTolerance__Should_Pass_when_slightly_greater_but_within_tolerance()
    {
        var slightlyAfter = BaseDate.AddTicks(5_000);
        slightlyAfter.ShouldBeLessThanWithTolerance(BaseDate, OneMillisecondNs);
    }

    [Test]
    public void ShouldBeLessThanWithTolerance__Should_Fail_when_greater_beyond_tolerance()
    {
        var wellAfter = BaseDate.AddSeconds(1);
        Should.Throw<Assertion>(() => wellAfter.ShouldBeLessThanWithTolerance(BaseDate, OneMillisecondNs));
    }

    // ── ShouldBeLessThanOrEqualToWithTolerance ──

    [Test]
    public void ShouldBeLessThanOrEqualToWithTolerance__Should_Pass_when_equal()
    {
        BaseDate.ShouldBeLessThanOrEqualToWithTolerance(BaseDate);
    }

    [Test]
    public void ShouldBeLessThanOrEqualToWithTolerance__Should_Pass_when_slightly_greater_but_within_tolerance()
    {
        var slightlyAfter = BaseDate.AddTicks(5_000);
        slightlyAfter.ShouldBeLessThanOrEqualToWithTolerance(BaseDate, OneMillisecondNs);
    }

    [Test]
    public void ShouldBeLessThanOrEqualToWithTolerance__Should_Fail_when_greater_beyond_tolerance()
    {
        var wellAfter = BaseDate.AddSeconds(1);
        Should.Throw<Assertion>(() => wellAfter.ShouldBeLessThanOrEqualToWithTolerance(BaseDate, OneMillisecondNs));
    }
}

#if NET6_0_OR_GREATER
[TestFixture]
class WhenComparingTimeOnlyWithTolerance
{
    static readonly TimeOnly BaseTime = new TimeOnly(12, 30, 45);
    const long OneMillisecondNs = 1_000_000;

    // ── ShouldEqualWithTolerance ──

    [Test]
    public void ShouldEqualWithTolerance__Should_Pass_when_within_tolerance()
    {
        var tolerance = OneMillisecondNs;
        var withinTolerance = BaseTime.Add(TimeSpan.FromTicks(tolerance / 100 - 1));
        BaseTime.ShouldEqualWithTolerance(withinTolerance, tolerance);
        withinTolerance.ShouldEqualWithTolerance(BaseTime, tolerance);
    }

    [Test]
    public void ShouldEqualWithTolerance__Should_Pass_when_exactly_equal()
    {
        BaseTime.ShouldEqualWithTolerance(BaseTime);
    }

    [Test]
    public void ShouldEqualWithTolerance__Should_Fail_when_outside_tolerance()
    {
        var tolerance = OneMillisecondNs;
        var outside = BaseTime.Add(TimeSpan.FromTicks(tolerance / 100 + 100));
        Should.Throw<Assertion>(() => BaseTime.ShouldEqualWithTolerance(outside, tolerance));
        Should.Throw<Assertion>(() => outside.ShouldEqualWithTolerance(BaseTime, tolerance));
    }

    [Test]
    public void ShouldEqualWithTolerance__Uses_default_1ms_tolerance()
    {
        var almostSame = BaseTime.Add(TimeSpan.FromTicks(9_999));
        BaseTime.ShouldEqualWithTolerance(almostSame);

        var tooFar = BaseTime.Add(TimeSpan.FromTicks(10_001));
        Should.Throw<Assertion>(() => BaseTime.ShouldEqualWithTolerance(tooFar));
    }

    // ── ShouldNotEqualWithMargin ──

    [Test]
    public void ShouldNotEqualWithMargin__Should_Pass_when_different_by_margin()
    {
        var tolerance = OneMillisecondNs;
        var farEnough = BaseTime.Add(TimeSpan.FromTicks(tolerance / 100 + 100));
        BaseTime.ShouldNotEqualWithMargin(farEnough, tolerance);
    }

    [Test]
    public void ShouldNotEqualWithMargin__Should_Fail_when_within_margin()
    {
        var tolerance = OneMillisecondNs;
        var tooClose = BaseTime.Add(TimeSpan.FromTicks(tolerance / 100 - 1));
        Should.Throw<Assertion>(() => BaseTime.ShouldNotEqualWithMargin(tooClose, tolerance));
    }

    // ── ShouldBeBetweenWithTolerance ──

    [Test]
    public void ShouldBeBetweenWithTolerance__Should_Pass_when_inside_range()
    {
        var left  = BaseTime;
        var right = BaseTime.Add(TimeSpan.FromSeconds(10));
        var middle = BaseTime.Add(TimeSpan.FromSeconds(5));
        middle.ShouldBeBetweenWithTolerance(left, right);
    }

    [Test]
    public void ShouldBeBetweenWithTolerance__Should_Pass_when_just_outside_range_but_within_tolerance()
    {
        var left  = BaseTime;
        var right = BaseTime.Add(TimeSpan.FromSeconds(10));
        var justBefore = BaseTime.Add(TimeSpan.FromTicks(-5_000));
        justBefore.ShouldBeBetweenWithTolerance(left, right, OneMillisecondNs);
    }

    [Test]
    public void ShouldBeBetweenWithTolerance__Should_Fail_when_outside_range_and_tolerance()
    {
        var left  = BaseTime;
        var right = BaseTime.Add(TimeSpan.FromSeconds(10));
        var wayBefore = BaseTime.Add(TimeSpan.FromSeconds(-1));
        Should.Throw<Assertion>(() => wayBefore.ShouldBeBetweenWithTolerance(left, right, OneMillisecondNs));
    }

    // ── ShouldBeGreaterThanWithTolerance ──

    [Test]
    public void ShouldBeGreaterThanWithTolerance__Should_Pass_when_greater()
    {
        var later = BaseTime.Add(TimeSpan.FromSeconds(1));
        later.ShouldBeGreaterThanWithTolerance(BaseTime);
    }

    [Test]
    public void ShouldBeGreaterThanWithTolerance__Should_Pass_when_slightly_less_but_within_tolerance()
    {
        var slightlyBefore = BaseTime.Add(TimeSpan.FromTicks(-5_000));
        slightlyBefore.ShouldBeGreaterThanWithTolerance(BaseTime, OneMillisecondNs);
    }

    [Test]
    public void ShouldBeGreaterThanWithTolerance__Should_Fail_when_less_beyond_tolerance()
    {
        var wellBefore = BaseTime.Add(TimeSpan.FromSeconds(-1));
        Should.Throw<Assertion>(() => wellBefore.ShouldBeGreaterThanWithTolerance(BaseTime, OneMillisecondNs));
    }

    // ── ShouldBeGreaterThanOrEqualToWithTolerance ──

    [Test]
    public void ShouldBeGreaterThanOrEqualToWithTolerance__Should_Pass_when_equal()
    {
        BaseTime.ShouldBeGreaterThanOrEqualToWithTolerance(BaseTime);
    }

    [Test]
    public void ShouldBeGreaterThanOrEqualToWithTolerance__Should_Pass_when_slightly_less_but_within_tolerance()
    {
        var slightlyBefore = BaseTime.Add(TimeSpan.FromTicks(-5_000));
        slightlyBefore.ShouldBeGreaterThanOrEqualToWithTolerance(BaseTime, OneMillisecondNs);
    }

    [Test]
    public void ShouldBeGreaterThanOrEqualToWithTolerance__Should_Fail_when_less_beyond_tolerance()
    {
        var wellBefore = BaseTime.Add(TimeSpan.FromSeconds(-1));
        Should.Throw<Assertion>(() => wellBefore.ShouldBeGreaterThanOrEqualToWithTolerance(BaseTime, OneMillisecondNs));
    }

    // ── ShouldBeLessThanWithTolerance ──

    [Test]
    public void ShouldBeLessThanWithTolerance__Should_Pass_when_less()
    {
        var earlier = BaseTime.Add(TimeSpan.FromSeconds(-1));
        earlier.ShouldBeLessThanWithTolerance(BaseTime);
    }

    [Test]
    public void ShouldBeLessThanWithTolerance__Should_Pass_when_slightly_greater_but_within_tolerance()
    {
        var slightlyAfter = BaseTime.Add(TimeSpan.FromTicks(5_000));
        slightlyAfter.ShouldBeLessThanWithTolerance(BaseTime, OneMillisecondNs);
    }

    [Test]
    public void ShouldBeLessThanWithTolerance__Should_Fail_when_greater_beyond_tolerance()
    {
        var wellAfter = BaseTime.Add(TimeSpan.FromSeconds(1));
        Should.Throw<Assertion>(() => wellAfter.ShouldBeLessThanWithTolerance(BaseTime, OneMillisecondNs));
    }

    // ── ShouldBeLessThanOrEqualToWithTolerance ──

    [Test]
    public void ShouldBeLessThanOrEqualToWithTolerance__Should_Pass_when_equal()
    {
        BaseTime.ShouldBeLessThanOrEqualToWithTolerance(BaseTime);
    }

    [Test]
    public void ShouldBeLessThanOrEqualToWithTolerance__Should_Pass_when_slightly_greater_but_within_tolerance()
    {
        var slightlyAfter = BaseTime.Add(TimeSpan.FromTicks(5_000));
        slightlyAfter.ShouldBeLessThanOrEqualToWithTolerance(BaseTime, OneMillisecondNs);
    }

    [Test]
    public void ShouldBeLessThanOrEqualToWithTolerance__Should_Fail_when_greater_beyond_tolerance()
    {
        var wellAfter = BaseTime.Add(TimeSpan.FromSeconds(1));
        Should.Throw<Assertion>(() => wellAfter.ShouldBeLessThanOrEqualToWithTolerance(BaseTime, OneMillisecondNs));
    }
}
#endif
