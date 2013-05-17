<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Product>" %>

<%@ Import Namespace="Ovineware.CodeSamples.DapperDemo.CSharp.Models" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <%bool isUpdate = ViewData["IsUpdate"] != null ? (bool)ViewData["IsUpdate"] : false;
      Page.Title = isUpdate ? String.Format("Update {0}", Model.ProductNumber) : "New Product";%>
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
      IEnumerable<SelectListItem> measurementUnits = ViewData["MeasurementUnits"] as IEnumerable<SelectListItem>;
      IEnumerable<SelectListItem> subCategories = ViewData["SubCategories"] as IEnumerable<SelectListItem>;
      IEnumerable<SelectListItem> models = ViewData["Models"] as IEnumerable<SelectListItem>; %>
    <h1>
        <%: Page.Title %></h1>
    <fieldset>
        <legend>Fields</legend>
        <%: Html.HiddenFor(model=>model.ProductID) %>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Name) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Name) %>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.ProductNumber) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.ProductNumber) %>
            <%: Html.ValidationMessageFor(model => model.ProductNumber) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.MakeFlag) %>
        </div>
        <div class="editor-field">
            <%: Html.CheckBoxFor(model => model.MakeFlag) %>
            <%: Html.ValidationMessageFor(model => model.MakeFlag) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.FinishedGoodsFlag) %>
        </div>
        <div class="editor-field">
            <%: Html.CheckBoxFor(model => model.FinishedGoodsFlag) %>
            <%: Html.ValidationMessageFor(model => model.FinishedGoodsFlag) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Color) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Color) %>
            <%: Html.ValidationMessageFor(model => model.Color) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.SafetyStockLevel) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.SafetyStockLevel) %>
            <%: Html.ValidationMessageFor(model => model.SafetyStockLevel) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.ReorderPoint) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.ReorderPoint) %>
            <%: Html.ValidationMessageFor(model => model.ReorderPoint) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.StandardCost) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.StandardCost) %>
            <%: Html.ValidationMessageFor(model => model.StandardCost) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.ListPrice) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.ListPrice) %>
            <%: Html.ValidationMessageFor(model => model.ListPrice) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Size) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Size) %>
            <%: Html.ValidationMessageFor(model => model.Size) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.SizeUnitMeasureCode) %>
        </div>
        <div class="editor-field">
            <%: Html.DropDownListFor(model => model.SizeUnitMeasureCode, measurementUnits) %>
            <%: Html.ValidationMessageFor(model => model.SizeUnitMeasureCode) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.WeightUnitMeasureCode) %>
        </div>
        <div class="editor-field">
            <%: Html.DropDownListFor(model => model.WeightUnitMeasureCode, measurementUnits) %>
            <%: Html.ValidationMessageFor(model => model.WeightUnitMeasureCode) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Weight) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Weight) %>
            <%: Html.ValidationMessageFor(model => model.Weight) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.DaysToManufacture) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.DaysToManufacture) %>
            <%: Html.ValidationMessageFor(model => model.DaysToManufacture) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.ProductLine) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.ProductLine) %>
            <%: Html.ValidationMessageFor(model => model.ProductLine) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Class) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Class) %>
            <%: Html.ValidationMessageFor(model => model.Class) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Style) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Style) %>
            <%: Html.ValidationMessageFor(model => model.Style) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.ProductSubcategoryID) %>
        </div>
        <div class="editor-field">
            <%: Html.DropDownListFor(model => model.ProductSubcategoryID, subCategories) %>
            <%: Html.ValidationMessageFor(model => model.ProductSubcategoryID) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.ProductModelID) %>
        </div>
        <div class="editor-field">
            <%: Html.DropDownListFor(model => model.ProductModelID, models) %>
            <%: Html.ValidationMessageFor(model => model.ProductModelID) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.SellStartDate) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.SellStartDate) %>
            <%: Html.ValidationMessageFor(model => model.SellStartDate) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.SellEndDate) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.SellEndDate) %>
            <%: Html.ValidationMessageFor(model => model.SellEndDate) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.DiscontinuedDate) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.DiscontinuedDate) %>
            <%: Html.ValidationMessageFor(model => model.DiscontinuedDate) %>
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
