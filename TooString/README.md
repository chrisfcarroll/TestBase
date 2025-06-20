TooString is a Stringifier that goes places serializers don't.

TooString() can
- make a best effort to stringify objects that JsonSerializer won't, including 
  System.Reflection classes and System.Type, or that JsonSerializer surprises 
  you with, such as ValueTuples.
- Output as Json, or ‘debug view’ style, or as [CallerArgumentExpression] 

TooString is not a serializer, it is intended for test and diagnostic display. A reliable Serializer must be fail-fast — it should throw if it cannot deterministically serialize
the input — but TooString is best effort; it will attempt to return a partial
representation of the input even when input cannot reliably be serialized.

Example:
```
( Math.Sqrt(4 * Math.PI / 3)  ).TooString( TooStringHow.CallerArgument ) 
// Output is the literal code: "Math.Sqrt(4 * Math.PI / 3)"

new { A = "boo", B = new Complex(3,4) }.TooString(TooStringHow.Json);
// Output is the System.Text.Json output:
// {"A":"boo","B":{"Real":3,"Imaginary":4,"Magnitude":5,"Phase":0.9272952180016122}}

new { A = "boo", B = new Complex(3,4) }.TooString(TooStringHow.Reflection);
// Output is "{ A = boo, B = (3, 4) }" 
```

### Gotchas

Example: Json-serializing value tuples is something of a surprise because (unlike anonymous objects or 
records or structs) they have no public properties and their public fields are not named as 
per your code. Takeaway: don't choose value tuples for public apis that must be jsonned.

Use modifications of `TooStringOptions.Default` to customise the results.

```
(one:1, two:"2").TooString( TooStringHow.Json )
System.Text.Json.JsonSerializer.Serialize(  (one:1, two:"2")  )
// Output is "{}" because there are no public fields

// do this instead:

var options = TooStringOptions.Default with
{
    JsonOptions = new JsonSerializerOptions { IncludeFields = true }
};
var jsonnedIncludeFields = (one:1, two:"2") .TooString(options);
// Output is "{"Item1":1,"Item2":"2"}"

- Infinite loops are avoided with MaxDepth settings.
```

### ChangeLog
<pre>
0.2.0  Added Net8. NB Net8 Json and Numerics output is different from Net6
       Rename TooStringStyle to TooStringHow.
       Fix SerializationStyle.Reflection output of DateTime, DateOnly, TimeOnly.
0.1.0  Can use DebugView, Json, ToString() or [CallerArgumentExpression] and can output Json or Debug strings.
</pre>
