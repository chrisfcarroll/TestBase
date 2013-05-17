using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ovineware.CodeSamples.DapperDemo.CSharp.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, object> valueField, Func<T, object> textField)
        {
            return items.ToSelectListItems(valueField, textField, null);
        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, Func<T, object> valueField, Func<T, object> textField, Func<T, bool> isSelected)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (T item in items)
            {
                string value = valueField != null ? valueField(item).ToString() : item.ToString();
                string text = textField != null ? textField(item).ToString() : item.ToString();                
                bool selected = isSelected != null ? isSelected(item) : false;
                SelectListItem listItem = new SelectListItem() { Value = value, Text = text, Selected = selected };
                listItems.Add(listItem);
            }
            return listItems;
        }
    }
}