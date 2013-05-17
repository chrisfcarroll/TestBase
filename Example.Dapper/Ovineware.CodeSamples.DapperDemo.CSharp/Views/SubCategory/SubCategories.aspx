<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<SubCategory>>" %>

<%@ Import Namespace="Ovineware.CodeSamples.DapperDemo.CSharp.Models" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sub-Categories</title>
</head>
<body>
    <h1>
        Sub-Categories</h1>
    <div>
        <%: Html.ActionLink("Create new sub-category", "New") %></div>
    <table>
        <tr>
            <th>
                Name
            </th>
            <th>
                Last Modified
            </th>
            <th>
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>            
            <td>
                <%= Html.ActionLink(item.Name, "Update", new { Id = item.Id }) %>
            </td>
            <td>
                <%: String.Format("{0:g}", item.ModifiedOn) %>
            </td>
            <td><%= Html.ActionLink("Delete", "Delete", new { Id = item.Id }) %></td>
        </tr>
        <% } %>
    </table>
</body>
</html>
