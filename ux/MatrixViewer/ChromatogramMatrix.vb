Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging.Driver
Imports mzblender
Imports Task

Public Class ChromatogramMatrix : Inherits DataMatrix

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(ChromatogramTick)
        End Get
    End Property

    Public Sub New(name As String, matrix As ChromatogramTick())
        MyBase.New(name, matrix)
    End Sub

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim matrix As ChromatogramTick() = Me.matrix

        For Each tick As ChromatogramTick In matrix
            table.Rows.Add(tick.Time, tick.Intensity)
        Next
    End Sub

    Public Overrides Function Plot(args As PlotProperty, picBox As Size) As GraphicsData
        Dim blender As New ChromatogramBlender(name, GetMatrix(Of ChromatogramTick))
        Dim img = blender.Rendering(args, picBox)
        Dim raster As New ImageData(img)

        Return raster
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("time", GetType(Double))
        Yield New NamedValue(Of Type)("intensity", GetType(Double))
    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Return False
    End Function

    Public Overrides Function SaveTo(filepath As String) As Boolean
        Return GetMatrix(Of ChromatogramTick).SaveTo(filepath)
    End Function
End Class
