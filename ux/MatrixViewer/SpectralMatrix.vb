Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Task

Public Class SpectralMatrix : Inherits DataMatrix

    ReadOnly precursor As (mz As Double, rt As Double)
    ReadOnly source As String
    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(ms2)
        End Get
    End Property

    Public Sub New(name As String, matrix As ms2(), precursor As (mz As Double, rt As Double), source As String)
        MyBase.New(name, matrix)

        Me.source = source
        Me.precursor = precursor
    End Sub

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim matrix As ms2() = Me.matrix
        Dim max = matrix.Select(Function(a) a.intensity).Max

        For Each tick As ms2 In matrix
            table.Rows.Add(
                tick.mz,
                tick.intensity,
                CInt(tick.intensity / max * 100),
                tick.Annotation
            )
        Next
    End Sub

    Public Overrides Function Plot(args As PlotProperty, picBox As Size) As GraphicsData
        Dim scanData As New LibraryMatrix With {
            .name = name,
            .ms2 = matrix,
            .parentMz = precursor.mz
        }

        Return PeakAssign.DrawSpectrumPeaks(
            scanData,
            padding:=args.GetPadding.ToString,
            bg:=args.background.ToHtmlColor,
            size:=$"{args.width},{args.height}",
            labelIntensity:=If(args.show_tag, 0.25, 100),
            gridFill:=args.gridFill.ToHtmlColor,
            barStroke:=$"stroke: steelblue; stroke-width: {args.line_width}px; stroke-dash: solid;",
            dpi:=200
        )
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("m/z", GetType(Double))
        Yield New NamedValue(Of Type)("intensity", GetType(Double))
        Yield New NamedValue(Of Type)("relative", GetType(Double))
        Yield New NamedValue(Of Type)("annotation", GetType(String))
    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Return False
    End Function

    Public Overrides Function SaveTo(filepath As String) As Boolean
        Return GetMatrix(Of ms2).SaveTo(filepath)
    End Function
End Class
