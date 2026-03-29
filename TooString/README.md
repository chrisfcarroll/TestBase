TooString is a Stringifier that goes places serializers don't.

TooString can
- make a best effort to stringify objects that JsonSerializer won't, including 
  System.Reflection classes and System.Type, or that JsonSerializer surprises 
  you with, such as ValueTuples.
- Output as Json or C# object notation, or ‘debug view’ style, or [CallerArgumentExpression] 

TooString offers 3 extension method groups on Object:
```csharp
value.ToStringified();
value.ToJson();
value.ToCallerArgumentString();
```
_TooString is not a serializer._ 

- A serializer should be fail-fast, but TooString is best-effort. 
- A Serializer should throw if it cannot deterministically serialize the input, but TooString 
  will attempt to return a partial or alternative representation of the input if the input 
  cannot be reliably serialized.
- TooString offers both MaxDepth and MaxEnumerationLength options for abbreviated output.


#### Default behaviour

- ToJson() or TooString(TooStringStyle.JsonStringifier) default to using System.Text.Json, 
  falling back to reflection for un-serializable types.

whereas
- TooString()
- TooString(TooStringStyle.JsonStringifier) 
- TooString(TooStringStyle.CSharp)
- TooString(TooStringStyle.DebugView)
all defaults to MaxDepth = 4, MaxEnumerationLength = 9.

Example:
```
var value = new { A = "boo", B = new Complex(3,4) };

value.ToJson();       // Output Is System.Text.Json {"A":"boo","B":{"Real":3,"Imaginary":...}}
value.TooString(),    //Output is { A = boo, B = <3; 4> } depending on .Net version.

( Math.Sqrt(4 * Math.PI / 3)  ).ToArgumentExpression() 
// Output is the literal code: "Math.Sqrt(4 * Math.PI / 3)"

var tuple = (one: 1, two: "2", three: new Complex(3,4));

System.Text.Json.JsonSerializer.Serialize(tuple) // Output is "{}" because there  
                                                 // are no public properties on a tuple

tuple.TooString(TooStringStyle.Json)
// Output stringifies the tuple and the Complex number as arrays
// [1,"2",[3,4]] 

tuple.ToStringified()
tuple.TooString(TooStringStyle.CSharp)
// Output is created by reflection and 
// on Net6.0: {item1 = 1, item2 = "2", item3 = (3,4)}  
// on Net8.0: {item1 = 1, item2 = "2", item3 = <3;4>}

var type = value.GetType();
System.Text.Json.JsonSerializer.Serialize(type) // Throws NotSupportedException

type.ToJson() // Outputs truncated Json with default MaxDepth = 4, MaxEnumerationLength = 9
```

### Options

Options are finicky because we can do 
- Json serialization using System.Text.Json, 
- or Reflection-based stringification with 
  - Json output or with 
  - CSharp output, 
  - or with DebugView output,
    - these all having MaxDepth, MaxEnumerationLength, and time/date format options. 
- or CallerArgumentExpression.

- For Json serialization, the options are System.Text.Json.JsonSerializationOptions. 
- For stringifying, there are options for MaxDepth, MaxEnumerationLength (for 
enumerables), and for DateTime, DateOnly, TimeOnly, and TimeSpan formatting.

Try one of these approaches:

```csharp
// For Json Output
value.ToJson()
value.TooString(TooStringStyle.JsonSerializer)
value.TooString( TooStringStyle.JsonStringifier )
value.TooString( AdvancedOptions.ForJson with { MaxEnumerationLength = 9 } )


// For CSharp objects,
value.TooString( TooStringStyle.CSharp)
value.TooString( AdvancedOptions.ForCSharp.With(...) )
value.TooString( maxDepth:4, maxLength:9, style:TooStringStyle.CSharp )
value.TooString( AdvancedOptions.ForStringified with 
{
    DateTimeFormat = "yyyyMMdd HH:mm:ss",
    TimeSpanFormat = @"d\.hh\:mm\:ss"
})
```


### Gotchas

Example: Json-serializing value tuples is something of a surprise because (unlike anonymous objects or 
records or structs) they have no public properties and their public fields are not named as 
per your code. Takeaway: don't choose value tuples for public apis that must be jsonned.

Use modifications of `TooStringOptions.Default` to customise the results.

```
System.Text.Json.JsonSerializer.Serialize(  (one:1, two:"2")  )
// Output is "{}" because there are no public fields

(one:1, two:"2").TooString( TooStringHow.Json )
// Output is [1,"2"]. 
// The value tuple is detected as an ITuple, and we use reflection instead
```

### ChangeLog
<pre>
ChangeLog
---------
0.5.0  ReflectionOptions.With(...), TooStringOptions.With(...). Fixes. 
0.4.0  ReflectionOptions.MaxLength default = 9 
       ReflectionOptions.ForJson and ReflectionOptions.ForDebugView instead of Default. 
       More overloads.
0.3.0  ReflectionOptions.MaxLength limits display of enumerable elements
0.2.0  Added Net8 (NB Net8 Json and Numerics output is different from Net6)
       Rename TooStringStyle to TooStringHow.
       Fix SerializationStyle.Reflection output of DateTime, DateOnly, TimeOnly.
0.1.0  Can use DebugView, Json, ToString() or [CallerArgumentExpression] and can output Json or Debug strings.
</pre>
