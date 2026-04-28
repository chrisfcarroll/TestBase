# Assertion Comparison Report: TestBase vs NUnit

Generated: 2026-04-27 23:13:15Z

## Boolean: true ShouldBeFalse

### TestBase
```
Expected: false
Actual:   true
Asserted: ShouldBeFalse
```

### NUnit
```
  Assert.That(true, NUnitIs.False)
  Expected: False
  But was:  True

```

## Boolean: false ShouldBeTrue

### TestBase
```
Expected: true
Actual:   false
Asserted: ShouldBeTrue
```

### NUnit
```
  Assert.That(false, NUnitIs.True)
  Expected: True
  But was:  False

```

## Collection: [1,2,3] ShouldBeEmpty

### TestBase
```
Expected: empty collection
Actual:   collection has elements
Asserted: ShouldBeEmpty
```

### NUnit
```
  Assert.That(list, NUnitIs.Empty)
  Expected: <empty>
  But was:  < 1, 2, 3 >

```

## Collection: [1,2,3] ShouldContain 5

### TestBase
```
Expected: collection containing 5
Actual:   [1, 2, 3]
Asserted: ShouldContain
```

### NUnit
```
  Assert.That(list, NUnitDoes.Contain(5))
  Expected: some item equal to 5
  But was:  < 1, 2, 3 >

```

## Collection: [] ShouldNotBeEmpty

### TestBase
```
Expected: non-empty collection
Actual:   empty
Asserted: ShouldNotBeEmpty
```

### NUnit
```
  Assert.That(list, NUnitIs.Not.Empty)
  Expected: not <empty>
  But was:  <empty>

```

## Comparison: 1 ShouldBeGreaterThan 5

### TestBase
```
Expected: > 5
Actual:   1
Asserted: ShouldBeGreaterThan
```

### NUnit
```
  Assert.That(1, NUnitIs.GreaterThan(5))
  Expected: greater than 5
  But was:  1

```

## Comparison: 10 ShouldBeLessThan 3

### TestBase
```
Expected: < 3
Actual:   10
Asserted: ShouldBeLessThan
```

### NUnit
```
  Assert.That(10, NUnitIs.LessThan(3))
  Expected: less than 3
  But was:  10

```

## EqualByValue: objects differing on Age

### TestBase
```
Age: Expected = 25, Actual = 30

Actual:   {
  Name = "Alice",
  Age = 30
}
Asserted: ShouldEqualByValue
```

### NUnit
```
  Assert.That(actual.Age, NUnitIs.EqualTo(expected.Age))
  Expected: 25
  But was:  30

```

## Equality: int 1 vs 2

### TestBase
```
Expected: 2
Actual:   1
Asserted: ShouldBe 2
```

### NUnit
```
  Assert.That(1, NUnitIs.EqualTo(2))
  Expected: 2
  But was:  1

```

## Equality: long strings differing mid-sentence

### TestBase
```
String lengths are both 43. Strings differ at index 20.
Expected: "The quick brown fox leaps over the lazy dog"
Actual:   "The quick brown fox jumps over the lazy dog"
-------------------------------^
Asserted: ShouldBe
```

### NUnit
```
  Assert.That(actual, NUnitIs.EqualTo(expected))
  String lengths are both 43. Strings differ at index 20.
  Expected: "The quick brown fox leaps over the lazy dog"
  But was:  "The quick brown fox jumps over the lazy dog"
  -------------------------------^

```

## Equality: anonymous objects differing on Age

### TestBase
```
Expected: { Name = Alice, Age = 25 }
Actual:   { Name = Alice, Age = 30 }
Asserted: ShouldBe { Name = Alice, Age = 25 }
```

### NUnit
```
  Assert.That(actual, NUnitIs.EqualTo(expected))
  Expected: <{ Name = Alice, Age = 25 }>
  But was:  <{ Name = Alice, Age = 30 }>

```

## Equality: string 'hello' vs 'world'

### TestBase
```
String lengths are both 5. Strings differ at index 0.
Expected: "world"
Actual:   "hello"
-----------^
Asserted: ShouldBe
```

### NUnit
```
  Assert.That("hello", NUnitIs.EqualTo("world"))
  String lengths are both 5. Strings differ at index 0.
  Expected: "world"
  But was:  "hello"
  -----------^

```

## Equality: string 'abcdef' vs 'abcXYZ'

### TestBase
```
String lengths are both 6. Strings differ at index 3.
Expected: "abcXYZ"
Actual:   "abcdef"
--------------^
Asserted: ShouldBe
```

### NUnit
```
  Assert.That("abcdef", NUnitIs.EqualTo("abcXYZ"))
  String lengths are both 6. Strings differ at index 3.
  Expected: "abcXYZ"
  But was:  "abcdef"
  --------------^

```

## Null: 'hello' ShouldBeNull

### TestBase
```
Expected: null
Actual:   hello
Asserted: ShouldBeNull
```

### NUnit
```
  Assert.That("hello", NUnitIs.Null)
  Expected: null
  But was:  "hello"

```

## Null: null ShouldNotBeNull

### TestBase
```
Expected: not null
Actual:   s
Asserted: ShouldNotBeNull
```

### NUnit
```
  Assert.That(s, NUnitIs.Not.Null)
  Expected: not null
  But was:  null

```

## String: 'hello world' ShouldContain 'xyz'

### TestBase
```
Expected: String containing "xyz"
Actual:   "hello world"
Asserted: ShouldContain
```

### NUnit
```
  Assert.That("hello world", NUnitDoes.Contain("xyz"))
  Expected: String containing "xyz"
  But was:  "hello world"

```

## String: 'abc123' ShouldMatch '^[0-9]+$'

### TestBase
```
Expected: match /^[0-9]+$/
Actual:   "abc123"
Asserted: ShouldMatch
```

### NUnit
```
  Assert.That("abc123", NUnitDoes.Match("^[0-9]+$"))
  Expected: String matching "^[0-9]+$"
  But was:  "abc123"

```

## String: 'hello world' ShouldStartWith 'world'

### TestBase
```
Expected: starts with "world"
Actual:   "hello world"
Asserted: ShouldStartWith
```

### NUnit
```
  Assert.That("hello world", NUnitDoes.StartWith("world"))
  Expected: String starting with "world"
  But was:  "hello world"

```

## Type: string ShouldBeOfType<int>

### TestBase
```
Expected: type System.Int32
Actual:   type System.String
Asserted: ShouldBeOfType
```

### NUnit
```
  Assert.That(val, NUnitIs.TypeOf<int>())
  Expected: <System.Int32>
  But was:  <System.String>

```

