@Code
	ViewBag.Title = "Home Page"
End code
<h2>@ViewBag.Message</h2>
<p>
	To learn more about DevExpress Extensions for ASP.NET MVC visit <a href="http://devexpress.com/Products/NET/Controls/ASP-NET-MVC/"
		title="ASP.NET MVC Website">http://devexpress.com/Products/NET/Controls/ASP-NET-MVC/</a>.
</p>
@ModelType System.Collections.IEnumerable
@Html.Partial("GridView", Model)
