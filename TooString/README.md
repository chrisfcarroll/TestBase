The TooString() extension method stringifies objects that other serializers don't.


TooString() can
- make a best effort to stringify unserializable objects.
- Output as Json, or C# Code, or ‘debug’ output

Example:
```
( Math.Sqrt(4 * Math.PI / 3)  ).TooString( TooStringStyle.CallerArgument ) 
// Output is the literal code: "Math.Sqrt(4 * Math.PI / 3)"

new { A = "boo", B = new Complex(3,4) }.TooString(TooStringStyle.Json);
// Output is the System.Text.Json output:
// {"A":"boo","B":{"Real":3,"Imaginary":4,"Magnitude":5,"Phase":0.9272952180016122}}

new { A = "boo", B = new Complex(3,4) }.TooString(TooStringStyle.Reflection);
// Output is "{ A = boo, B = { Real = 3, Imaginary = 4, Magnitude = 5, Phase = 0.9272952180016122 } }" 
```

### Gotchas

- Json serializing value tuples is something of a surprise because (unlike anonymous objects or
  records or structs) they have no public properties and their public fields are not named as
  per your code. Takeaway: don't choose value tuples for public apis that must be jsonned.
```
(one:1, two:"2").TooString( TooStringStyle.Json )
System.Text.Json.JsonSerializer.Serialize(  (one:1, two:"2")  )
// Output is "{}" because there are no public fields
(one:1, two:"2").TooString( TooStringStyle.Json )
var options = TooStringOptions.Default with
{
    JsonOptions = new JsonSerializerOptions { IncludeFields = true }
};
var jsonnedIncludeFields = (one:1, two:"2") .TooString(options);
// Output is "{Item1:1,Item2:"2"}"

- Infinite loops are avoided with MaxDepth settings.

ChangeLog
---------
0.1.0.0  Can use Reflection, Json, ToString() or [CallerArgumentExpression] and can output Json or Debug strings.