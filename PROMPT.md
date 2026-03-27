PROMPT.md 2026-03-27
=========================

Proposals for future improvements to TestBase, based on the work done in PROMPT.md.

## 1. Extend Differ test cases

TestBase.Comparer has test cases for many more edge cases than Differ has. Apply 
those same test cases to Differ and prove it works for records, advanced numerics
and other edge cases

commit type = testcases:

## 2. Extend Differ coverage

The Differ handles objects, collections, strings, and primitives. Consider extending:

2.1 Dictionary diffing: show which keys are missing/extra and which values differ, rather than treating dictionaries as generic collections of KeyValuePair.

commit types = feature:

2.2 Nested collection diffing: when collections contain objects, show property-level diffs for mismatched elements rather than just "elements differ at [i]".

commit types = feature:

2.3 Add configurable max diff depth to prevent runaway output on deeply nested graphs.

commit types = feature:

2.4 Add a `DiffOptions.IgnoreOrder` flag for collection comparison.

commit types = feature:

2.5 Add configurable max diff length to prevent runaway output on long collections.

max depth is a common feature, but max length is not so common. It allows to collections
that are very long and allow us to limit the length of display.

commit types = feature:


## 3. Remove ExpressionToCodeLib dependency for NET6_OR_GREATER targets

With the shift away from expression-tree-based assertions, ExpressionToCodeLib is now only used in:
- `ShouldContain(Expression<Func<T,bool>>)` and `ShouldNotContain(Expression<Func<T,bool>>)` in IEnumerableShoulds for rendering the predicate as code in error messages.
- The `Assert.That(T, Expression<Func<T,bool>>)` overloads in Assert.cs/Assertion.cs.

3.1 Remove ExpressionToCode usage in the ShouldContain() and ShouldNotContain().

commit type = tidy:

3.2 Replace ExpressionToCode in Assert.That rendering with CallerArgumentExpression in Net6 and greater targets, to capture the predicate source text at compile time. For Older targets that don't support CallerArgumentExpression, stay with the existing code

commit type = tidy:

3.3 Remove the ExpressionToCodeLib and FastExpressionCompiler dependencies from Net6 and greater targets

commit type = tidy:
