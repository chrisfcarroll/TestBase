# TestBase.Differ

Prerelease: An object-to-object comparison library that produces a tree of differences. Useful for unit tests, audit logs, or debugging.

## API

### Differ
The main entry point for comparing two objects.

```csharp
var result = Differ.Diff(expected, actual, options);
if (!result) // Implicit bool conversion for result.AreEqual
{
    Console.WriteLine(result.ToString());
}
```

### DiffOptions
Controls how comparison is performed.
- `FloatTolerance`: Tolerance for floating-point comparison (default: 1e-14).
- `ExcludeMembers`: Member names to exclude from comparison (supports dotted paths).
- `IncludeOnlyMembers`: If non-empty, only these members are compared.
- `RequireSameType`: If true, types must match exactly (default: false).
- `MaxDifferences`: Maximum number of differences to report before stopping (default: 2).
- `MaxDepth`: Maximum recursion depth (default: 10).
- `LeftLabel` / `RightLabel`: Labels used in output (default: "Expected"/"Actual").
- `PublicPropertiesOnly`: Only compare public readable properties.
- `WritablePropertiesOnly`: Only compare writable properties.

### DiffResult
Represents the result of a comparison.
- `AreEqual`: Boolean property (also supports implicit conversion to `bool`).
- `Path`: The dotted path to the difference (e.g., "User.Address.Street").
- `LeftValue` / `RightValue`: String representations of the values that differed.
- `Children`: A list of child `DiffResult` objects for nested differences.
- `ToString()`: Produces a clean, indented summary of all differences.

### DiffFormatter
Formats `DiffResult` with optional ANSI colour output for console display.
- `DiffFormatter.Format(result)`: Returns a formatted string.
- `DiffFormatter.UseColour`: Static property to enable/disable ANSI colour output (default: false).
