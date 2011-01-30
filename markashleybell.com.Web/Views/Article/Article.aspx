﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<markashleybell.com.Web.Models.ArticleViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Article
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <h1><%= Model.Title %></h1>
    
    <p><%= Model.Published.ToString("dddd, d MMMM yyyy HH:mm")%></p>

    <%= Model.BodyMarkdown %>
    
    <% if (Model.Comments.Count > 0) { %>
    <div id="comments">
        <h2>Comments</h2>
        <% foreach (CommentViewModel c in Model.Comments) { %>
        <h3><%=((c.Url == "") ? c.AuthorName : "<a href=\"" + c.Url + "\">" + c.AuthorName + "</a>")%> <%=c.Published.ToString("dd/MM/yyyy hh:mm")%></h3>
        <%=c.BodyMarkdown%>
        <% } %>
    </div>
    <% } %>

    <% using(Html.BeginForm("Article", "Article", FormMethod.Post, new { id="commentform" })) { %>

    <h2>Add your comment</h2>

    <p><%= Html.LabelFor(x => x.NewComment.AuthorName) %><br />
    <%= Html.TextBoxFor(x => x.NewComment.AuthorName) %>
    <%= Html.ValidationMessageFor(x => x.NewComment.AuthorName)%></p>

    <p><%= Html.LabelFor(x => x.NewComment.Email) %><br />
    <%= Html.TextBoxFor(x => x.NewComment.Email)%>
    <%= Html.ValidationMessageFor(x => x.NewComment.Email)%></p>

    <p><%= Html.LabelFor(x => x.NewComment.Url) %><br />
    <%= Html.TextBoxFor(x => x.NewComment.Url)%></p>

    <p><%= Html.LabelFor(x => x.NewComment.Body) %><br />
    <%= Html.TextAreaFor(x => x.NewComment.Body)%>
    <%= Html.ValidationMessageFor(x => x.NewComment.Body)%></p>

    <p><label id="captcha" for="z7sfd602nlwi">If you have 2 octopi, how many tentacles will they have?</label> &nbsp;<input type="text" size="5" id="z7sfd602nlwi" name="z7sfd602nlwi" value="" /></p>

    <div class="clr">&nbsp;</div>

    <p><%= Html.HiddenFor(x => x.ArticleID)%>
    <%= Html.HiddenFor(x => x.Slug)%>
    <input id="commentsubmit" type="submit" value="Submit Comment" /></p>

    <% } %>

    <script type="text/javascript" src="/scripts/comment.js"></script>
    <script type="text/javascript">
        $('pre>code').addClass('prettyprint');
        prettyPrint();
    </script>

</asp:Content>