Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports stdNum = System.Math

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

            If Not rawdata.IsNullOrEmpty Then
                Call Rendering()
            End If
        End Set
    End Property

    Public Property ScaleLevels As Integer
        Get
            Return m_levels
        End Get
        Set(value As Integer)
            m_levels = value

            If Not rawdata.IsNullOrEmpty Then
                Call Rendering()
            End If
        End Set
    End Property

    Dim m_levels As Integer = 255
    Dim m_palette As ScalerPalette = ScalerPalette.turbo

    ' mzscale and rtscale is used for 
    ' scale the mapping of the mouse xy
    ' not for plot rendering

    ''' <summary>
    ''' Y axis mapper
    ''' </summary>
    Dim mzscale As d3js.scale.LinearScale
    ''' <summary>
    ''' X axis mapper
    ''' </summary>
    Dim rtscale As d3js.scale.LinearScale

    Dim mz_range As DoubleRange
    Dim rt_range As DoubleRange
    Dim int_range As DoubleRange

    Dim mzBins As BlockSearchFunction(Of Meta)
    Dim rtBins As BlockSearchFunction(Of Meta)

    Dim rawdata As Meta()
    Dim scatters As SerialData()

    Private Sub PeakScatterViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Public Function LoadPeaks(peaksdata As IEnumerable(Of Meta)) As PeakScatterViewer
        rawdata = peaksdata.ToArray
        mzBins = New BlockSearchFunction(Of Meta)(rawdata, Function(i) i.mz, 0.5, fuzzy:=True)
        rtBins = New BlockSearchFunction(Of Meta)(rawdata, Function(i) i.scan_time, 5, fuzzy:=True)
        mz_range = New DoubleRange(0, rawdata.Select(Function(i) i.mz).Max * 1.125)
        rt_range = New DoubleRange(0, rawdata.Select(Function(i) i.scan_time).Max * 1.125)
        int_range = New DoubleRange(rawdata.Select(Function(i) i.intensity))

        Call Rendering()
        Call ViewerResize()

        Return Me
    End Function

    Private Iterator Function getScatter() As IEnumerable(Of PointData)
        Dim p As New DoubleRange(0, m_levels - 1)

        For Each m As Meta In rawdata
            Yield New PointData With {
                .pt = New PointF(m.scan_time, m.mz),
                .value = m.intensity,
                .tag = CInt(int_range.ScaleMapping(.value, p)).ToString
            }
        Next
    End Function

    ''' <summary>
    ''' rendering of the scatter image
    ''' </summary>
    Private Sub Rendering()
        Dim colors As Color() = Designer.GetColors(m_palette.Description, m_levels)
        Dim colorlevels = getScatter _
            .GroupBy(Function(p) p.tag) _
            .Select(Function(t)
                        Return GetColorLevel(colors(Integer.Parse(t.Key)), t)
                    End Function) _
            .ToArray
        Dim defineSize As Size = Me.Size
        Dim scaler As New DataScaler() With {
            .AxisTicks = Nothing,
            .region = New Rectangle(New Point, defineSize),
            .X = d3js.scale.linear().domain(rt_range).range(integers:={0, defineSize.Width}),
            .Y = d3js.scale.linear().domain(mz_range).range(integers:={0, defineSize.Height})
        }

        Using g As Graphics2D = defineSize.CreateGDIDevice(filled:=Color.White)
            For Each level As SerialData In colorlevels
                Call Bubble.Plot(g, level, scaler, scale:=Function(r) 5)
            Next

            BackgroundImage = g.ImageResource
        End Using
    End Sub

    Private Function GetColorLevel(color As Color, points As IEnumerable(Of PointData)) As SerialData
        Return New SerialData() With {
            .color = color,
            .pointSize = 5,
            .pts = points.ToArray
        }
    End Function

    Private Sub PeakScatterViewer_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        Dim peakId As String = Nothing
        Dim mz, rt As Double
        Dim loc As Point = Cursor.Position

        Call GetPeak(peakId, mz, rt, loc)

        RaiseEvent MoveOverPeak(peakId, mz, rt)
    End Sub

    Private Sub GetPeak(ByRef peakId As String, ByRef mz As Double, ByRef rt As Double, loc As Point)
        Dim pt = Me.PointToClient(loc)
        Dim size As Size = Me.Size

        If mz_range IsNot Nothing AndAlso rt_range IsNot Nothing Then
            Dim mzi = (pt.Y / size.Height) * mz_range.Length + mz_range.Min
            Dim rti = (pt.X / size.Width) * rt_range.Length + rt_range.Min

            Dim qmz = mzBins.Search(New Meta With {.mz = mzi}).ToDictionary(Function(a) a.id)
            Dim qrt = rtBins.Search(New Meta With {.scan_time = rti}).ToDictionary(Function(a) a.id)
            Dim find = qmz.Values.Select(Function(a) a.id) _
                .Intersect(qrt.Values.Select(Function(a) a.id)) _
                .OrderBy(Function(id) stdNum.Abs(qmz(id).mz - mzi) + stdNum.Abs(qrt(id).scan_time - rti)) _
                .FirstOrDefault

            mz = mzi
            rt = rti

            If find Is Nothing Then
                peakId = Nothing
            Else
                peakId = find
            End If
        Else
            peakId = Nothing
        End If
    End Sub

    Private Sub ViewerResize() Handles Me.SizeChanged
        Dim size As Size = Me.Size

        If mz_range IsNot Nothing AndAlso rt_range IsNot Nothing Then
            ' scale the mapping of the mouse xy
            ' not for plot rendering
            mzscale = d3js.scale.linear().domain(mz_range).range(integers:={0, size.Height})
            rtscale = d3js.scale.linear().domain(rt_range).range(integers:={0, size.Width})
        End If
    End Sub

    Private Sub PeakScatterViewer_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
        Dim peakId As String = Nothing
        Dim mz, rt As Double
        Dim loc As Point = Cursor.Position

        Call GetPeak(peakId, mz, rt, loc)

        If peakId IsNot Nothing Then
            RaiseEvent ClickOnPeak(peakId, mz, rt)
        End If
    End Sub
End Class
