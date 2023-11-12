Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv

Public Class MSAlignmentMatrix : Inherits DataMatrix

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(SSM2MatrixFragment)
        End Get
    End Property

    Public Sub New(name As String, matrix As SSM2MatrixFragment())
        MyBase.New(name, matrix)
    End Sub

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim matrix As SSM2MatrixFragment() = Me.matrix

        For Each tick As SSM2MatrixFragment In matrix
            table.Rows.Add(tick.mz, tick.query, tick.ref, tick.da)
        Next
    End Sub

    Public Overrides Function Plot(args As Task.PlotProperty) As Imaging.Driver.GraphicsData
        Throw New NotImplementedException()
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("m/z", GetType(Double))
        Yield New NamedValue(Of Type)("intensity(query)", GetType(Double))
        Yield New NamedValue(Of Type)("intensity(target)", GetType(Double))
        Yield New NamedValue(Of Type)("tolerance", GetType(Double))
    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Return False
    End Function

    Public Overrides Function SaveTo(filepath As String) As Boolean
        Return GetMatrix(Of SSM2MatrixFragment).SaveTo(filepath)
    End Function
End Class
