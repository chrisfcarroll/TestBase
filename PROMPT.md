PROMPT.md 2026-04-19
=========================

### TooString

1. The indentation for writeIndented = true is wrong for CSharpString and DebugView. It is correct for Json. Fix the indentation in all cases, and make sure that the tests confirm it is correct.

2. **`ToArgumentExpression()` still exists alongside `ToCallerArgumentString()`** — both do the same thing. Let's go back to ToArgumentExpression(). Remove or correct any references to ToCallerArgumentString() in the code or the docs.

3. **`TooStringOptions.Default` has `WriteIndented = false`** but `TooString()`, `ToJson()`, and `ToCSharpString()` now default to `writeIndented = true`. This means `value.TooString()` produces different output from `value.TooString(TooStringOptions.Default)`. This is surprising and could confuse users. Correct the default to be WriteIndented = true. Make sure that the tests still pass.
