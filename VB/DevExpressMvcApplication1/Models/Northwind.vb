Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.Data
Imports System.Data.Linq
Imports System.Linq
Imports System.Web
Imports System.Web.UI

Public Module NorthwindDataProvider

    Const NorthwindDataContextKey As String = "DXNorthwindDataContext"

    Public ReadOnly Property DB As NorthwindDataContext
        Get
            If System.Web.HttpContext.Current.Items(NorthwindDataProvider.NorthwindDataContextKey) Is Nothing Then System.Web.HttpContext.Current.Items(NorthwindDataProvider.NorthwindDataContextKey) = New NorthwindDataContext()
            Return CType(System.Web.HttpContext.Current.Items(NorthwindDataProvider.NorthwindDataContextKey), NorthwindDataContext)
        End Get
    End Property

    Private Function CalculateAveragePrice(ByVal categoryID As Integer) As Double
        Return CDbl((From product In NorthwindDataProvider.DB.Products Where product.CategoryID = categoryID Select product).Average(Function(s) s.UnitPrice))
    End Function

    Public Function GetCategories() As IEnumerable
        Return From category In NorthwindDataProvider.DB.Categories Select category
    End Function

    Public Function GetCategoriesNames() As IEnumerable
        Return From category In NorthwindDataProvider.DB.Categories Select category.CategoryName
    End Function

    Public Function GetCategoriesAverage() As IEnumerable
        Return From category In NorthwindDataProvider.DB.Categories Select New With {category.CategoryName, .AvgPrice = NorthwindDataProvider.CalculateAveragePrice(category.CategoryID)}
    End Function

    Public Function GetCustomers() As IEnumerable
        Return From customer In NorthwindDataProvider.DB.Customers Select customer
    End Function

    Public Function GetProducts() As IEnumerable
        Return From product In NorthwindDataProvider.DB.Products Select product
    End Function

    Public Function GetProducts(ByVal categoryName As String) As IEnumerable
        Return From product In NorthwindDataProvider.DB.Products Join category In NorthwindDataProvider.DB.Categories On product.CategoryID Equals category.CategoryID Where Equals(category.CategoryName, categoryName) Select product
    End Function

    Public Function GetEmployees() As IEnumerable
        Return From employee In NorthwindDataProvider.DB.Employees Select employee
    End Function

    Public Function GetEmployeePhoto(ByVal employeeId As Integer) As Binary
        Return(From employee In NorthwindDataProvider.DB.Employees Where employee.EmployeeID = employeeId Select employee.Photo).SingleOrDefault()
    End Function

    Public Function GetEmployeeNotes(ByVal employeeId As Integer) As String
        Return(From employee In NorthwindDataProvider.DB.Employees Where employee.EmployeeID = employeeId Select employee.Notes).[Single]()
    End Function

    Public Function GetOrders() As IEnumerable
        Return From order In NorthwindDataProvider.DB.Orders Select order
    End Function

    Public Function GetInvoices() As IEnumerable
        Return From invoice In NorthwindDataProvider.DB.Invoices Join customer In NorthwindDataProvider.DB.Customers On invoice.CustomerID Equals customer.CustomerID Select New With {customer.CompanyName, invoice.City, invoice.Region, invoice.Country, invoice.UnitPrice, invoice.Quantity}
    End Function

    Public Function GetFullInvoices() As IEnumerable
        Return From invoice In NorthwindDataProvider.DB.Invoices Join customer In NorthwindDataProvider.DB.Customers On invoice.CustomerID Equals customer.CustomerID Join order In NorthwindDataProvider.DB.Orders On invoice.OrderID Equals order.OrderID Select New With {.SalesPerson = order.Employee.FirstName & " " & order.Employee.LastName, customer.CompanyName, invoice.Country, invoice.Region, invoice.OrderDate, invoice.ProductName, invoice.UnitPrice, invoice.Quantity}
    End Function

    Public Function GetInvoices(ByVal customerID As String) As IEnumerable
        Return From invoice In NorthwindDataProvider.DB.Invoices Where Equals(invoice.CustomerID, customerID) Select invoice
    End Function

    Public Function GetEditableProducts() As IList(Of EditableProduct)
        Dim products As System.Collections.Generic.IList(Of EditableProduct) = CType(System.Web.HttpContext.Current.Session("Products"), System.Collections.Generic.IList(Of EditableProduct))
        If products Is Nothing Then
            products =(From product In NorthwindDataProvider.DB.Products Select New EditableProduct With {.ProductID = product.ProductID, .ProductName = product.ProductName, .CategoryID = product.CategoryID, .QuantityPerUnit = product.QuantityPerUnit, .UnitPrice = product.UnitPrice, .UnitsInStock = product.UnitsInStock, .Discontinued = product.Discontinued}).ToList()
            System.Web.HttpContext.Current.Session("Products") = products
        End If

        Return products
    End Function

    Public Function GetEditableProduct(ByVal productID As Integer) As EditableProduct
        Return(From product In NorthwindDataProvider.GetEditableProducts() Where product.ProductID = productID Select product).FirstOrDefault()
    End Function

    Public Function GetNewEditableProductID() As Integer
        Dim lastProduct As EditableProduct =(From product In NorthwindDataProvider.GetEditableProducts() Select product).Last()
        Return If((lastProduct IsNot Nothing), lastProduct.ProductID + 1, 0)
    End Function

    Public Sub DeleteProduct(ByVal productID As Integer)
        Dim product As EditableProduct = NorthwindDataProvider.GetEditableProduct(productID)
        If product IsNot Nothing Then NorthwindDataProvider.GetEditableProducts().Remove(product)
    End Sub

    Public Sub InsertProduct(ByVal product As EditableProduct)
        Dim editProduct As EditableProduct = New EditableProduct()
        editProduct.ProductID = NorthwindDataProvider.GetNewEditableProductID()
        editProduct.ProductName = product.ProductName
        editProduct.CategoryID = product.CategoryID
        editProduct.QuantityPerUnit = product.QuantityPerUnit
        editProduct.UnitPrice = product.UnitPrice
        editProduct.UnitsInStock = product.UnitsInStock
        editProduct.Discontinued = product.Discontinued
        NorthwindDataProvider.GetEditableProducts().Add(editProduct)
    End Sub

    Public Sub UpdateProduct(ByVal product As EditableProduct)
        Dim editProduct As EditableProduct = NorthwindDataProvider.GetEditableProduct(product.ProductID)
        If editProduct IsNot Nothing Then
            editProduct.ProductName = product.ProductName
            editProduct.CategoryID = product.CategoryID
            editProduct.QuantityPerUnit = product.QuantityPerUnit
            editProduct.UnitPrice = product.UnitPrice
            editProduct.UnitsInStock = product.UnitsInStock
            editProduct.Discontinued = product.Discontinued
        End If
    End Sub

    Public Function GetEmployeesList() As IEnumerable
        Return From employee In NorthwindDataProvider.DB.Employees Select New With {.ID = employee.EmployeeID, .Name = employee.LastName & " " & employee.FirstName}
    End Function

    Public Function GetFirstEmployeeID() As Integer
        Return(From employee In NorthwindDataProvider.DB.Employees Select employee.EmployeeID).First(Of Integer)()
    End Function

    Public Function GetEmployee(ByVal employeeId As Integer) As Employee
        Return(From employee In NorthwindDataProvider.DB.Employees Where employeeId = employee.EmployeeID Select employee).[Single](Of Employee)()
    End Function

    Public Function GetOrders(ByVal employeeID As Integer) As IEnumerable
        Return From order In NorthwindDataProvider.DB.Orders Where order.EmployeeID = employeeID Join order_detail In NorthwindDataProvider.DB.Order_Details On order.OrderID Equals order_detail.OrderID Join customer In NorthwindDataProvider.DB.Customers On order.CustomerID Equals customer.CustomerID Select New With {order.OrderID, order.ShipName, order_detail.Quantity, order_detail.UnitPrice, customer.ContactName, customer.CompanyName, customer.City, customer.Address, customer.Phone, customer.Fax}
    End Function
End Module

Public Class EditableProduct

    Public Property ProductID As Integer

    <System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage:="Product Name is required")>
    <System.ComponentModel.DataAnnotations.StringLengthAttribute(50, ErrorMessage:="Must be under 50 characters")>
    Public Property ProductName As String

    <System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage:="Category is required")>
    Public Property CategoryID As Integer?

    <System.ComponentModel.DataAnnotations.StringLengthAttribute(100, ErrorMessage:="Must be under 100 characters")>
    Public Property QuantityPerUnit As String

    <System.ComponentModel.DataAnnotations.RangeAttribute(0, 10000, ErrorMessage:="Must be between 0 and 10000$")>
    Public Property UnitPrice As Decimal?

    <System.ComponentModel.DataAnnotations.RangeAttribute(0, 1000, ErrorMessage:="Must be between 0 and 1000")>
    Public Property UnitsInStock As Short?

    Private discontinuedField As Boolean?

    Public Property Discontinued As Boolean?
        Get
            Return Me.discontinuedField
        End Get

        Set(ByVal value As Boolean?)
            Me.discontinuedField = If(value Is Nothing, False, value)
        End Set
    End Property
End Class
