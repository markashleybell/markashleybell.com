<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<markashleybell.com.Web.Models.ArticleViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit</h2>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>
        
        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.ArticleID) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.ArticleID) %>
                <%: Html.ValidationMessageFor(model => model.ArticleID) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Title) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Title) %>
                <%: Html.ValidationMessageFor(model => model.Title) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Author) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Author) %>
                <%: Html.ValidationMessageFor(model => model.Author) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Summary) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Summary) %>
                <%: Html.ValidationMessageFor(model => model.Summary) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.SummaryMarkdown) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.SummaryMarkdown) %>
                <%: Html.ValidationMessageFor(model => model.SummaryMarkdown) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Body) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Body) %>
                <%: Html.ValidationMessageFor(model => model.Body) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.BodyMarkdown) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.BodyMarkdown) %>
                <%: Html.ValidationMessageFor(model => model.BodyMarkdown) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Slug) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Slug) %>
                <%: Html.ValidationMessageFor(model => model.Slug) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Published) %>
            </div>
            <div class="editor-field">
                <%: Html.EditorFor(model => model.Published, Model.Published) %>
                <%: Html.ValidationMessageFor(model => model.Published) %>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Updated) %>
            </div>
            <div class="editor-field">
                <%: Html.EditorFor(model => model.Updated, Model.Updated)%>
                <%: Html.ValidationMessageFor(model => model.Updated) %>
            </div>
            
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

