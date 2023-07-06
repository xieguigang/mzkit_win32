Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

''' <summary>
''' mz@rt 2d scatter viewer
''' </summary>
Public Class PeakScatterViewer

    Public Event ClickOnPeak(peakId As String, mz As Double, rt As Double)
    Public Event MoveOverPeak(peakId As String, mz As Double, rt As Double)

    Public Property ColorScale As ScalerPalette
        Get
            Return m_palette
        End Get
        Set(value As ScalerPalette)
            m_palette = value
            Call Rendering()
        End Set
    End Property

    Public Property ScaleLevels As Integer
        Get
            Return m_levels
        End Get
        Set(value As Integer)
            m_levels = value
            Call Rendering()
        End Set
    End Property

    Dim m_levels As Integer = 255
    Dim m_palette As ScalerPalette = ScalerPalette.turbo

    Dim mzscale As d3js.scale.LinearScale
    Dim rtscale As d3js.scale.LinearScale

    Dim mz_range As DoubleRange
    Dim rt_range As DoubleRange

    Dim mzBins As BlockSearchFunction(Of Meta)
    Dim rtBins As BlockSearchFunction(Of Meta)

    Private Sub PeakScatterViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Public Function LoadPeaks(peaksdata As IEnumerable(Of Meta)) As PeakScatterViewer
        Dim rawdata As Meta() = peaksdata.ToArray

        mzBins = New BlockSearchFunction(Of Meta)(rawdata, Function(i) i.mz, 0.5, fuzzy:=True)
        rtBins = New BlockSearchFunction(Of Meta)(rawdata, Function(i) i.scan_time, 5, fuzzy:=True)
        mz_range = New DoubleRange(rawdata.Select(Function(i) i.mz))
        rt_range = New DoubleRange(rawdata.Select(Function(i) i.scan_time))

        Call Rendering()
        Call ViewerResize()

        Return Me
    End Function

    ''' <summary>
    ''' rendering of the scatter image
    ''' </summary>
    Private Sub Rendering()

    End Sub

    Private Sub PeakScatterViewer_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        Dim peakId As String = Nothing
        Dim mz, rt As Double

        Call GetPeak(peakId, mz, rt)

        RaiseEvent MoveOverPeak(peakId, mz, rt)
    End Sub

    Private Sub GetPeak(ByRef peakId As String, ByRef mz As Double, ByRef rt As Double)
        Dim pt = PointToClient([Control].MousePosition)
        Dim size As Size = Me.Size

        mz = (pt.X / size.Width) * mz_range.Length + mz_range.Min
        rt = (pt.Y / size.Height) * rt_range.Length + rt_range.Min
        peakId = Nothing
    End Sub

    Private Sub ViewerResize() Handles Me.Resize
        Dim size As Size = Me.Size

        mzscale = d3js.scale.linear().domain(mz_range).range(integers:={0, size.Width})
        rtscale = d3js.scale.linear().domain(rt_range).range(integers:={0, size.Height})
    End Sub

    Private Sub PeakScatterViewer_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
        Dim peakId As String = Nothing
        Dim mz, rt As Double

        Call GetPeak(peakId, mz, rt)

        RaiseEvent ClickOnPeak(peakId, mz, rt)
    End Sub
End Class
