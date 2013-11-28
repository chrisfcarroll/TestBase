using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace TestBase.Tests.ComparerEqualsByValueTests.Example
{
    public enum StatusEnum
    {
        Invalid = 0,
        ClientOk = 1,
        NotProceeding
    }

    public class SearchBox
    {
        public string NameOrId { get; set; }

        public string Datum1 { get; set; }

        public StatusEnum Enum1 { get; set; }

        public static implicit operator SearchBox(string query)
        {
            var result = new SearchBox();
            var clauses = query.Split(new[] { "&", " and " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var clause in clauses)
            {
                var parts = clause.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Count() != 2)
                {
                    result.NameOrId = parts[0].Replace("'", "''");
                }
                else
                {
                    switch (parts[0])
                    {
                        case "Name":
                            result.NameOrId = parts[1].Replace("'", "''");
                            break;

                        case "Campaign":
                        case "Datum1":
                        case "Campaign.Name":
                            result.Datum1 = parts[1].Replace("'", "''");
                            break;

                        case "StatusEnum":
                            result.Enum1 = (StatusEnum)Enum.Parse(typeof(StatusEnum), parts[1].Replace("'", "''"), true);
                            break;

                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            var emptyValues = new object[] { null, default(StatusEnum) };

            var filterInWords = string.Join(" & ",
                                            GetType()
                                                .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                                                .Select(p =>
                                                    {
                                                        var value = p.GetValue(this, null);
                                                        return !emptyValues.Contains(value)
                                                                   ? string.Format("{0}={1}", p.Name, value)
                                                                   : null;
                                                    })
                                                .Where(s => s != null)
                );

            return string.IsNullOrWhiteSpace(filterInWords) ? "All Clients" : filterInWords;
        }
    }
}