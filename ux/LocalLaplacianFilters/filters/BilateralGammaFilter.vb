Imports System
Imports UMapx.Imaging

Namespace LaplacianHDR.Filters
    ''' <summary>
    ''' Defines bilateral gamma correction filter.
    ''' </summary>
    Public Class BilateralGammaCorrection
        Inherits Correction
#Region "Private data"
        Private g As Single
#End Region

#Region "Filter components"
        ''' <summary>
        ''' Initializes bilateral gamma correction filter.
        ''' </summary>
        ''' <param name="g">Gamma</param>
        ''' <param name="space">Space</param>
        Public Sub New(ByVal g As Single, ByVal space As Space)
            Value = g
            MyBase.Space = space
        End Sub
        ''' <summary>
        ''' Gets or sets gamma value.
        ''' </summary>
        Public Property Value As Single
            Get
                Return g
            End Get
            Set(ByVal value As Single)
                g = value
                ' MyBase.Rebuild = True
            End Set
        End Property

#Disable Warning BC40004 ' 成员与基类型中的成员发生冲突，因此应声明为 "Shadows"
        ''' <summary>
        ''' Implements filter rebuilding.
        ''' </summary>
        Protected Overloads Overrides Sub Rebuild()
#Enable Warning BC40004 ' 成员与基类型中的成员发生冲突，因此应声明为 "Shadows"
            Values = Gamma(g, 256)
        End Sub
#End Region

#Region "Static voids"
        ''' <summary>
        ''' Returns array of bilateral gamma filter.
        ''' </summary>
        ''' <param name="g">Gamma</param>
        ''' <param name="length">Length</param>
        ''' <returns>Array</returns>
        Public Shared Function Gamma(ByVal g As Single, ByVal length As Integer) As Single()
            Dim table = New Single(length - 1) {}

            For x = 0 To length - 1
                table(x) = Gamma(x / CSng(length), g)
            Next
            Return table
        End Function

        ''' <summary>
        ''' Returns bilateral gamma function.
        ''' </summary>
        ''' <param name="x">Argument</param>
        ''' <param name="g">Gamma</param>
        ''' <returns>Double</returns>
        Public Shared Function Gamma(ByVal x As Single, ByVal g As Single) As Single
            Dim y, z, w As Double

            ' highlights
            If x >= 0.5 Then
                z = (1.0 - x) * 2.0
                w = Math.Pow(z, g)
                ' shadows
                y = 1.0 - w / 2.0
            Else
                z = x * 2.0
                w = Math.Pow(z, g)
                y = w / 2.0
            End If

            ' check
            If Double.IsNaN(y) Then Return 1.0F

            Return y
        End Function
#End Region
    End Class
End Namespace
