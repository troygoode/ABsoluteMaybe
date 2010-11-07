<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="ABsoluteMaybe" %>

<asp:Content ID="title" ContentPlaceHolderID="TitleContent" runat="server">
	Home Page
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="MainContent" runat="server">
	<h2><%: ViewData["Message"] %></h2>
	
	<p>To learn more about ABsoluteMaybe visit <a href="https://github.com/TroyGoode/ABsoluteMaybe" title="GitHub">https://github.com/TroyGoode/ABsoluteMaybe</a>.</p>

	<p>Want to impress all of your friends? <%: Html.ActionLink("Buy an \"ABsoluteMaybe\" bumper stick for your kids' tricycle today!", "ShoppingCart") %></p>

	<% if (ABsoluteMaybe.Test("Begging_Totally_Works")){ %>
		<%: Html.ActionLink("Click me, CLICK ME! Please, please, pleeeeease click me (please)?", "BeggingTotallyWorks")%>
	<% }else{ %>
		<%: Html.ActionLink("Click this if you feel like it. I don't really care. Meh.", "BeggingTotallyWorks")%>
	<% } %>
</asp:Content>