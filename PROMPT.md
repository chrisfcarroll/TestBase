PROMPT.md 2026-04-10
=========================

1. The developer-facing interface is not yet simple enough.

Correctly the readme says
```csharp
value.TooString();
value.ToJson();
value.ToCallerArgumentString();
```
But let's make it to this:

5 extension method groups on Object
```csharp
value.ToCSharpString()
value.ToJson();
value.ToSTJson()
value.TooString();
value.ToCallerArgumentString();
```

ToCSharpString() and ToJson() will simply pass through to TooString() with the appropriate parameters ans StringifyAs value. There should be no overload that takes a TooStringOptions parameter. Instead, they should just take the individual parameters, with default values so that every parameter is optional, of all the options.

1.1. Update the code to match this plan
1.2. Update the README to match
1.3. Update the tests for the readme code to match

Then check in as 'change' with a short scope and explanation
Then check in TooString with the next version bump, but still preview

2. After this change, change the default for ToJson,ToCSharpString and TooString to writeIndented=true.
- This will cause a big change to the Tests because the tests currently assume writeIndented=false.
- This also means you must review all the Json tests to carefully split them between ToSTJon, which keeps the STJ default, and ToJson which has this default.

Then check in as 'change' with a short scope and explanation
Then check in TooString with the next version bump, but still preview

3. After this, change the usage by TestBase.Assertion of TooString to maxDepth=1, maxLength=3.
- This too will have rippling changes to TestBase tests so you have to fix them all.

Then check in as 'change' with a short scope and explanation
Then check in TestBase with the next version bump, but still preview.

4. The write a short report.md and list anything that is inconsistent or confusing or wrong in the developer experience of (1) TooString and then (2) TestBase. Especially look if anythign can be clearer more helpful in the output of TestBase assertions.
