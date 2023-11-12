Imports Microsoft.VisualBasic.Imaging.Driver
Imports Task

Public Class PDAMatrix : Inherits DataMatrix

    Public Sub New(name As String, matrix As Array)
        MyBase.New(name, matrix)
    End Sub

    Public Overrides Function Plot(args As PlotProperty) As GraphicsData

    End Function
End Class
