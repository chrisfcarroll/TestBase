## Developer Experience Report

### TooString

1. **`ToArgumentExpression()` still exists alongside `ToCallerArgumentString()`** — both do the same thing. If the intent is to consolidate on `ToCallerArgumentString()`, the old method should be marked `[Obsolete]` or removed. Currently both are public and discoverable.

2. **`TooString(StringifyAs style)` vs the named methods** — the named methods (`ToCSharpString`, `ToJson`) are cleaner, but `TooString(StringifyAs.CSharp)` still works and returns the same output. However, **`TooString(StringifyAs.CSharp)` and `ToCSharpString()` have the same default for `writeIndented`** (true), so they're consistent. The duplication may confuse new users about which to use.

3. **No `ToDebugViewString()` convenience method** — CSharp, Json, and STJson all have dedicated methods, but DebugView requires `TooString(maxDepth: 3, style: StringifyAs.DebugView, writeIndented: false)` or using TooStringOptions. This is inconsistent with the pattern.

4. **`TooStringOptions.Default` has `WriteIndented = false`** but `TooString()`, `ToJson()`, and `ToCSharpString()` now default to `writeIndented = true`. This means `value.TooString()` produces different output from `value.TooString(TooStringOptions.Default)`. This is surprising and could confuse users.

5. **`ToJson()` uses reflection (JsonStringifier), not System.Text.Json** — the name `ToJson` suggests standard JSON serialization, but it actually uses a custom reflection-based stringifier. `ToSTJson()` is the one that uses System.Text.Json. The naming may confuse users who expect `ToJson()` to produce standard JSON output. The acronym "STJ" is not widely known outside this project.

6. **`maxLength` vs `maxEnumerationLength`** — the `TooString(int maxDepth, int maxLength, ...)` overload uses `maxLength`, but `ToCSharpString()` and `ToJson()` use `maxEnumerationLength`. The inconsistency in parameter naming is confusing.

### TestBase

1. **Assertion output with `maxDepth=1, maxLength=3` may be too terse** — for complex objects, `maxDepth=1` truncates all nested objects to type names on the first level. When an assertion fails, the user sees very little of the actual value, which can make debugging harder. Consider whether `maxDepth=2` would be a better default.

2. **Assertion `Actual` string is now indented** — because `TooString(maxDepth:1, maxLength:3)` inherits `writeIndented=true` from the default. The multi-line assertion output may be harder to scan in test runner output, where single-line is often more readable. The assertion output format (`"Actual :\n---\n..."`) already adds its own newlines, so indented TooString output creates doubly-nested formatting.

3. **The `PreferredToStringMethod` array still tries `TooString` first, then falls back through ExpressionToCode and Newtonsoft** — with `maxDepth=1`, TooString will almost always succeed (it's best-effort), so the fallback methods are rarely exercised. This is probably fine but worth noting.

4. **`EqualsByValueShoulds.AssertEqualByDiff` uses `actual?.TooString(maxDepth: 1, maxLength: 3)`** — the diff output from `Differ` is the main diagnostic, and `actualStr` is only shown in the assertion message. With maxDepth=1, this is fine since the diff provides the detail. But if users rely on the `Actual` field of the assertion for debugging, it may be too abbreviated.
