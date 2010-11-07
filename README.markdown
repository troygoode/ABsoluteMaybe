# What is this?
An [A/B testing](http://en.wikipedia.org/wiki/A/B_testing) library for ASP.Net / ASP.Net MVC based on the [A/Bingo](http://www.bingocardcreator.com/abingo) library for Rails.

# Example
Let's say your boss comes up to you and says "Hey, I think we should replace the 'Register' link on our homepage with this sweet animated GIF my daughter created in her middle school's web design class. It'll attract more attention and get us more users!" You of course immediately suspect that it will drive away far more users than it will attract, but how do you know for sure? Simple, we A/B test it!

First, let's go ahead and add the blinking button image to our homepage...

*Views/Home/Index.aspx:*
<pre>
&lt;% if(ABsoluteMaybe.Test("huge_animated_register_button")){ %&gt;
	&lt;a href="&lt;% =Url.Action("Register", "Account) %&gt;"&gt;&lt;img src="/Content/HuuuuugeAnimatedRegisterButton.gif" /&gt;&lt;/a&gt;
&lt;% }else{ %&gt;
	&lt;%: Html.ActionLink("Click here to register!", "Register", "Account") %&gt;
&lt;% } %&gt;
</pre>

You see that when we added it we kept our existing link there as well, and wrapped both in an if-statement. The condition for this if-statement is **ABsoluteMaybe.Test("huge_animated_register_button_")**, which tells ABsoluteMaybe that we want to experiment with two options inside the if-statement and track how many people click each using the key *"huge_animated_button"*.

How does ABsoluteMaybe know when the user has actually clicked the button or link? For that we'll need to add a bit of code to our Register action...

*Controllers/AccountController.cs:*
<pre>
public ActionResult Register(){
	ABsoluteMaybe.Convert("huge_animated_register_button");
	return View();
}
</pre>

You might've noticed that we don't have to tell ABsoluteMaybe whether the button or link was clicked. This is because when **ABsoluteMaybe.Test** was executed above, one of the two options was randomly assigned to the current user (who is tracked via a cookie by default). This also means that no matter how many times a user visits the homepage, they'll always see the same thing (in other words, refreshing the homepage won't alternate between the button and link).

That is all we had to do to create our A/B test. Now we push the code to production and wait a while for the results to roll in. A couple days later we check out the included ABsoluteMabye dashboard and look at the status of our test:

**Experiment: huge_animated_register_button**
<table>
	<tr>
		<td><strong>Option</strong></td>
		<td><strong>Participants</strong></td>
		<td><strong>Conversions</strong></td>
		<td> </td>
	</tr>
	<tr>
		<td>True</td>
		<td>100</td>
		<td>15 (15%)</td>
		<td>[End experiment, picking this.]</td>
	</tr>
	<tr>
		<td>False</td>
		<td>100</td>
		<td>30 (30%)</td>
		<td>[End experiment, picking this.]</td>
	</tr>
	<tr>
		<td><strong>Total</strong></td>
		<td><strong>200</strong></td>
		<td><strong>30</strong></td>
		<td> </td>
	</tr>
</table>
*The best option you have is [False], which had 30 conversion from 100 participants (30%). The other option was [True], which had 15 conversion from 100 participants (15%). This difference is 99% likely to be statistically significant.*

Woohoo! The annoying blinking button was the loser after all. You print the dashboard page out and show it to your boss, who asks for you to switch back to the 'Register' link as quickly as possible. You tell him that might take a few hours, walk back to your desk and click "[End experiment, picking this.]" on the "False" option of the experiment (causing all users to see the false option of the experiment - i.e.: the Register link - from now on) and spend the rest of our day down at the local pub.

# Getting Started
*Coming soon.*

# Contributing
Contributions are welcome, fork away.

# Credits
ABsoluteMaybe is modeled closely after [Patrick McKenzie](http://twitter.com/patio11)'s [A/Bingo](http://www.bingocardcreator.com/abingo) library for Ruby on Rails. Some code (namely the z-score/p-value calculation) was ported from A/Bingo to .Net by [Jason Kester](http://twitter.com/#!/jasonkester) for his [FairlyCertain](http://www.fairtutor.com/fairlycertain/) library.

# License
ABsoluteMaybe is released under the [MIT License](http://en.wikipedia.org/wiki/MIT_License).