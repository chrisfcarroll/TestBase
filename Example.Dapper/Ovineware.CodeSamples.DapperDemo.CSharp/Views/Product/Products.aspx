<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Product>>" %>

<%@ Import Namespace="Ovineware.CodeSamples.DapperDemo.CSharp.Models" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Products</title>
</head>
<body>
    <div>
        <h1>
            Products</h1>
        <div>
            <%= Html.ActionLink("Create new product", "New") %></div>
        <div>
            <table>
                <thead>
                    <tr>
                        <td>
                        </td>
                        <th>
                            Product Number
                        </th>
                        <th>
                            Name
                        </th>
                        <th>
                            List Price
                        </th>
                        <th>
                            Last Modified
                        </th>
                        <th>
                            Sub-category
                        </th>
                        <th>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <% foreach (Product product in Model)
                       { %>
                    <tr>
                        <td>
                            <img src="<%= Url.Action("Image", new { Id = product.ProductID, Size = ImageSize.Thumbnail }) %>"
                                alt="<%= product.Name %>" />
                        </td>
                        <td>
                            <%: Html.ActionLink(product.ProductNumber, "Update", new { Id = product.ProductID }) %>
                        </td>
                        <td>
                            <%: product.Name %>
                        </td>
                        <td>
                            <%: String.Format("{0:c}", product.ListPrice) %>
                        </td>
                        <td>
                            <%: product.ModifiedDate %>
                        </td>
                        <td>
                            <%= product.SubCategory != null ? product.SubCategory.Name : "<em>None</em>" %>
                        </td>
                        <td>
                            <%= Html.ActionLink("Delete", "Delete", new { Id = product.ProductID }) %>
                        </td>
                    </tr>
                    <%} %>
                </tbody>
            </table>
        </div>
    </div>
</body>
</html>
