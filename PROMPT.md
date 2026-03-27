PROMPT.md 2026-03-27
=========================

Proposals for future improvements to TestBase, based on the work done in PROMPT.md.

## 1. Extend Differ test cases [DONE]

TestBase.Comparer has test cases for many more edge cases than Differ has. Apply
those same test cases to Differ and prove it works for records, advanced numerics
and other edge cases

commit type = testcases:

## 2. Make the output for collections more concise. [DONE]

Then, although it is not the usual thing for tests, I want to have 'tests' that
demonstrate sample output to TestContext.Out. The point is to demonstrate just
how excellently concise the output is.

commit type = tests:

Having done that demo, then imagine how the output could be more concise, to
help a viewer see at a glance just how concise the output is. Now add
assertions to the demos, to assert that the output for collections is as
concise as we would like it to be, and make the assertions pass. The trick
will be to pick out what to display.

commit type = feature scope= ... comment = More concise collection diffs.

**Done:** DifferOutputConcisenessTests added. Output flattened to single lines:
- `[1].Name: strings differ at index 0 Expected = "B", Actual = "X"`
- `[2]: Expected = missing, Actual = 3`

## 3. Extend Differ coverage [DONE]

The Differ handles objects, collections, strings, and primitives. Consider extending:

3.1 Dictionary diffing: show which keys are missing/extra and which values differ, rather than treating dictionaries as generic collections of KeyValuePair. [DONE]

**Done:** `["key"]: Expected = value, Actual = missing` format for key-based diffs.

3.2 Nested collection diffing: when collections contain objects, show property-level diffs for mismatched elements rather than just "elements differ at [i]". [DONE - was already working]

3.3 Add configurable max diff depth to prevent runaway output on deeply nested graphs. [DONE - DiffOptions.MaxDepth already existed]

3.4 Add a `DiffOptions.IgnoreOrder` flag for collection comparison. [DONE]

**Done:** Set-like comparison that finds matching elements regardless of position.

3.5 Add configurable max diff length to prevent runaway output on long collections. [DONE]

**Done:** `DiffOptions.MaxCollectionLength` limits how many collection elements are compared.

## 4. Remove ExpressionToCodeLib dependency for NET6`_OR`_GREATER targets

With the shift away from expression-tree-based assertions, ExpressionToCodeLib is now only used in:
- `ShouldContain(Expression<Func<T,bool>>)` and `ShouldNotContain(Expression<Func<T,bool>>)` in IEnumerableShoulds for rendering the predicate as code in error messages.
- The `Assert.That(T, Expression<Func<T,bool>>)` overloads in Assert.cs/Assertion.cs.

4.1 Remove ExpressionToCode usage in the ShouldContain() and ShouldNotContain(). [DONE]

**Done:** Uses `predicate.ToString()` for NET6+, ExpressionToCode for older targets.

4.2 Replace ExpressionToCode in Assert.That rendering with CallerArgumentExpression in Net6 and greater targets, to capture the predicate source text at compile time. For Older targets that don't support CallerArgumentExpression, stay with the existing code [DONE - was already in place]

**Done:** Assert.cs line 26-34 already uses CallerArgumentExpression.

4.3 Remove the ExpressionToCodeLib and FastExpressionCompiler dependencies from Net6 and greater targets

**Status:** Not done. Assertion.cs still uses ExpressionToCodeLib heavily for:
- `ExpressionToCodeConfiguration.GetAnnotatedToCode().AnnotatedToCode()` for rich expression rendering
- `ObjectStringify.Default.PlainObjectToCode` for object stringification
- `predicate.CompileFast()` from FastExpressionCompiler

Removing these requires significant refactoring of Assertion.cs constructors.

commit type = tidy:
