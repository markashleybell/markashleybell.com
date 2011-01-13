<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<List<markashleybell.com.Web.Models.ArticleViewModel>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Articles</h1>

    <ul>

    <% foreach(var article in Model) { %>

    <li><a href="/articles/<%= article.Slug%>/"><%= article.Title%></a> <span><%= article.Published.ToString("dddd, d MMMM yyyy HH:mm")%></span> <span><%= article.SummaryMarkdown%></span></li>

    <% } %>

    </ul>

</asp:Content>