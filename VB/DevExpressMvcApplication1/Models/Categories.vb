Imports System.Collections
Imports System.Linq
Imports System.Web.UI

Public MustInherit Class ItemsData
    Implements IHierarchicalEnumerable, IEnumerable

    Private data As IEnumerable

    Public Sub New()
        data = GetData()
    End Sub

    Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return data.GetEnumerator()
    End Function

    Public Function GetHierarchyData(ByVal enumeratedItem As Object) As IHierarchyData Implements IHierarchicalEnumerable.GetHierarchyData
        Return CType(enumeratedItem, IHierarchyData)
    End Function

    Public MustOverride Function GetData() As IEnumerable
End Class

Public Class ItemData
    Implements IHierarchyData

    Private _Text As String, _NavigateUrl As String

    Public Property Text As String
        Get
            Return _Text
        End Get

        Protected Set(ByVal value As String)
            _Text = value
        End Set
    End Property

    Public Property NavigateUrl As String
        Get
            Return _NavigateUrl
        End Get

        Protected Set(ByVal value As String)
            _NavigateUrl = value
        End Set
    End Property

    Public Sub New(ByVal text As String, ByVal navigateUrl As String)
        Me.Text = text
        Me.NavigateUrl = navigateUrl
    End Sub

    ' IHierarchyData
    Private ReadOnly Property IHierarchyData_HasChildren As Boolean Implements IHierarchyData.HasChildren
        Get
            Return HasChildren()
        End Get
    End Property

    Private ReadOnly Property Item As Object Implements IHierarchyData.Item
        Get
            Return Me
        End Get
    End Property

    Private ReadOnly Property Path As String Implements IHierarchyData.Path
        Get
            Return NavigateUrl
        End Get
    End Property

    Private ReadOnly Property Type As String Implements IHierarchyData.Type
        Get
            Return [GetType]().ToString()
        End Get
    End Property

    Private Function GetChildren() As IHierarchicalEnumerable Implements IHierarchyData.GetChildren
        Return CreateChildren()
    End Function

    Private Function GetParent() As IHierarchyData Implements IHierarchyData.GetParent
        Return Nothing
    End Function

    Protected Overridable Function HasChildren() As Boolean
        Return False
    End Function

    Protected Overridable Function CreateChildren() As IHierarchicalEnumerable
        Return Nothing
    End Function
End Class

Public Class CategoriesData
    Inherits ItemsData

    Public Overrides Function GetData() As IEnumerable
        Return From category In DB.Categories Select New CategoryData(category)
    End Function
End Class

Public Class CategoryData
    Inherits ItemData

    Private _Category As Category

    Public Property Category As Category
        Get
            Return _Category
        End Get

        Protected Set(ByVal value As Category)
            _Category = value
        End Set
    End Property

    Public Sub New(ByVal category As Category)
        MyBase.New(category.CategoryName, "?CategoryID=" & category.CategoryID)
        Me.Category = category
    End Sub

    Protected Overrides Function HasChildren() As Boolean
        Return True
    End Function

    Protected Overrides Function CreateChildren() As IHierarchicalEnumerable
        Return New ProductsData(Category.CategoryID)
    End Function
End Class

Public Class ProductsData
    Inherits ItemsData

    Private _CategoryID As Integer

    Public Property CategoryID As Integer
        Get
            Return _CategoryID
        End Get

        Protected Set(ByVal value As Integer)
            _CategoryID = value
        End Set
    End Property

    Public Sub New(ByVal categoryID As Integer)
        MyBase.New()
        Me.CategoryID = categoryID
    End Sub

    Public Overrides Function GetData() As IEnumerable
        Return From product In DB.Products Where product.CategoryID = CategoryID Select New ProductData(product)
    End Function
End Class

Public Class ProductData
    Inherits ItemData

    Public Sub New(ByVal product As Product)
        MyBase.New(product.ProductName, "?CategoryID=" & product.CategoryID & "&ProductID=" & product.ProductID)
    End Sub
End Class
