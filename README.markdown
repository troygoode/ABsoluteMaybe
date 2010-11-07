# What is this?
An [A/B testing](http://en.wikipedia.org/wiki/A/B_testing) library for ASP.Net / ASP.Net MVC based on the [A/Bingo](http://www.bingocardcreator.com/abingo) library for Rails.

# Example
Let's say your boss comes up to you and says "Hey, I think we should replace the 'Register' link on our homepage with this sweet animated GIF my daughter created in her middle school's web design class. It'll attract more attention and get us more users!" You of course immediately suspect that it will drive away far more users than it will attract, but how do you know for sure? Simple, we A/B test it!

First, let's go ahead and add the blinking button image to our homepage...

*Views/Home/Index.aspx:*
<pre>
&lt;% if(ABsoluteMaybe.Test("huge_animated_register_button")){ %&gt;
	&lt;a href="&lt;%= Url.Action("Register", "Account) %&gt;"&gt;&lt;img src="/Content/HuuuuugeAnimatedRegisterButton.gif" /&gt;&lt;/a&gt;
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

That is all we had to do to create our A/B test. Now we push the code to production and wait a while for the results to roll in. A couple days later we check out the included ABsoluteMaybe dashboard and look at the status of our test:

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
		<td>105</td>
		<td>16 (15.24%)</td>
		<td>[End experiment, picking this.]</td>
	</tr>
	<tr>
		<td>False</td>
		<td>94</td>
		<td>32 (34.04%)</td>
		<td>[End experiment, picking this.]</td>
	</tr>
	<tr>
		<td><strong>Total</strong></td>
		<td><strong>199</strong></td>
		<td><strong>48</strong></td>
		<td> </td>
	</tr>
</table>
*The best option you have is [False], which had 32 conversion from 94 participants (34.04%). The other option was [True], which had 16 conversion from 105 participants (15.24%). This difference is 99% likely to be statistically significant.*

Woohoo! The annoying blinking button was the loser after all. You print the dashboard page out and show it to your boss, who asks for you to switch back to the 'Register' link as quickly as possible. You tell him that might take a few hours, walk back to your desk and click "[End experiment, picking this.]" on the "False" option of the experiment (causing all users to see the false option of the experiment - i.e.: the Register link - from now on) and spend the rest of our day down at the local pub.

# Getting Started
*Coming soon.*

# Additional Examples

## Conversion Keywords
Sometimes you want to mark multiple experiments as having a conversion at once. For example, you might have two experiments on two different pages that both are intended to increase the number of users that make it to your purchase funnel: one that shows a button on your site's sidebar and one that shows a different button in a popup window...

*Views/Shared/Sidebar.aspx*
<pre>
&lt;% if(ABsoluteMaybe.Test("sidebar_checkout_button", "LoadedCheckoutScreen")){%&gt;...&lt;% } %&gt;
</pre>

*Views/Shared/CheckoutPopupWindow.aspx*
<pre>
&lt;% if(ABsoluteMaybe.Test("popup_checkout_button", "LoadedCheckoutScreen")){%&gt;...&lt;% } %&gt;
</pre>

In the above two tests we've not only supplied the name of the experiment, but also tied the experiments to a conversion keyword ("LoadedCheckoutScreen"). On the checkout action we can now add the following code to convert both experiments:

*Controllers/Checkout.cs*
<pre>
public ActionResult Checkout()
{
	ABsoluteMaybe.Convert("LoadedCheckoutScreen");
	return View();
}
</pre>

*Note: Running multiple experiments at once that interact with the same or closely related systems may cause you to receive less accurate results. For more information ask your local friendly statistician.*

## Non-boolean Options

By default an ABsoluteMaybe experiment is created with two options: true and false. You can also pass an array of values into the **.Test** method to use your own values instead. In this case we're testing to see if free shipping will improve our sales funnel. Note that in this case the return value of **.Test** is now a decimal rather than the usual boolean, and we were able to pass it right down to the view via our viewmodel.

*Controllers/Checkout.cs*
<pre>
public ActionResult Checkout()
{
	return View(new CheckoutViewModel{
		ProductsInCart = _shoppingCartRepository.FindByUser(_userId),
		ShippingCost = ABsoluteMaybe.Test("shipping_costs", new[]{9.99m, 0m})
	});
}
</pre>

Converting non-boolean options works the same as any other:

*Controllers/Checkout.cs*
<pre>
public ActionResult CheckoutPost(CheckoutViewModel form)
{
	ABsoluteMaybe.Convert("shipping_costs");
	//snip
	return RedirectToAction("ThankYou", "Checkout");
}
</pre>

## More Than Two Options

You can pass more than two values to the **.Test** method, but *please* note that doing so means that you will not receive a likelihood of statistical significance report on the Dashboard because math is hard.

<pre>
var shippingCost = ABsoluteMaybe.Test("shipping_costs", new[]{9.99m, 5.99m, 0m})
</pre>

# Contributing
Contributions are welcome, fork away.

# Credits
ABsoluteMaybe is modeled closely after [Patrick McKenzie](http://twitter.com/patio11)'s [A/Bingo](http://www.bingocardcreator.com/abingo) library for Ruby on Rails. Some code (namely the z-score/p-value calculation) was ported from A/Bingo to .Net by [Jason Kester](http://twitter.com/#!/jasonkester) for his [FairlyCertain](http://www.fairtutor.com/fairlycertain/) library.

# License
ABsoluteMaybe is released under the [MIT License](http://en.wikipedia.org/wiki/MIT_License).