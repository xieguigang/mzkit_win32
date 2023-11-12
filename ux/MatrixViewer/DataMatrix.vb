Imports Microsoft.VisualBasic.Imaging.Driver
Imports Task

Public MustInherit Class DataMatrix

    ''' <summary>
    ''' any matrix data for display on current page
    ''' </summary>
    Protected ReadOnly matrix As Array
    ''' <summary>
    ''' the name or display title of the current matrix
    ''' </summary>
    Protected name As String

    Sub New(name As String, matrix As Array)
        Me.name = name
        Me.matrix = matrix
    End Sub

    Public Function SetName(name As String) As DataMatrix
        Me.name = name
        Return Me
    End Function

    Public MustOverride Function Plot(args As PlotProperty) As GraphicsData

End Class
