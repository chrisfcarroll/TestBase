PROMPT-next.md 2026-03-21
=========================

Proposals for future improvements to TestBase, based on the work done in PROMPT.md.

## 1. Drop legacy framework targets

TestBase currently multi-targets net8.0, net6.0, net5.0, net45, netstandard2.1, netstandard2.0, and netstandard1.6. Net5.0 is end-of-life. Net45 and the netstandard1.x targets add significant conditional compilation complexity. Consider:

1.1 Drop net5.0, netstandard1.6 and net45 targets from TestBase.csproj. Keep netstandard2.0 for broad compatibility, net6.0 as LTS baseline, and net8.0/net9.0 as current.
1.2 This would allow removing the `#if NET6_0_OR_GREATER` guards around Differ usage—all targets would get clean diff output.
1.3 Move net45/netstandard1.6 support to a maintenance branch if needed.

## 2. Use CallerArgumentExpression everywhere for actualExpression

Currently the rewritten Should methods pass `null` for `actualExpression` in EqualsByValueShoulds because `[CallerArgumentExpression]` cannot follow `params`. Consider:

2.1 Change method signatures to use a non-params overload pattern, e.g. `ShouldEqualByValue<T>(this T @this, T expected, string message = null)` with a separate overload for args.
2.2 Or use `CallerArgumentExpression` on the non-params overloads and have the params overloads call through.
2.3 This would give assertion failures the actual caller expression (e.g. `myObject.Name`) instead of `null` or `@this`.

## 3. Extend Differ coverage

The Differ handles objects, collections, strings, and primitives. Consider extending:

3.1 Dictionary diffing: show which keys are missing/extra and which values differ, rather than treating dictionaries as generic collections of KeyValuePair.
3.2 Nested collection diffing: when collections contain objects, show property-level diffs for mismatched elements rather than just "elements differ at [i]".
3.3 Add configurable max diff depth to prevent runaway output on deeply nested graphs.
3.4 Add a `DiffOptions.IgnoreOrder` flag for collection comparison.

## 4. Remove ExpressionToCodeLib dependency

With the shift away from expression-tree-based assertions, ExpressionToCodeLib is now only used in:
- `ShouldContain(Expression<Func<T,bool>>)` and `ShouldNotContain(Expression<Func<T,bool>>)` in IEnumerableShoulds for rendering the predicate as code in error messages.
- The `Assert.That(T, Expression<Func<T,bool>>)` overloads in Assert.cs/Assertion.cs.

Consider:
4.1 Replace ExpressionToCode rendering with CallerArgumentExpression to capture the predicate source text at compile time.
4.2 This would eliminate the ExpressionToCodeLib and FastExpressionCompiler transitive dependencies, reducing the package footprint significantly.

## 5. Structured test output / ITestOutputHelper integration

5.1 Add optional integration with xUnit's `ITestOutputHelper` and NUnit's `TestContext.WriteLine` so diff output is captured in test logs automatically.
5.2 Consider a `DiffFormatter` mode that outputs structured data (e.g. JSON) for CI/CD pipeline consumption.

## 6. Performance: avoid multiple enumeration

Several methods in IEnumerableShoulds enumerate the source multiple times (e.g. `ShouldContainEachOf` iterates `actual` once per subset item, `SingleOrAssertFail` enumerates then counts on failure). Consider:

6.1 Materialize to a list once at the start of methods that need multiple passes.
6.2 Add analyzer/roslyn warning suppressions where multiple enumeration is intentional.

## 7. Snapshot testing

7.1 Build on the Differ infrastructure to add snapshot testing: `actual.ShouldMatchSnapshot()` that serializes via TooString, compares to a stored `.snapshot` file, and on first run (or with an update flag) writes the snapshot.
7.2 This would be a natural extension of the Differ + TooString combination and would make TestBase competitive with Verify and similar libraries.

## 8. Source-generator based assertions

8.1 Investigate using C# source generators to produce assertion methods that capture variable names, types, and expressions at compile time—eliminating runtime reflection and expression tree compilation entirely.
8.2 This could dramatically improve assertion performance in large test suites while maintaining the fluent API.
