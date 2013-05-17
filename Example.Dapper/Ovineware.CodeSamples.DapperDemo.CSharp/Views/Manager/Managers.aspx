<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Manager>>" %>

<%@ Import Namespace="Ovineware.CodeSamples.DapperDemo.CSharp.Models" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Managers</title>
</head>
<body>
    <div>
        <% using (Html.BeginForm())
           { %>
        <label for="employeeId">
            Employee:</label>&nbsp;<%= Html.DropDownList("employeeId", ViewData["Employees"] as IEnumerable<SelectListItem>) %>&nbsp;<input
                type="submit" value="Get Managers" />
        <%} %>
    </div>
    <div>
        <% if (Model != null && Model.Count() > 0)
           { %>
        <h2>
            Managers</h2>
        <table>
            <thead>
                <tr>
                    <th>
                        First Name
                    </th>
                    <th>
                        Last Name
                    </th>
                </tr>
            </thead>
            <tbody>
                <% foreach (Manager manager in Model)
                   { %>
                <tr>
                    <td>
                        <%: manager.ManagerFirstName %>
                    </td>
                    <td>
                        <%: manager.ManagerLastName %>
                    </td>
                </tr>
                <%} %>
            </tbody>
        </table>
        <%} %>
    </div>
</body>
</html>
