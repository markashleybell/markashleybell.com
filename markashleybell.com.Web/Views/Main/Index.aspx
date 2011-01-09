﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Hi! My name's Mark.</h1>
    <div id="home">
        
        <p>Computers like me. Well, most of them, anyway—and even if we don’t get on at first, I usually win them over.</p>
        <p>I speak fluent C#, Python, SQL, JavaScript, jQuery and classic ASP (and also some conversational PHP).</p> 

		<ul>
			<li><a onclick="pageTracker._trackEvent('Outgoing Links', 'Click', 'Eclectica');" href="http://eclectica.co.uk/">Eclectica (my design and visual arts blog)</a> &raquo;</li>
			<li><a onclick="pageTracker._trackEvent('Outgoing Links', 'Click', 'Stack Overflow');" href="http://stackoverflow.com/users/43140/mark-bell">My Stack Overflow profile</a> &raquo;</li>
			<li><a onclick="pageTracker._trackEvent('Outgoing Links', 'Click', 'LinkedIn');" href="http://www.linkedin.com/in/markashleybell">My LinkedIn profile</a> &raquo;</li>
			<li><a onclick="pageTracker._trackEvent('Outgoing Links', 'Click', 'Twitter');" href="http://twitter.com/markashleybell">My Twitter account</a> &raquo;</li>

		</ul>
    
		<h2>If you're looking for my jQuery plugins...</h2>
		<p>Here they are: <a href="/jquery/">jQuery plugins</a>. They're all free and open source, but I'd be interested to know how you're using them - please let me know!</p>
        <p>Drop me a mail at the address below:</p>
		<p id="e">Turn On Javascript To Contact Me!</p>

        <script type="text/javascript" src="/Scripts/e.js"></script>

    </div>
    <div id="twitter">
        <script src="http://widgets.twimg.com/j/2/widget.js" type="text/javascript"></script>
        <script type="text/javascript">
            new TWTR.Widget({
                version: 2,
                type: 'profile',
                rpp: 5,
                interval: 6000,
                width: 300,
                height: 500,
                theme: {
                    shell: {
                        background: '#333333',
                        color: '#ffffff'
                    },
                    tweets: {
                        background: '#ffffff',
                        color: '#000000',
                        links: '#071aeb'
                    }
                },
                features: {
                    scrollbar: false,
                    loop: false,
                    live: false,
                    hashtags: true,
                    timestamp: true,
                    avatars: true,
                    behavior: 'all'
                }
            }).render().setUser('markashleybell').start();
        </script>
    </div>

</asp:Content>