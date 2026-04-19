## Developer Experience Report

### TooString

1. **`ToArgumentExpression()` still exists alongside `ToCallerArgumentString()`** — both do the same thing. Let's go back to ToArgumentExpression(). Remove or correct any references to ToCallerArgumentString() in the code or the docs.

2. **`TooStringOptions.Default` has `WriteIndented = false`** but `TooString()`, `ToJson()`, and `ToCSharpString()` now default to `writeIndented = true`. This means `value.TooString()` produces different output from `value.TooString(TooStringOptions.Default)`. This is surprising and could confuse users. Correct the default to be WriteIndented = true.


### TestBase

1. **The `PreferredToStringMethod` array still tries `TooString` first, then falls back through ExpressionToCode and Newtonsoft** — with `maxDepth=1`, TooString will almost always succeed (it's best-effort), so the fallback methods are rarely exercised. This is probably fine but worth noting.

4. **`EqualsByValueShoulds.AssertEqualByDiff` uses `actual?.TooString(maxDepth: 1, maxLength: 3)`** — the diff output from `Differ` is the main diagnostic, and `actualStr` is only shown in the assertion message. With maxDepth=1, this is fine since the diff provides the detail. But if users rely on the `Actual` field of the assertion for debugging, it may be too abbreviated.
