<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard.Models.ExperimentViewModel>>" %>

<asp:Content ID="title" ContentPlaceHolderID="TitleContent" runat="server">
	ABsoluteMaybe Dashboard
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="MainContent" runat="server">

	<h2>ABsoluteMaybe Dashboard</h2>

	<ul>
	<% foreach (var experiment in Model) { %>
		<li>
			<h3><%: experiment.Experiment.Name %></h3>
			<table>
				<thead>
					<tr>
						<th>Option</th>
						<th>Participants</th>
						<th>Conversions</th>
					</tr>
				</thead>
				<tbody>
				<% foreach (var option in experiment.Experiment.Options) { %>
					<tr>
						<td><%: option.Name %></td>
						<td><%: option.Participants %></td>
						<td><%: option.Conversions %> <%: option.Participants == 0 ? "" : (option.Conversions / option.Participants).ToString("(#0.##%)")%></td>
					</tr>
				<% } %>
				</tbody>
			</table>
			<p><%: experiment.Results %></p>
		</li>
	<% } %>
	</ul>

</asp:Content>