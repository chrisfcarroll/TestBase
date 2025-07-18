TooString is a Stringifier that goes places serializers don't.

TooString() can
- make a best effort to stringify objects that JsonSerializer won't, including 
  System.Reflection classes and System.Type, or that JsonSerializer surprises 
  you with, such as ValueTuples.
- Output as Json, or ‘debug view’ style, or as [CallerArgumentExpression] 

TooString offers 3 extension method groups on Object:
```csharp
value.TooString();
value.ToJson();
value.ToDebugViewString();
```
TooString is not a serializer. A serializer should be fail-fast, but TooString is best-effort.
A Serializer should throw if it cannot deterministically serialize the input, but TooString 
will attempt to return a partial or alternative representation of the input even when input 
cannot reliably be serialized.

#### Default behaviour

- ToJson() defaults to using System.Text.Json, falling back to reflection for un-serializable types.
whereas
- TooString(ReflectionStyle.Json) 
- TooString(ReflectionStyle.DebugView)
- ToDebugView()
all defaults to MaxDepth = 4, MaxEnumerationLength = 9.

Example:
```
var value = new { A = "boo", B = new Complex(3,4) };

value.ToJson();           // Output Is System.Text.Json {"A":"boo","B":{"Real":3,"Imaginary":...}}
value.ToDebugViewString(),//Output is { A = boo, B = <3; 4> } depending on .Net version.

( Math.Sqrt(4 * Math.PI / 3)  ).TooString( TooStringHow.CallerArgument ) 
// Output is the literal code: "Math.Sqrt(4 * Math.PI / 3)"

var tuple = (one: 1, two: "2", three: new Complex(3,4));

System.Text.Json.JsonSerializer.Serialize(tuple) // Output is "{}" because there  
                                                 // are no public properties on a tuple

tuple.TooString(ReflectionStyle.Json)
// Output is created by reflection and stringifies the tuple and the Complex number as arrays
// [1,"2",[3,4]] 

tuple.ToDebugViewString()
tuple.TooString(TooStringHow.Reflection)
// Output is created by reflection and mimics typical debugger display
// on Net6.0: {item1 = 1, item2 = "2", item3 = (3,4)}  
// on Net8.0: {item1 = 1, item2 = "2", item3 = <3;4>}

var type = value.GetType();
System.Text.Json.JsonSerializer.Serialize(type) // Throws NotSupportedException

type.ToJson() // Outputs truncated Json with default MaxDepth = 4, MaxEnumerationLength = 9
```

### Options

Options are finicky because we can do either Json serialization, or Reflection-based stringification
with Json output or with Debugview output, or CallerArgumentExpression.
For Json serialization, the options are System.Text.Json.JsonSerializationOptions. 
For Reflection-based output, there are options for MaxDepth, but also MaxLength (of 
enumerable display), and for DateTime, DateOnly, TimeOnly, and TimeSpan formatting.

Try one of these approaches:

```csharp
// For Json using STJ
value.ToJson()
value.TooString( new JsonSerializerOptions(JsonSerializerDefaults.Web){WriteIndented = true})
// For Json using Reflection
value.TooString( ReflectionStyle.Json )
value.TooString( ReflectionOptions.ForJson with {} )


// For DebugView
value.ToDebugViewString()
value.TooString( ReflectionStyle.DebugView )
value.TooString( ReflectionOptions.ForDebugView.With(...) ) // Same output as value.ToDebugViewString()
value.TooString( maxDepth:4, maxLength:9, style:ReflectionStyle.DebugView )
value.TooString( ReflectionOptions.ForDebugView with 
{
    DateTimeFormat = "yyyyMMdd HH:mm:ss",
    TimeSpanFormat = @"d\.hh\:mm\:ss"
})

// For Either
value.TooString( TooStringOptions.Default with { ... } )

//CallerArgumentExpression will automatically be chosen if the expression is not just a name:
(1 + value).TooString()
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
