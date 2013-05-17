<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<SubCategory>" %>

<%@ Import Namespace="Ovineware.CodeSamples.DapperDemo.CSharp.Models" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <%bool isUpdate = ViewData["IsUpdate"] != null ? (bool)ViewData["IsUpdate"] : false;
      Page.Title = isUpdate ? String.Format("Update {0}", Model.Name) : "New Sub-Category";%>
    <title>
        <%: Page.Title %></title>
</head>
<body>
    <h1>
        <%: Page.Title %></h1>
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <%bool isUpdate = ViewData["IsUpdate"] != null ? (bool)ViewData["IsUpdate"] : false;
      IEnumerable<SelectListItem> categories = ViewData["Categories"] as IEnumerable<SelectListItem>;%>
    <fieldset>
        <legend>Fields</legend>
        <%: Html.HiddenFor(model => model.Id) %>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.CategoryId) %>
        </div>
        <div class="editor-field">
            <%: Html.DropDownListFor(model => model.CategoryId, categories) %>
            <%: Html.ValidationMessageFor(model => model.CategoryId) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Name) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Name) %>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <p>
            <input type="submit" value="<%= isUpdate ? "Update" : "Create" %>" />
        </p>
    </fieldset>
    <% } %>
    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>
</body>
</html>
