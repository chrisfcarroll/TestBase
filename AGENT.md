TooString.csproj is a multi-targetted .Net project that stringifies C# objects as either Json or C# or a other formats.

It has some problems

1. We have to be clear on the difference between STJson and our own Stringify Json. Let's make that clear with the top level methods. Add a ToSTJson() method on Object TooString(). One overload should accept a JsonSerializerOptions parameter, and a another overload should accept individual parameters for all the things you can reasonably set on JsonSerializerOptions. These methods should not use TooString options at all. Add tests to make sure the right call is made/the right result is returned.

2. Then, the existing ToJson() should only use JsonStringifier. Add an overload in which in which every possible option can be set.

3. TooString() should also have an overload in which every possible option can be set. This can replace the existing overload in which some options can be set. There is no need to include the JsonOptions for STJ as a parameter, that is only useful for ToJson(). Make sure you have tests for enough combinations to be sure that the TooStringOptions is correctly built and passed.

4. Then see how you can simplify the TooStringOptions. It still seems too complicated.

For all of these, use my conventional commit format of 'one-word-reason:scope:detail' eg 'wip:TooString.cs,TooStringOptions.cs:simplify options'
