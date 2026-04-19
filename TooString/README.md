TooString is a Stringifier that goes places serializers don't.

TooString can
- make a best effort to stringify objects that JsonSerializer won't, including
  System.Reflection classes and System.Type; or for which JsonSerializer returns
  empty output, such as ValueTuples.
- Output as Json or C# object notation, or 'debug view' style, or [CallerArgumentExpression]

TooString offers 5 extension method groups on Object:
```csharp
value.ToCSharpString()
value.ToJson();
value.ToSTJson()
value.TooString();
value.ToCallerArgumentString();
```
_TooString is not a serializer._ 

- A serializer should be fail-fast, but TooString is best-effort. 
- A Serializer should throw if it cannot deterministically serialize the input, but TooString 
  will attempt to return a partial or alternative representation of the input if the input 
  cannot be reliably serialized.
- TooString offers both MaxDepth and MaxEnumerationLength options for abbreviated output.


### Default behaviour

- ToCSharpString() returns reflection-based C# anonymous-object notation.
- ToJson() returns reflection-based JSON stringification (not System.Text.Json).
- ToSTJson() is a convenience method for System.Text.Json.JsonSerializer.Serialize(...).
- TooString() defaults to CSharp style; pass a StringifyAs to choose the style.
- ToCallerArgumentString() returns the literal code expression.

All reflection-based methods default to MaxDepth = 3, MaxEnumerationLength = 9.

#### What's the different between ToJson() and ToSTJson()?

- System.Text.Json will return "{}" by default for any ValueTuple. ToJson() will return the tuple items as an array.
- ToJson() can abbreviate output with both maxDepth and maxEnumerableLength options, System.Text.Json has no maxEnumerableLength option.
- System.Text.Json throws given values in System.Reflection, delegates, Types, or other non-serializable types. ToJson() will tell you more than you want to know.
- System.Text.Json will return property values for classes in System.Numerics, ToJson() will represent multi-dimensional numbers as arrays.

Example:
```
var value = new { A = "boo", B = new Complex(3,4) };

value.ToCSharpString()  // Output is { A = "boo", B = <3; 4> } depending on .Net version.
value.ToJson();         // Output is reflection-based JSON: {"A":"boo","B":[3,4]}
value.ToSTJson();       // Output is System.Text.Json: {"A":"boo","B":{"Real":3,...}}

( Math.Sqrt(4 * Math.PI / 3)  ).ToCallerArgumentString() 
// Output is the literal code: "Math.Sqrt(4 * Math.PI / 3)"

var tuple = (one: 1, two: "2", three: new Complex(3,4));

System.Text.Json.JsonSerializer.Serialize(tuple) // Output is "{}" because there  
                                                 // are no public properties on a tuple

tuple.ToJson()
// Output stringifies the tuple and the Complex number as arrays
// [1,"2",[3,4]] 

tuple.ToCSharpString()
// Output is created by reflection and 
// on Net6.0 : (1, "2", (3,4))  
// on Net8.0+: (1, "2", <3;4>)

var type = value.GetType();
System.Text.Json.JsonSerializer.Serialize(type) // Throws NotSupportedException
type.ToJson() // Outputs the type object, truncated to default MaxDepth = 3, MaxEnumerationLength = 9
```

### Options

Each method takes individual optional parameters (no TooStringOptions parameter needed):

```csharp
// Reflection-based JSON
value.ToJson()
value.ToJson(writeIndented: true, maxDepth: 5)

// C# object notation
value.ToCSharpString()
value.ToCSharpString(writeIndented: true, maxDepth: 5)

// System.Text.Json
value.ToSTJson()
value.ToSTJson(writeIndented: true, propertyNamingPolicy: JsonNamingPolicy.CamelCase)

// TooString with style selection
value.TooString()
value.TooString(StringifyAs.JsonStringifier)
value.TooString(maxDepth: 4, maxEnumerableLength: 9, style: StringifyAs.CSharp)
value.TooString(TooStringOptions.ForJson with { MaxEnumerationLength = 9 })
```


### Gotchas

Example: Json-serializing value tuples is something of a surprise because (unlike anonymous objects or records or structs) they have no public properties and their public fields are not named as per your code. Takeaway: don't choose value tuples for public apis that must be jsonned.

```
System.Text.Json.JsonSerializer.Serialize(  (one:1, two:"2")  )
// Output is "{}" because there are no public fields

(one:1, two:"2").ToJson()
// Output is [1,"2"]. 
// The value tuple is detected as an ITuple, and we use reflection instead
```

### ChangeLog
<pre>
ChangeLog
---------
0.8.0  5 extension method groups: ToCSharpString, ToJson, ToSTJson, TooString, ToCallerArgumentString.
       ToJson and ToCSharpString take individual parameters, no TooStringOptions overload.
0.7.0  Easier to build new TooStringOptions(){...}. Sanitize overloads.
0.6.0  TooString() defaults to CSharp.
0.5.0  ReflectionOptions.With(...), TooStringOptions.With(...). Fixes. 
0.4.0  ReflectionOptions.MaxLength default = 9 
       ReflectionOptions.ForJson and ReflectionOptions.ForDebugView instead of Default. 
       More overloads.
0.3.0  ReflectionOptions.MaxLength limits display of enumerable elements
0.2.0  Added Net8 (NB Net8 Json and Numerics output is different from Net6)
       Rename StringifyAs to TooStringHow.
       Fix SerializationStyle.Reflection output of DateTime, DateOnly, TimeOnly.
0.1.0  Can use DebugView, Json, ToString() or [CallerArgumentExpression] and can output Json or Debug strings.
</pre>
