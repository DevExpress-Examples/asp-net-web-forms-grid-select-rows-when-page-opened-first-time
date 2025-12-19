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
		Private Const NorthwindDataContextKey As String = "DXNorthwindDataContext"

		Public ReadOnly Property DB() As NorthwindDataContext
			Get
				If HttpContext.Current.Items(NorthwindDataContextKey) Is Nothing Then
					HttpContext.Current.Items(NorthwindDataContextKey) = New NorthwindDataContext()
				End If
				Return DirectCast(HttpContext.Current.Items(NorthwindDataContextKey), NorthwindDataContext)
			End Get
		End Property

		Private Function CalculateAveragePrice(ByVal categoryID As Integer) As Double
			Return CDbl((
				From product In DB.Products
				Where product.CategoryID = categoryID
				Select product).Average(Function(s) s.UnitPrice))
		End Function
		Public Function GetCategories() As IEnumerable
			Return From category In DB.Categories
				Select category
		End Function
		Public Function GetCategoriesNames() As IEnumerable
			Return From category In DB.Categories
				Select category.CategoryName
		End Function
		Public Function GetCategoriesAverage() As IEnumerable
			Return From category In DB.Categories
				Select New With {
					Key category.CategoryName,
					Key .AvgPrice = CalculateAveragePrice(category.CategoryID)
				}
		End Function
		Public Function GetCustomers() As IEnumerable
			Return From customer In DB.Customers
				Select customer
		End Function
		Public Function GetProducts() As IEnumerable
			Return From product In DB.Products
				Select product
		End Function
		Public Function GetProducts(ByVal categoryName As String) As IEnumerable
			Return From product In DB.Products
				Join category In DB.Categories On product.CategoryID Equals category.CategoryID
				Where category.CategoryName = categoryName
				Select product
		End Function
		Public Function GetEmployees() As IEnumerable
			Return From employee In DB.Employees
				Select employee
		End Function
		Public Function GetEmployeePhoto(ByVal employeeId As Integer) As Binary
			Return (
				From employee In DB.Employees
				Where employee.EmployeeID = employeeId
				Select employee.Photo).SingleOrDefault()
		End Function
		Public Function GetEmployeeNotes(ByVal employeeId As Integer) As String
			Return (
				From employee In DB.Employees
				Where employee.EmployeeID = employeeId
				Select employee.Notes).Single()
		End Function
		Public Function GetOrders() As IEnumerable
			Return From order In DB.Orders
				Select order
		End Function
		Public Function GetInvoices() As IEnumerable
			Return From invoice In DB.Invoices
				Join customer In DB.Customers On invoice.CustomerID Equals customer.CustomerID
				Select New With {
					Key customer.CompanyName,
					Key invoice.City,
					Key invoice.Region,
					Key invoice.Country,
					Key invoice.UnitPrice,
					Key invoice.Quantity
				}
		End Function
		Public Function GetFullInvoices() As IEnumerable
			Return From invoice In DB.Invoices
				Join customer In DB.Customers On invoice.CustomerID Equals customer.CustomerID
				Join order In DB.Orders On invoice.OrderID Equals order.OrderID
				Select New With {
					Key .SalesPerson = order.Employee.FirstName & " " & order.Employee.LastName,
					Key customer.CompanyName,
					Key invoice.Country,
					Key invoice.Region,
					Key invoice.OrderDate,
					Key invoice.ProductName,
					Key invoice.UnitPrice,
					Key invoice.Quantity
				}
		End Function
		Public Function GetInvoices(ByVal customerID As String) As IEnumerable
			Return From invoice In DB.Invoices
				Where invoice.CustomerID = customerID
				Select invoice
		End Function

		Public Function GetEditableProducts() As IList(Of EditableProduct)
			Dim products As IList(Of EditableProduct) = DirectCast(HttpContext.Current.Session("Products"), IList(Of EditableProduct))

			If products Is Nothing Then
				products = (
					From product In DB.Products
					Select New EditableProduct With {
						.ProductID = product.ProductID,
						.ProductName = product.ProductName,
						.CategoryID = product.CategoryID,
						.QuantityPerUnit = product.QuantityPerUnit,
						.UnitPrice = product.UnitPrice,
						.UnitsInStock = product.UnitsInStock,
						.Discontinued = product.Discontinued
					}).ToList()
				HttpContext.Current.Session("Products") = products
			End If
			Return products
		End Function
		Public Function GetEditableProduct(ByVal productID As Integer) As EditableProduct
			Return (
				From product In GetEditableProducts()
				Where product.ProductID = productID
				Select product).FirstOrDefault()
		End Function
		Public Function GetNewEditableProductID() As Integer
			Dim lastProduct As EditableProduct = (
				From product In GetEditableProducts()
				Select product).Last()
			Return If(lastProduct IsNot Nothing, lastProduct.ProductID + 1, 0)
		End Function
		Public Sub DeleteProduct(ByVal productID As Integer)
			Dim product As EditableProduct = GetEditableProduct(productID)
			If product IsNot Nothing Then
				GetEditableProducts().Remove(product)
			End If
		End Sub
		Public Sub InsertProduct(ByVal product As EditableProduct)
			Dim editProduct As New EditableProduct()
			editProduct.ProductID = GetNewEditableProductID()
			editProduct.ProductName = product.ProductName
			editProduct.CategoryID = product.CategoryID
			editProduct.QuantityPerUnit = product.QuantityPerUnit
			editProduct.UnitPrice = product.UnitPrice
			editProduct.UnitsInStock = product.UnitsInStock
			editProduct.Discontinued = product.Discontinued
			GetEditableProducts().Add(editProduct)
		End Sub
		Public Sub UpdateProduct(ByVal product As EditableProduct)
			Dim editProduct As EditableProduct = GetEditableProduct(product.ProductID)
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
			Return From employee In DB.Employees
				Select New With {
					Key .ID = employee.EmployeeID,
					Key .Name = employee.LastName & " " & employee.FirstName
				}
		End Function
		Public Function GetFirstEmployeeID() As Integer
			Return (
				From employee In DB.Employees
				Select employee.EmployeeID).First()
		End Function
		Public Function GetEmployee(ByVal employeeId As Integer) As Employee
			Return (
				From employee In DB.Employees
				Where employeeId = employee.EmployeeID
				Select employee).Single()
		End Function
		Public Function GetOrders(ByVal employeeID As Integer) As IEnumerable
			Return From order In DB.Orders
				Where order.EmployeeID = employeeID
				Join order_detail In DB.Order_Details On order.OrderID Equals order_detail.OrderID
				Join customer In DB.Customers On order.CustomerID Equals customer.CustomerID
				Select New With {
					Key order.OrderID,
					Key order.ShipName,
					Key order_detail.Quantity,
					Key order_detail.UnitPrice,
					Key customer.ContactName,
					Key customer.CompanyName,
					Key customer.City,
					Key customer.Address,
					Key customer.Phone,
					Key customer.Fax
				}
		End Function
	End Module

	Public Class EditableProduct
		Public Property ProductID() As Integer

		<Required(ErrorMessage := "Product Name is required")>
		<StringLength(50, ErrorMessage := "Must be under 50 characters")>
		Public Property ProductName() As String

		<Required(ErrorMessage := "Category is required")>
		Public Property CategoryID() As Integer?

		<StringLength(100, ErrorMessage := "Must be under 100 characters")>
		Public Property QuantityPerUnit() As String

		<Range(0, 10000, ErrorMessage := "Must be between 0 and 10000$")>
		Public Property UnitPrice() As Decimal?

		<Range(0, 1000, ErrorMessage := "Must be between 0 and 1000")>
		Public Property UnitsInStock() As Short?

'INSTANT VB NOTE: The field discontinued was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private discontinued_Conflict? As Boolean
		Public Property Discontinued() As Boolean?
			Get
				Return discontinued_Conflict
			End Get
			Set(ByVal value? As Boolean)
				discontinued_Conflict = If(value Is Nothing, False, value)
			End Set
		End Property
	End Class

