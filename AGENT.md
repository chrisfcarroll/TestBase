TooString.csproj is a multi-targetted .Net project that stringifies C# objects as either Json or C# or a other formats.

It has some problems

1. The developer interface for TooStringOptions is still too difficult. Is there a way to simplify it? Ensure all the TooString and TestBase tests pass.

2. It would be best if both ToJson() and TooString() had overloads in which every possible option can be set. Except in the case of TooString() there is no need to include the JsonOptions for STJ as a parameter, that is only useful for ToJson(). Make sure you have tests for enough combinations to be soure that the TooStringOptions is correctly built and passed.