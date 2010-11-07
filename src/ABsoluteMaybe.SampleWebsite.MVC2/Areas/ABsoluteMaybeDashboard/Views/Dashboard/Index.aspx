<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard.Models.DashboardIndexViewModel>" %>

<asp:Content ID="title" ContentPlaceHolderID="TitleContent" runat="server">
	ABsoluteMaybe Dashboard
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="MainContent" runat="server">

	<h2>ABsoluteMaybe Dashboard</h2>
	<% if (TempData.ContainsKey("Flash")) { %>
		<p class="flash"><%: TempData["Flash"] %></p>
	<% } %>

	<ul>
	<% foreach (var experiment in Model.Experiments) { %>
		<li>
			<h3><%: experiment.Name %></h3>
			<p>
				Experiment started <%: experiment.Started.ToLocalTime().ToShortDateString() %> <%: experiment.Started.ToLocalTime().ToShortTimeString() %>.
				<% if (experiment.Ended != null) { %>
				Experiment ended <%: experiment.Ended.Value.ToLocalTime().ToShortDateString() %> <%: experiment.Ended.Value.ToLocalTime().ToShortTimeString() %>.
				<% } %>
			</p>
			<table>
				<thead>
					<tr>
						<th>Option</th>
						<th>Participants</th>
						<th>Conversions</th>
						<th></th>
					</tr>
				</thead>
				<tbody>
				<% foreach (var option in experiment.Options) { %>
					<tr>
						<td><%: option.Name %></td>
						<td><%: option.Participants %></td>
						<td><%: option.Conversions %> <%: option.ConversionRate == null ? "" : option.ConversionRate.Value.ToString("(#0.##%)") %></td>
						<td>
							<% if (!experiment.IsEnded) { %>
								<% using (Html.BeginForm("EndExperiment", "Dashboard", new{area = "ABsoluteMaybeDashboard"})) { %>
									<%: Html.Hidden("experiment", experiment.Name) %>
									<%: Html.Hidden("option", option.Name) %>
									<%: Html.AntiForgeryToken() %>
									<input type="submit" value="End experiment, picking this." />
								<% } %>
							<% }else if(option.IsAlwaysUseOption){ %>
							All users now see this option.
							<% } %>
						</td>
					</tr>
				<% } %>
				</tbody>
				<tfoot class="total">
					<tr>
						<th>Total</th>
						<th><%: experiment.TotalParticipants %></th>
						<th><%: experiment.TotalConversions %></th>
						<th></th>
					</tr>
				</tfoot>
			</table>
			<p><%: experiment.Results %></p>
		</li>
	<% } %>
	</ul>

</asp:Content>