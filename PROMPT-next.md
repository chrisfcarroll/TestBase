<div hidden>

PROMPT-next.md
==============

Ignore this file until PROMPT.md is empty.

## 4. Structured test output / ITestOutputHelper integration

5.1 Add optional integration with xUnit's `ITestOutputHelper` and NUnit's `TestContext.WriteLine` so diff output is captured in test logs automatically.
5.2 Consider a `DiffFormatter` mode that outputs structured data (e.g. JSON) for CI/CD pipeline consumption.

## 5. Performance: avoid multiple enumeration

Several methods in IEnumerableShoulds enumerate the source multiple times (e.g. `ShouldContainEachOf` iterates `actual` once per subset item, `SingleOrAssertFail` enumerates then counts on failure). Consider:

6.1 Materialize to a list once at the start of methods that need multiple passes.
6.2 Add analyzer/roslyn warning suppressions where multiple enumeration is intentional.

## 6. Snapshot testing

7.1 Build on the Differ infrastructure to add snapshot testing: `actual.ShouldMatchSnapshot()` that serializes via TooString, compares to a stored `.snapshot` file, and on first run (or with an update flag) writes the snapshot.
7.2 This would be a natural extension of the Differ + TooString combination and would make TestBase competitive with Verify and similar libraries.

## 7. Source-generator based assertions

8.1 Investigate using C# source generators to produce assertion methods that capture variable names, types, and expressions at compile time—eliminating runtime reflection and expression tree compilation entirely.
8.2 This could dramatically improve assertion performance in large test suites while maintaining the fluent API.
<div>