Imports System.Web.Mvc

Namespace DevExpressMvcApplication1.Controllers

    Public Class HomeController
        Inherits Controller

        Public Function Index() As ActionResult
            ViewBag.Message = "Welcome to DevExpress Extensions for ASP.NET MVC!"
            ViewData("selectedRows") = New Integer() {1, 5, 9, 4, 11, 17, 34, 77}
            Return View(GetProducts())
        End Function

        Public Function InlineEditingPartial() As ActionResult
            Return PartialView("GridView", GetEditableProducts())
        End Function
    End Class
End Namespace
