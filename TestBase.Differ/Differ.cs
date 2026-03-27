using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using TooString;

namespace TestBase;

/// <summary>
/// Produces clean, readable diffs of .NET objects: strings, primitives, collections, and complex objects.
/// <code>
/// var diff = Differ.Diff(expected, actual);
/// if (!diff.AreEqual)
///     Console.WriteLine(diff.ToString());
/// </code>
/// </summary>
public static class Differ
{
    /// <summary>
    /// Compare two values and return a DiffResult tree.
    /// ToString() on the result produces a clean, readable display of differences.
    /// </summary>
    public static DiffResult Diff(object? left, object? right, DiffOptions? options = null)
    {
        var opts = options ?? DiffOptions.Default;
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
        var result = DiffCore(left, right, "", opts, visited, depth: 0);
        return result.WithLabels(opts.LeftLabel, opts.RightLabel);
    }

    static DiffResult DiffCore(
        object? left, object? right, string path,
        DiffOptions opts, HashSet<object> visited, int depth)
    {
        // Both null
        if (left is null && right is null) return DiffResult.Equal;
        if (opts.NullEqualsDbNull)
        {
            if (left is DBNull && right is null) return DiffResult.Equal;
            if (left is null && right is DBNull) return DiffResult.Equal;
        }

        // Reference equality
        if (ReferenceEquals(left, right)) return DiffResult.Equal;

        // One is null
        if (left is null)
        {
            if (opts.NullEqualsEmptyCollection && right is IEnumerable e && right is not string && !HasElements(e))
                return DiffResult.Equal;
            return DiffResult.Different(path, "null", Stringify(right), "one side is null");
        }
        if (right is null)
        {
            if (opts.NullEqualsEmptyCollection && left is IEnumerable e && left is not string && !HasElements(e))
                return DiffResult.Equal;
            return DiffResult.Different(path, Stringify(left), "null", "one side is null");
        }

        var leftType = left.GetType();
        var rightType = right.GetType();

        // Depth guard
        if (depth > opts.MaxDepth)
            return DiffResult.Equal;

        // Cycle detection
        if (!leftType.IsValueType && !visited.Add(left))
            return DiffResult.Equal;

        // Type mismatch
        if (opts.RequireSameType && leftType != rightType)
            return DiffResult.Different(path, leftType.Name, rightType.Name, "types differ");

        // FileSystemInfo special case (avoids StackOverflow)
        if (left is FileSystemInfo lf && right is FileSystemInfo rf)
            return lf.FullName == rf.FullName
                ? DiffResult.Equal
                : DiffResult.Different(path, lf.FullName, rf.FullName, "paths differ");
        if (left is FileSystemInfo || right is FileSystemInfo)
            return DiffResult.Different(path, leftType.Name, rightType.Name, "types differ");

        // Strings — show first difference point
        if (left is string ls && right is string rs)
            return DiffStrings(ls, rs, path);

        // Floating-point with tolerance
        if (IsFloatingPoint(left) && IsFloatingPoint(right))
            return DiffFloats(Convert.ToDouble(left), Convert.ToDouble(right), path, opts.FloatTolerance);

        // Value types and types that override Equals (but not records — recurse into those)
        if (leftType.IsValueType || (OverridesEquals(leftType) && !IsRecord(leftType)))
        {
            return left.Equals(right)
                ? DiffResult.Equal
                : DiffResult.Different(path, Stringify(left), Stringify(right));
        }

        // Dictionaries - show key-level diffs rather than treating as KeyValuePair collection
        if (left is IDictionary leftDict && right is IDictionary rightDict)
            return DiffDictionaries(leftDict, rightDict, path, opts, visited, depth);

        // Collections
        if (left is IEnumerable leftEnum && right is IEnumerable rightEnum)
            return DiffCollections(leftEnum, rightEnum, path, opts, visited, depth);

        if (left is IEnumerable || right is IEnumerable)
            return DiffResult.Different(path, leftType.Name, rightType.Name, "one side is a collection, the other is not");

        // Complex objects — compare members
        return DiffMembers(left, right, path, opts, visited, depth);
    }

    static DiffResult DiffStrings(string left, string right, string path)
    {
        if (left == right) return DiffResult.Equal;

        int firstDiff = 0;
        int minLen = Math.Min(left.Length, right.Length);
        while (firstDiff < minLen && left[firstDiff] == right[firstDiff])
            firstDiff++;

        int snippetStart = Math.Max(0, firstDiff - 10);
        int snippetEnd = Math.Min(minLen, firstDiff + 20);

        string SnipAt(string s)
        {
            var end = Math.Min(s.Length, snippetEnd);
            var snip = s[snippetStart..end];
            var prefix = snippetStart > 0 ? "..." : "";
            var suffix = end < s.Length ? "..." : "";
            return $"\"{prefix}{snip}{suffix}\"";
        }

        var msg = left.Length != right.Length
            ? $"strings differ at index {firstDiff} (lengths {left.Length} vs {right.Length})"
            : $"strings differ at index {firstDiff}";

        return DiffResult.Different(path, SnipAt(left), SnipAt(right), msg);
    }

    static DiffResult DiffFloats(double left, double right, string path, double tolerance)
    {
        if (left > right - tolerance && left < right + tolerance)
            return DiffResult.Equal;
        return DiffResult.Different(path, left.ToString("G17"), right.ToString("G17"),
            $"values differ (tolerance {tolerance})");
    }

    static DiffResult DiffCollections(
        IEnumerable left, IEnumerable right, string path,
        DiffOptions opts, HashSet<object> visited, int depth)
    {
        var leftList = left.Cast<object?>().ToList();
        var rightList = right.Cast<object?>().ToList();

        // Apply MaxCollectionLength limit
        var maxLen = opts.MaxCollectionLength > 0 ? opts.MaxCollectionLength : int.MaxValue;
        var leftTruncated = leftList.Count > maxLen;
        var rightTruncated = rightList.Count > maxLen;
        if (leftTruncated) leftList = leftList.Take(maxLen).ToList();
        if (rightTruncated) rightList = rightList.Take(maxLen).ToList();

        var diffs = new List<DiffResult>();

        if (opts.IgnoreOrder)
            DiffCollectionsUnordered(leftList, rightList, path, opts, visited, depth, diffs);
        else
            DiffCollectionsOrdered(leftList, rightList, path, opts, visited, depth, diffs);

        if (diffs.Count == 0) return DiffResult.Equal;
        return DiffResult.WithChildren(path, diffs);
    }

    static void DiffCollectionsOrdered(
        List<object?> leftList, List<object?> rightList, string path,
        DiffOptions opts, HashSet<object> visited, int depth, List<DiffResult> diffs)
    {
        int maxItems = Math.Max(leftList.Count, rightList.Count);
        int diffsFound = 0;

        for (int i = 0; i < maxItems && diffsFound < opts.MaxDifferences; i++)
        {
            if (i >= leftList.Count)
            {
                diffs.Add(DiffResult.Different($"{path}[{i}]", "missing", Stringify(rightList[i])));
                diffsFound++;
            }
            else if (i >= rightList.Count)
            {
                diffs.Add(DiffResult.Different($"{path}[{i}]", Stringify(leftList[i]), "missing"));
                diffsFound++;
            }
            else
            {
                var itemDiff = DiffCore(leftList[i], rightList[i], $"{path}[{i}]", opts, visited, depth + 1);
                if (!itemDiff.AreEqual)
                {
                    diffs.Add(itemDiff);
                    diffsFound++;
                }
            }
        }
    }

    static void DiffCollectionsUnordered(
        List<object?> leftList, List<object?> rightList, string path,
        DiffOptions opts, HashSet<object> visited, int depth, List<DiffResult> diffs)
    {
        var matchedRight = new bool[rightList.Count];
        int diffsFound = 0;

        // For each item in left, find a matching item in right
        for (int li = 0; li < leftList.Count && diffsFound < opts.MaxDifferences; li++)
        {
            var leftItem = leftList[li];
            bool found = false;

            for (int ri = 0; ri < rightList.Count; ri++)
            {
                if (matchedRight[ri]) continue;

                var itemDiff = DiffCore(leftItem, rightList[ri], "", opts, visited, depth + 1);
                if (itemDiff.AreEqual)
                {
                    matchedRight[ri] = true;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                diffs.Add(DiffResult.Different($"{path}[{li}]", Stringify(leftItem), "no match"));
                diffsFound++;
            }
        }

        // Report unmatched items from right
        for (int ri = 0; ri < rightList.Count && diffsFound < opts.MaxDifferences; ri++)
        {
            if (!matchedRight[ri])
            {
                diffs.Add(DiffResult.Different($"{path}[+{ri}]", "no match", Stringify(rightList[ri])));
                diffsFound++;
            }
        }
    }

    static DiffResult DiffDictionaries(
        IDictionary left, IDictionary right, string path,
        DiffOptions opts, HashSet<object> visited, int depth)
    {
        var diffs = new List<DiffResult>();
        int diffsFound = 0;
        var leftKeys = new HashSet<object>(left.Keys.Cast<object>());
        var rightKeys = new HashSet<object>(right.Keys.Cast<object>());

        // Keys in left but not right
        foreach (var key in leftKeys.Except(rightKeys))
        {
            if (diffsFound >= opts.MaxDifferences) break;
            var keyPath = $"{path}[{Stringify(key)}]";
            diffs.Add(DiffResult.Different(keyPath, Stringify(left[key]), "missing"));
            diffsFound++;
        }

        // Keys in right but not left
        foreach (var key in rightKeys.Except(leftKeys))
        {
            if (diffsFound >= opts.MaxDifferences) break;
            var keyPath = $"{path}[{Stringify(key)}]";
            diffs.Add(DiffResult.Different(keyPath, "missing", Stringify(right[key])));
            diffsFound++;
        }

        // Compare values for common keys
        foreach (var key in leftKeys.Intersect(rightKeys))
        {
            if (diffsFound >= opts.MaxDifferences) break;
            var keyPath = $"{path}[{Stringify(key)}]";
            var valueDiff = DiffCore(left[key], right[key], keyPath, opts, visited, depth + 1);
            if (!valueDiff.AreEqual)
            {
                diffs.Add(valueDiff);
                diffsFound++;
            }
        }

        if (diffs.Count == 0) return DiffResult.Equal;
        return DiffResult.WithChildren(path, diffs);
    }

    static DiffResult DiffMembers(
        object left, object right, string path,
        DiffOptions opts, HashSet<object> visited, int depth)
    {
        var leftType = left.GetType();
        var rightType = right.GetType();
        var diffs = new List<DiffResult>();
        int diffsFound = 0;

        var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        if (!opts.PublicPropertiesOnly)
            bindingFlags |= BindingFlags.NonPublic;

        // Compare properties
        foreach (var prop in leftType.GetProperties(bindingFlags | BindingFlags.GetProperty))
        {
            if (diffsFound >= opts.MaxDifferences) break;

            var memberPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";

            if (IsExcluded(memberPath, opts)) continue;
            if (!IsIncluded(memberPath, opts)) continue;
            if (opts.WritablePropertiesOnly && !prop.CanWrite) continue;
            if (!prop.CanRead) continue;

            try
            {
                if (prop.GetGetMethod(true)?.GetParameters().Length != 0) continue;
                var rightProp = rightType.GetProperty(prop.Name, bindingFlags | BindingFlags.GetProperty);
                if (rightProp is null)
                {
                    if (opts.NullEqualsMissingProperty)
                    {
                        var leftValP = prop.GetValue(left);
                        if (leftValP is null) continue;
                    }

                    diffs.Add(DiffResult.Different(memberPath, "exists", "missing", "property missing on right"));
                    diffsFound++;
                    continue;
                }

                var leftVal = prop.GetValue(left);
                var rightVal = rightProp.GetValue(right);
                var memberDiff = DiffCore(leftVal, rightVal, memberPath, opts, visited, depth + 1);
                if (!memberDiff.AreEqual)
                {
                    diffs.Add(memberDiff);
                    diffsFound++;
                }
            }
            catch { /* skip inaccessible properties */ }
        }

        // Compare public fields
        if (!opts.PublicPropertiesOnly && !opts.WritablePropertiesOnly)
        {
            foreach (var field in leftType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (diffsFound >= opts.MaxDifferences) break;

                var memberPath = string.IsNullOrEmpty(path) ? field.Name : $"{path}.{field.Name}";

                if (IsExcluded(memberPath, opts)) continue;
                if (!IsIncluded(memberPath, opts)) continue;

                try
                {
                    var rightField = rightType.GetField(field.Name, BindingFlags.Public | BindingFlags.Instance);
                    if (rightField is null)
                    {
                        if (opts.NullEqualsMissingProperty)
                        {
                            var leftVal2 = field.GetValue(left);
                            if (leftVal2 is null) continue;
                        }

                        diffs.Add(DiffResult.Different(memberPath, "exists", "missing", "field missing on right"));
                        diffsFound++;
                        continue;
                    }

                    var leftVal = field.GetValue(left);
                    var rightVal = rightField.GetValue(right);
                    var memberDiff = DiffCore(leftVal, rightVal, memberPath, opts, visited, depth + 1);
                    if (!memberDiff.AreEqual)
                    {
                        diffs.Add(memberDiff);
                        diffsFound++;
                    }
                }
                catch { /* skip inaccessible fields */ }
            }
        }

        // Check for properties on right that left doesn't have
        foreach (var prop in rightType.GetProperties(bindingFlags | BindingFlags.GetProperty))
        {
            if (diffsFound >= opts.MaxDifferences) break;
            var memberPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";
            if (IsExcluded(memberPath, opts)) continue;
            if (!IsIncluded(memberPath, opts)) continue;
            if (opts.WritablePropertiesOnly && !prop.CanWrite) continue;

            var leftProp = leftType.GetProperty(prop.Name, bindingFlags | BindingFlags.GetProperty);
            if (leftProp is null)
            {
                if (opts.NullEqualsMissingProperty)
                {
                    try
                    {
                        if (prop.GetGetMethod(true)?.GetParameters().Length != 0) continue;
                        var rightVal = prop.GetValue(right);
                        if (rightVal is null) continue;
                    }
                    catch { }
                }

                diffs.Add(DiffResult.Different(memberPath, "missing", "exists", "property missing on left"));
                diffsFound++;
            }
        }

        // Check for fields on right that left doesn't have
        if (!opts.PublicPropertiesOnly && !opts.WritablePropertiesOnly)
        {
            foreach (var field in rightType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (diffsFound >= opts.MaxDifferences) break;
                var memberPath = string.IsNullOrEmpty(path) ? field.Name : $"{path}.{field.Name}";
                if (IsExcluded(memberPath, opts)) continue;
                if (!IsIncluded(memberPath, opts)) continue;

                var leftField = leftType.GetField(field.Name, BindingFlags.Public | BindingFlags.Instance);
                if (leftField is null)
                {
                    if (opts.NullEqualsMissingProperty)
                    {
                        try
                        {
                            var rightVal = field.GetValue(right);
                            if (rightVal is null) continue;
                        }
                        catch { }
                    }

                    diffs.Add(DiffResult.Different(memberPath, "missing", "exists", "field missing on left"));
                    diffsFound++;
                }
            }
        }

        if (diffs.Count == 0)
        {
            // Anonymous types with matching structure
            if (IsAnonymousType(leftType) && IsAnonymousType(rightType)) return DiffResult.Equal;
            if (leftType == rightType || !opts.RequireSameType) return DiffResult.Equal;
            return DiffResult.Different(path, leftType.Name, rightType.Name, "types differ");
        }

        return DiffResult.WithChildren(path, diffs);
    }

    static bool IsExcluded(string memberPath, DiffOptions opts)
        => opts.ExcludeMembers.Contains(memberPath);

    static bool IsIncluded(string memberPath, DiffOptions opts)
    {
        if (opts.IncludeOnlyMembers.Count == 0) return true;
        return opts.IncludeOnlyMembers.Any(m =>
            memberPath.StartsWith(m + ".") || memberPath == m || m.StartsWith(memberPath + "."));
    }

    static bool IsFloatingPoint(object value)
        => value is double or float or decimal;

    static bool OverridesEquals(Type type)
    {
        if (type == typeof(object)) return false;
        if (IsAnonymousType(type)) return false;
        var equalsMethod = type.GetMethod("Equals", [typeof(object)]);
        return equalsMethod?.DeclaringType == type;
    }

    static bool IsAnonymousType(Type type)
    {
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
            && (type.Name.Contains("AnonymousType") || type.Name.Contains("AnonType"))
            && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
            && type.Attributes.HasFlag(TypeAttributes.NotPublic);
    }

    static bool IsRecord(Type type)
    {
        return type.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance) is not null;
    }

    static string Stringify(object? value)
    {
        if (value is null) return "null";
        if (value is Enum e) return e.ToString();
        try { return value.TooString(); }
        catch { return value.ToString() ?? "null"; }
    }

    static bool HasElements(IEnumerable enumerable)
    {
        var enumerator = enumerable.GetEnumerator();
        try { return enumerator.MoveNext(); }
        finally { (enumerator as IDisposable)?.Dispose(); }
    }
}
