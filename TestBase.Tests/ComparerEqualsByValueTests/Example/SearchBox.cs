using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace TestBase.Tests.ComparerEqualsByValueTests.Example
{
    public enum ClientStatus
    {
        Invalid = 0,
        ClientOk = 1,
        NotProceeding
    }

    public enum CaseStatus
    {
        Invalid = 0,
        CaseOk = 2,
        Pending
    }

    public class SearchBox
    {
        [DisplayName("Search by Name or Id")]
        public string NameOrId { get; set; }

        [DisplayName("Campaign Name")]
        public string CampaignName { get; set; }

        [DisplayName("Client Status")]
        public ClientStatus ClientStatus { get; set; }

        [DisplayName("Case Status")]
        public CaseStatus CaseStatus { get; set; }

        public int? PdmsIdIfNumeric
        {
            get
            {
                int result;
                return !string.IsNullOrWhiteSpace(NameOrId) && int.TryParse(NameOrId, out result)
                           ? (int?)result
                           : null;
            }
        }


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
                        case "CampaignName":
                        case "Campaign.Name":
                            result.CampaignName = parts[1].Replace("'", "''");
                            break;

                        case "ClientStatus":
                            result.ClientStatus = (ClientStatus)Enum.Parse(typeof(ClientStatus), parts[1].Replace("'", "''"), true);
                            break;

                        case "CaseStatus":
                            result.CaseStatus = (CaseStatus)Enum.Parse(typeof(CaseStatus), parts[1].Replace("'", "''"), true);
                            break;
                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            var emptyValues = new object[] { null, default(CaseStatus), default(ClientStatus) };

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