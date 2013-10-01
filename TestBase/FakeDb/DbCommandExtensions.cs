using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace TestBase.FakeDb
{
    public static class DbCommandExtensions
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> fakeData)
        {
            return fakeData.Cast<object>().ToDataTable(typeof(T));
        }

        public static DataTable ToDataTable(this IEnumerable<object> fakeData, Type pocoTypeToReturn)
        {
            var t = new DataTable();
            var propertyInfos = pocoTypeToReturn.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance);

            foreach (var property in propertyInfos)
            {
                t.Columns.Add(new DataColumn(property.Name, property.PropertyType));
            }

            foreach (var row in fakeData)
            {
                var items = new object[propertyInfos.Length];
                for (var i = 0; i < items.Length; i++)
                {
                    items[i] = propertyInfos[i].GetValue(row, null);
                }
                var dataRow = propertyInfos.Select(x => x.GetValue(row, null)).ToArray();

                t.LoadDataRow(dataRow, true);
            }
            return t;
        }
    }
}