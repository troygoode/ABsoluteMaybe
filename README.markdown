# What is this?
An [A/B testing](http://en.wikipedia.org/wiki/A/B_testing) library for ASP.Net / ASP.Net MVC based on the Rails [A/Bingo](http://www.bingocardcreator.com/abingo) library.

# Example
Let's say your boss comes up to you and says "Hey, I think we should replace the 'Register' link on our homepage with this sweet animated GIF my daughter created in her middle school's web design class. It'll attract more attention and get us more users!" You of course immediately suspect that it will drive away far more users than it will attract, but how do you know for sure? Simple, we A/B test it!

First, let's go ahead and add the blinking button image to our homepage...

*Views/Home/Index.aspx:*
<pre>
&lt;% if(ABsoluteMaybe.Test("huge_animated_register_button")){ %&gt;
	%lt;a href="&lt;% =Url.Action("Register", "Account) %&gt;"&gt;&lt;img src="/Content/HuuuuugeAnimatedRegisterButton.gif" /&gt;&lt;/a&gt;
&lt;% }else{ %&gt;
	&lt;: Html.ActionLink("Click here to register!", "Register", "Account") &gt;
&lt;% } %&gt;
</pre>

You see that when we added it we kept our existing link there as well, and wrapped both in an if-statement. The condition for this if-statement is **ABsoluteMaybe.Test("huge_animated_register_button_")**, which tells ABsoluteMaybe that we want to experiment with two options inside the if-statement and track how many people click each using the key *"huge_animated_button"*.

How does ABsoluteMaybe know when the user has actually click the button? For that we'll need to add a bit of code to our Register action...

*Controllers/AccountController.cs:*
<pre>
public ActionResult Register(){
	ABsoluteMaybe.Convert("huge_animated_register_button");
	return View();
}
</pre>

# Getting Started
...

# Contributing
...

# Credits
ABsoluteMaybe is modeled closely after [Patrick McKenzie](http://twitter.com/patio11)'s [A/Bingo](http://www.bingocardcreator.com/abingo) library for Ruby on Rails. Some code (namely the z-score/p-value calculation) was ported from A/Bingo to .Net by [Jason Kester](http://twitter.com/#!/jasonkester) for his [FairlyCertain](http://www.fairtutor.com/fairlycertain/) library.

# License
ABsoluteMaybe is released under the [MIT License](http://en.wikipedia.org/wiki/MIT_License).