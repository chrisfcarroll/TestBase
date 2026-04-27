#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using NUnit.Framework;
using NUnitAssert = NUnit.Framework.Assert;
using NUnitIs = NUnit.Framework.Is;
using NUnitDoes = NUnit.Framework.Does;

namespace TestBase.Tests.AssertionFailureDisplay;

[TestFixture]
public class AssertionComparisonReport
{
    record ComparisonResult(string Category, string Scenario, string TestBaseOutput, string NUnitOutput);

    static readonly List<ComparisonResult> Results = new();

    static string CaptureTestBaseFailure(Action action)
    {
        try { action(); return "[No failure — assertion passed]"; }
        catch (Exception ex) { return ex.Message; }
    }

    static string CaptureNUnitFailure(TestDelegate action)
    {
        var ex = NUnitAssert.Catch(action);
        return ex?.Message ?? "[No failure — assertion passed]";
    }

    void Compare(string category, string scenario, Action testBase, TestDelegate nunit)
    {
        var tb = CaptureTestBaseFailure(testBase);
        var nu = CaptureNUnitFailure(nunit);
        Results.Add(new(category, scenario, tb, nu));
        TestContext.Out.WriteLine($"=== {category}: {scenario} ===");
        TestContext.Out.WriteLine($"TestBase:\n{tb}\n");
        TestContext.Out.WriteLine($"NUnit:\n{nu}\n");
    }

    // ── Equality ──

    [Test]
    public void Equality_Int()
    {
        Compare("Equality", "int 1 vs 2",
            () => 1.ShouldBe(2),
            () => NUnitAssert.That(1, NUnitIs.EqualTo(2)));
    }

    [Test]
    public void Equality_String()
    {
        Compare("Equality", "string 'hello' vs 'world'",
            () => "hello".ShouldBe("world"),
            () => NUnitAssert.That("hello", NUnitIs.EqualTo("world")));
    }

    [Test]
    public void Equality_StringDiffAtEnd()
    {
        Compare("Equality", "string 'abcdef' vs 'abcXYZ'",
            () => "abcdef".ShouldBe("abcXYZ"),
            () => NUnitAssert.That("abcdef", NUnitIs.EqualTo("abcXYZ")));
    }

    [Test]
    public void Equality_LongString()
    {
        var actual = "The quick brown fox jumps over the lazy dog";
        var expected = "The quick brown fox leaps over the lazy dog";
        Compare("Equality", "long strings differing mid-sentence",
            () => actual.ShouldBe(expected),
            () => NUnitAssert.That(actual, NUnitIs.EqualTo(expected)));
    }

    [Test]
    public void Equality_Object()
    {
        var actual = new { Name = "Alice", Age = 30 };
        var expected = new { Name = "Alice", Age = 25 };
        Compare("Equality", "anonymous objects differing on Age",
            () => actual.ShouldBe(expected),
            () => NUnitAssert.That(actual, NUnitIs.EqualTo(expected)));
    }

    // ── Null ──

    [Test]
    public void Null_ShouldNotBeNull()
    {
        string s = null;
        Compare("Null", "null ShouldNotBeNull",
            () => s.ShouldNotBeNull(),
            () => NUnitAssert.That(s, NUnitIs.Not.Null));
    }

    [Test]
    public void Null_ShouldBeNull()
    {
        Compare("Null", "'hello' ShouldBeNull",
            () => "hello".ShouldBeNull(),
            () => NUnitAssert.That("hello", NUnitIs.Null));
    }

    // ── Boolean ──

    [Test]
    public void Boolean_ShouldBeTrue()
    {
        Compare("Boolean", "false ShouldBeTrue",
            () => false.ShouldBeTrue(),
            () => NUnitAssert.That(false, NUnitIs.True));
    }

    [Test]
    public void Boolean_ShouldBeFalse()
    {
        Compare("Boolean", "true ShouldBeFalse",
            () => true.ShouldBeFalse(),
            () => NUnitAssert.That(true, NUnitIs.False));
    }

    // ── Comparison ──

    [Test]
    public void Comparison_GreaterThan()
    {
        Compare("Comparison", "1 ShouldBeGreaterThan 5",
            () => 1.ShouldBeGreaterThan(5),
            () => NUnitAssert.That(1, NUnitIs.GreaterThan(5)));
    }

    [Test]
    public void Comparison_LessThan()
    {
        Compare("Comparison", "10 ShouldBeLessThan 3",
            () => 10.ShouldBeLessThan(3),
            () => NUnitAssert.That(10, NUnitIs.LessThan(3)));
    }

    // ── Type ──

    [Test]
    public void Type_ShouldBeOfType()
    {
        object val = "hello";
        Compare("Type", "string ShouldBeOfType<int>",
            () => val.ShouldBeOfType<int>(),
            () => NUnitAssert.That(val, NUnitIs.TypeOf<int>()));
    }

    // ── String ──

    [Test]
    public void String_ShouldContain()
    {
        Compare("String", "'hello world' ShouldContain 'xyz'",
            () => "hello world".ShouldContain("xyz"),
            () => NUnitAssert.That("hello world", NUnitDoes.Contain("xyz")));
    }

    [Test]
    public void String_ShouldStartWith()
    {
        Compare("String", "'hello world' ShouldStartWith 'world'",
            () => "hello world".ShouldStartWith("world"),
            () => NUnitAssert.That("hello world", NUnitDoes.StartWith("world")));
    }

    [Test]
    public void String_ShouldMatch()
    {
        Compare("String", "'abc123' ShouldMatch '^[0-9]+$'",
            () => "abc123".ShouldMatch("^[0-9]+$"),
            () => NUnitAssert.That("abc123", NUnitDoes.Match("^[0-9]+$")));
    }

    // ── Collection ──

    [Test]
    public void Collection_ShouldContain()
    {
        var list = new[] { 1, 2, 3 };
        Compare("Collection", "[1,2,3] ShouldContain 5",
            () => list.ShouldContain(5),
            () => NUnitAssert.That(list, NUnitDoes.Contain(5)));
    }

    [Test]
    public void Collection_ShouldBeEmpty()
    {
        var list = new[] { 1, 2, 3 };
        Compare("Collection", "[1,2,3] ShouldBeEmpty",
            () => ((System.Collections.IEnumerable)list).ShouldBeEmpty(),
            () => NUnitAssert.That(list, NUnitIs.Empty));
    }

    [Test]
    public void Collection_ShouldNotBeEmpty()
    {
        var list = Array.Empty<int>();
        Compare("Collection", "[] ShouldNotBeEmpty",
            () => list.ShouldNotBeEmpty(),
            () => NUnitAssert.That(list, NUnitIs.Not.Empty));
    }

    // ── EqualByValue (TestBase-specific) ──

    [Test]
    public void EqualByValue_DifferentProperty()
    {
        var actual = new { Name = "Alice", Age = 30 };
        var expected = new { Name = "Alice", Age = 25 };
        Compare("EqualByValue", "objects differing on Age",
            () => actual.ShouldEqualByValue(expected),
            () =>
            {
                NUnitAssert.That(actual.Name, NUnitIs.EqualTo(expected.Name));
                NUnitAssert.That(actual.Age, NUnitIs.EqualTo(expected.Age));
            });
    }

    // ── Report generation ──

    [OneTimeTearDown]
    public void WriteReport()
    {
        if (Results.Count == 0) return;

        var sb = new StringBuilder();
        sb.AppendLine("# Assertion Comparison Report: TestBase vs NUnit");
        sb.AppendLine();
        sb.AppendLine($"Generated: {DateTime.UtcNow:u}");
        sb.AppendLine();

        foreach (var r in Results)
        {
            sb.AppendLine($"## {r.Category}: {r.Scenario}");
            sb.AppendLine();
            sb.AppendLine("### TestBase");
            sb.AppendLine("```");
            sb.AppendLine(r.TestBaseOutput);
            sb.AppendLine("```");
            sb.AppendLine();
            sb.AppendLine("### NUnit");
            sb.AppendLine("```");
            sb.AppendLine(r.NUnitOutput);
            sb.AppendLine("```");
            sb.AppendLine();
        }

        var dir = Path.GetDirectoryName(typeof(AssertionComparisonReport).Assembly.Location)!;
        var path = Path.Combine(dir, "AssertionComparisonReport.md");
        File.WriteAllText(path, sb.ToString());
        TestContext.Out.WriteLine($"\nReport written to: {path}");
    }

    // ── 4.3: LLM Review ──

    [Test, Explicit("Requires ANTHROPIC_API_KEY environment variable")]
    public async System.Threading.Tasks.Task LlmReviewOfReport()
    {
        var dir = Path.GetDirectoryName(typeof(AssertionComparisonReport).Assembly.Location)!;
        var reportPath = Path.Combine(dir, "AssertionComparisonReport.md");
        if (!File.Exists(reportPath))
        {
            NUnitAssert.Fail($"Report not found at {reportPath}. Run the other tests in this fixture first.");
            return;
        }

        var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            NUnitAssert.Ignore("ANTHROPIC_API_KEY not set — skipping LLM review.");
            return;
        }

        var report = await File.ReadAllTextAsync(reportPath);

        var prompt = """
            You are reviewing a comparison report of two .NET assertion libraries: TestBase and NUnit.
            For each assertion comparison, provide a factual, terse verdict:
            - Which output is clearer and more helpful for diagnosing the failure?
            - If NUnit is clearly better, state tersely what TestBase should change to improve.
            - If TestBase is better, note what it does well.
            - If they are roughly equal, say so.

            Output your review as markdown, one section per comparison, keeping each verdict to 1-3 sentences.
            """;

        var requestBody = JsonSerializer.Serialize(new
        {
            model = "claude-sonnet-4-6",
            max_tokens = 4096,
            messages = new[]
            {
                new { role = "user", content = $"{prompt}\n\n{report}" }
            }
        });

        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("x-api-key", apiKey);
        http.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        var response = await http.PostAsync(
            "https://api.anthropic.com/v1/messages",
            new StringContent(requestBody, Encoding.UTF8, "application/json"));

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            NUnitAssert.Fail($"API call failed ({response.StatusCode}): {responseBody}");
            return;
        }

        using var doc = JsonDocument.Parse(responseBody);
        var reviewText = doc.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString();

        var reviewPath = Path.Combine(dir, "AssertionComparisonReport.LlmReview.md");
        var reviewContent = $"# LLM Review of Assertion Comparison Report\n\n" +
                            $"Reviewed by: claude-sonnet-4-6\n" +
                            $"Date: {DateTime.UtcNow:u}\n\n" +
                            reviewText;

        await File.WriteAllTextAsync(reviewPath, reviewContent);
        TestContext.Out.WriteLine($"\nLLM review written to: {reviewPath}");
        TestContext.Out.WriteLine(reviewText);
    }
}
#endif
