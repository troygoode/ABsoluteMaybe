<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ABsoluteMaybe.SampleWebsite.MVC2.Controllers.HomeController.ShoppingCartViewModel>" %>

<asp:Content ID="title" ContentPlaceHolderID="TitleContent" runat="server">
	Shopping Cart
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="MainContent" runat="server">

	<h2>Shopping Cart</h2>
	<% using (Html.BeginForm()){ %>
	<fieldset>
		<dl>
			<dt>Bumper Sticker:</dt>
			<dd><%: string.Format("{0:c}", Model.BumperStickerCost)%></dd>
		</dl>
		<dl>
			<dt>Shipping Cost:</dt>
			<dd><%: string.Format("{0:c}", Model.ShippingCost)%></dd>
		</dl>
		<dl>
			<dt>Total:</dt>
			<dd><%: string.Format("{0:c}", Model.BumperStickerCost + Model.ShippingCost)%></dd>
		</dl>

		<%: Html.AntiForgeryToken() %>
		<%: Html.HttpMethodOverride(HttpVerbs.Put) %>
		<input type="submit" value="Complete Purchase" />
	</fieldset>
	<% } %>

</asp:Content>