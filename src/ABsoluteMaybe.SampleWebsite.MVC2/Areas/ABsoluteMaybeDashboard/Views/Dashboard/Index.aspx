<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard.Models.DashboardIndexViewModel>" %>

<asp:Content ID="title" ContentPlaceHolderID="TitleContent" runat="server">
	ABsoluteMaybe Dashboard
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="MainContent" runat="server">

	<h2>ABsoluteMaybe Dashboard</h2>

	<ul>
	<% foreach (var experiment in Model.Experiments) { %>
		<li>
			<h3><%: experiment.Name %></h3>
			<table>
				<thead>
					<tr>
						<th>Option</th>
						<th>Participants</th>
						<th>Conversions</th>
					</tr>
				</thead>
				<tbody>
				<% foreach (var option in experiment.Options) { %>
					<tr>
						<td><%: option.Name %></td>
						<td><%: option.Participants %></td>
						<td><%: option.Conversions %> <%: option.ConversionRate == null ? "" : option.ConversionRate.Value.ToString("(#0.##%)") %></td>
					</tr>
				<% } %>
				</tbody>
				<tfoot class="total">
					<tr>
						<td>Total</td>
						<td><%: experiment.TotalParticipants %></td>
						<td><%: experiment.TotalConversions %></td>
					</tr>
				</tfoot>
			</table>
			<p><%: experiment.Results %></p>
		</li>
	<% } %>
	</ul>

</asp:Content>