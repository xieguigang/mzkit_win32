Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports stdNum = System.Math

''' <summary>
''' mz@rt 2d scatter viewer
''' </summary>
Public Class PeakScatterViewer

    Public Event ClickOnPeak(peakId As String, mz As Double, rt As Double)
    Public Event MoveOverPeak(peakId As String, mz As Double, rt As Double)

    Public Property HtmlView As Boolean
        Get
            Return WebView21.Visible
        End Get
        Set(value As Boolean)
            If Not value Then
                WebView21.Hide()
                WebView21.Visible = False
                WebView21.Dock = DockStyle.None
                PictureBox1.Visible = True
                PictureBox1.Show()
                PictureBox1.Dock = DockStyle.Fill

                Call ViewerResize()
            Else
                PictureBox1.Visible = False
                PictureBox1.Hide()
                PictureBox1.Dock = DockStyle.None
                WebView21.Show()
                WebView21.Visible = True
                WebView21.Dock = DockStyle.Fill

                ToolStripStatusLabel1.Text = "Use mouse left button to rotate, and mouse right button to pan the canvas"
            End If
        End Set
    End Property

    Public Property ColorScale As ScalerPalette
        Get
            Return m_palette
        End Get
        Set(value As ScalerPalette)
            m_palette = value
            lcms_scatter.colorScaler = value

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

    Public Property LogScale As Boolean
        Get
            Return m_log
        End Get
        Set(value As Boolean)
            m_log = value

            If Not rawdata.IsNullOrEmpty Then
                Call Rendering()
            End If
        End Set
    End Property

    Dim m_log As Boolean = True
    Dim m_levels As Integer = 255
    Dim m_palette As ScalerPalette = ScalerPalette.turbo
    Dim m_rawdata As Meta()
    Dim canvas_padding As Double = 20

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

    Dim lcms_scatter As New LCMSScatter

    ''' <summary>
    ''' the scatter raw data in current view range
    ''' </summary>
    Dim rawdata As Meta()
    Dim scatters As SerialData()

    Public Function GetSelectedIons() As Meta()
        Return rawdata.ToArray
    End Function

    Public Function GetMenu() As ContextMenuStrip
        Return ContextMenuStrip1
    End Function

    Private Sub PeakScatterViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        WebKit.Init(WebView21)
        HtmlView = False
    End Sub

    Public Function LoadPeaks(peaksdata As IEnumerable(Of Meta)) As PeakScatterViewer
        m_rawdata = peaksdata.ToArray

        Call Application.DoEvents()

        'Dim maxinto As Double

        'If Not m_rawdata.IsNullOrEmpty Then
        '    maxinto = m_rawdata.Select(Function(r) r.intensity).FindThreshold(0.999999999999)
        'End If

        Call Application.DoEvents()

        For i As Integer = 0 To m_rawdata.Length - 1
            If m_rawdata(i).id Is Nothing Then
                m_rawdata(i).id = m_rawdata(i).GetHashCode.ToHexString
            End If

            ' m_rawdata(i).intensity = If(m_rawdata(i).intensity > maxinto, maxinto, m_rawdata(i).intensity)
        Next

        Call Application.DoEvents()

        m_rawdata = m_rawdata _
            .GroupBy(Function(i) i.id) _
            .Select(Function(a) a.First) _
            .ToArray

        Call Application.DoEvents()

        LoadPeaks2(m_rawdata.ToArray)
        lcms_scatter.SetMetadata(m_rawdata)
        WebView21.CoreWebView2.Reload()

        Return Me
    End Function

    Private Sub LoadPeaks2(peaksdata As IEnumerable(Of Meta))
        rawdata = peaksdata.ToArray

        If rawdata.IsNullOrEmpty Then
            mzBins = Nothing
            rtBins = Nothing
            mz_range = Nothing
            rt_range = Nothing
            int_range = Nothing
        Else
            mzBins = New BlockSearchFunction(Of Meta)(rawdata, Function(i) i.mz, 1, fuzzy:=True)
            rtBins = New BlockSearchFunction(Of Meta)(rawdata, Function(i) i.scan_time, 10, fuzzy:=True)
            mz_range = New DoubleRange(rawdata.Select(Function(i) i.mz)).DoCall(AddressOf autoPaddingRange)
            rt_range = New DoubleRange(rawdata.Select(Function(i) i.scan_time)).DoCall(AddressOf autoPaddingRange)
            int_range = New DoubleRange(rawdata.Select(Function(i) i.intensity))

            Call Rendering()
            Call ViewerResize()
        End If
    End Sub

    Private Function autoPaddingRange(r As DoubleRange) As DoubleRange
        Dim min = r.Min
        Dim max = r.Max
        Dim maxPad = max * 1.0125
        Dim d As Double = maxPad - max

        Return New DoubleRange(min - d, maxPad)
    End Function

    Private Iterator Function getScatter() As IEnumerable(Of PointData)
        Dim p As New DoubleRange(0, m_levels - 1)
        Dim int_range As DoubleRange = Me.int_range
        Dim v As Double

        If LogScale Then
            int_range = New DoubleRange(0, stdNum.Log(int_range.Max))
        End If

        For Each m As Meta In rawdata
            v = m.intensity

            If LogScale Then
                v = stdNum.Log(v + 1)
            End If

            Yield New PointData With {
                .pt = New PointF(m.scan_time, m.mz),
                .value = m.intensity,
                .tag = CInt(int_range.ScaleMapping(v, p)).ToString
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
                        Dim i As Integer = Integer.Parse(t.Key)

                        If i >= colors.Length Then
                            i = colors.Length - 1
                        End If

                        Return GetColorLevel(colors(i), t)
                    End Function) _
            .ToArray
        Dim defineSize As Size = PictureBox1.Size
        Dim scaler As New DataScaler() With {
            .AxisTicks = (rt_range.CreateAxisTicks.AsVector, mz_range.CreateAxisTicks.AsVector),
            .region = New Rectangle(New Point(canvas_padding, canvas_padding), New Size(defineSize.Width - canvas_padding * 2, defineSize.Height - canvas_padding * 2)),
            .X = d3js.scale.linear().domain(rt_range).range(integers:={canvas_padding, defineSize.Width - canvas_padding * 2}),
            .Y = d3js.scale.linear().domain(mz_range).range(integers:={canvas_padding, defineSize.Height - canvas_padding * 2})
        }

        ' ignore this gdi device error
        If defineSize.Width = 0 OrElse defineSize.Height = 0 Then
            Return
        End If

        Using g As Graphics2D = defineSize.CreateGDIDevice(filled:=Color.White)
            Dim axisPen As New Pen(Color.Black, 5)
            Dim labelFont As New CSSFont(New Font(FontFace.MicrosoftYaHei, 16))
            Dim labelColor As Brush = Brushes.Black
            Dim tickFont As New Font(FontFace.MicrosoftYaHei, 12)

            ' draw axis
            ' x
            Call Axis.DrawX(g, axisPen, "RT(second)", scaler, XAxisLayoutStyles.Bottom, 0, Nothing, labelFont.CSSValue, labelColor, tickFont, labelColor, htmlLabel:=False)
            ' y
            Call Axis.DrawY(g, axisPen, "m/z", scaler, 0, mz_range.CreateAxisTicks.AsVector, YAxisLayoutStyles.Left, Nothing, labelFont.CSSValue, labelColor, tickFont, labelColor, htmlLabel:=False)

            For Each level As SerialData In colorlevels
                Call Bubble.Plot(g, level, scaler, scale:=Function(r) 5)
            Next

            PictureBox1.BackgroundImage = g.ImageResource
        End Using
    End Sub

    Private Function GetColorLevel(color As Color, points As IEnumerable(Of PointData)) As SerialData
        Return New SerialData() With {
            .color = color,
            .pointSize = 5,
            .pts = points.ToArray
        }
    End Function

    Private Sub PeakScatterViewer_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If drawBox Then
            p1 = Cursor.Position
            PictureBox1.Refresh()
            ToolStripStatusLabel1.Text = "Select new sub-region..."
        Else
            Dim peakId As String = Nothing
            Dim mz, rt As Double
            Dim loc As Point = Cursor.Position

            Call GetPeak(peakId, mz, rt, loc)

            RaiseEvent MoveOverPeak(peakId, mz, rt)

            ToolStripStatusLabel1.Text = $"m/z {mz.ToString("F4")} RT {(rt / 60).ToString("F2")}min; find ion: {peakId}"

            If peakId.StringEmpty Then
                Call ToolTip1.SetToolTip(PictureBox1, $"m/z {mz.ToString("F4")} RT {(rt / 60).ToString("F2")}min")
            Else
                Call ToolTip1.SetToolTip(PictureBox1, $"m/z {mz.ToString("F4")} RT {(rt / 60).ToString("F2")}min" & vbCrLf & vbCrLf & peakId)
            End If
        End If
    End Sub

    Private Sub GetPeak(ByRef peakId As String, ByRef mz As Double, ByRef rt As Double, loc As Point)
        Dim pt = PictureBox1.PointToClient(loc)
        Dim size As Size = PictureBox1.Size

        If mz_range IsNot Nothing AndAlso rt_range IsNot Nothing Then
            Dim mzi = ((size.Height - canvas_padding - pt.Y) / (size.Height - canvas_padding * 2)) * mz_range.Length + mz_range.Min
            Dim rti = (pt.X / (size.Width - canvas_padding * 2)) * rt_range.Length + rt_range.Min

            Dim qmz = mzBins.Search(New Meta With {.mz = mzi}).ToDictionary(Function(a) a.id)
            Dim qrt = rtBins.Search(New Meta With {.scan_time = rti}).ToDictionary(Function(a) a.id)
            Dim find = qmz.Values.Select(Function(a) a.id) _
                .Intersect(qrt.Values.Select(Function(a) a.id)) _
                .OrderBy(Function(id)
                             Return stdNum.Abs(qmz(id).mz - mzi) + stdNum.Abs(qrt(id).scan_time - rti)
                         End Function) _
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
        Dim size As Size = PictureBox1.Size

        If mz_range IsNot Nothing AndAlso rt_range IsNot Nothing Then
            ' scale the mapping of the mouse xy
            ' not for plot rendering
            mzscale = d3js.scale.linear().domain(mz_range).range(integers:={canvas_padding, size.Height - canvas_padding * 2})
            rtscale = d3js.scale.linear().domain(rt_range).range(integers:={canvas_padding, size.Width - canvas_padding * 2})

            Call Rendering()
        End If
    End Sub

    Dim drawBox As Boolean = False
    Dim p0 As Point
    Dim p1 As Point
    Dim t0 As Date

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        drawBox = True
        t0 = Now
        p0 = Cursor.Position
    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        drawBox = False

        If (Now - t0).TotalMilliseconds > 300 Then
            ' do nothing 
        Else
            Dim peakId As String = Nothing
            Dim mz, rt As Double
            Dim loc As Point = Cursor.Position

            Call GetPeak(peakId, mz, rt, loc)

            If peakId IsNot Nothing Then
                RaiseEvent ClickOnPeak(peakId, mz, rt)

                ToolStripStatusLabel1.Text = $"m/z {mz.ToString("F4")} RT {(rt / 60).ToString("F2")}min; ion: '{peakId}' has been selected!"
            End If

            Return
        End If

        Dim mz0, rt0 As Double
        Dim mz1, rt1 As Double

        Call GetPeak(Nothing, mz0, rt0, p0)
        Call GetPeak(Nothing, mz1, rt1, p1)

        If mz0 > mz1 Then Call mz1.Swap(mz0)
        If rt0 > rt1 Then Call rt1.Swap(rt0)

        If Not m_rawdata.IsNullOrEmpty Then
            Call m_rawdata _
                .AsParallel _
                .Where(Function(s)
                           Dim test_mz = s.mz >= mz0 AndAlso s.mz <= mz1
                           Dim test_rt = s.scan_time >= rt0 AndAlso s.scan_time <= rt1

                           Return test_mz AndAlso test_rt
                       End Function) _
                .DoCall(AddressOf LoadPeaks2)
        End If

        Workbench.StatusMessage($"Zoom-in of the sub-region: m/z range {mz0.ToString("F4")} ~ {mz1.ToString("F4")}, RT range {(rt0 / 60).ToString("F2")} ~ {(rt1 / 60).ToString("F2")}min.")
    End Sub

    Private Sub ResetToolStripMenuItem_Click() Handles ResetToolStripMenuItem.Click
        If Not m_rawdata.IsNullOrEmpty Then
            Call LoadPeaks2(m_rawdata)
        End If
    End Sub

    Private Sub PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox1.Paint
        If drawBox Then
            Dim x0 = p0.X, y0 = p0.Y
            Dim x1 = p1.X, y1 = p1.Y

            If x0 > x1 Then x0.Swap(x1)
            If y0 > y1 Then y1.Swap(y1)

            Dim rect As New Rectangle(PictureBox1.PointToClient(New Point(x0, y0)), New Size(x1 - x0, y1 - y0))
            Dim pen As New Pen(Color.Red, 2) With {
                .DashStyle = DashStyle.Dot
            }

            Call e.Graphics.DrawRectangle(pen, rect)
        End If
    End Sub

    Private Sub ViewIn3DCanvasToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewIn3DCanvasToolStripMenuItem.Click
        HtmlView = True
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", lcms_scatter)
        Call WebView21.CoreWebView2.Navigate($"http://127.0.0.1:{Workbench.WebPort}/LCMS-scatter.html")
        Call WebKit.DeveloperOptions(WebView21, enable:=True,)
    End Sub

    Private Sub DViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DViewerToolStripMenuItem.Click
        HtmlView = True
    End Sub

    Private Sub ScatterViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ScatterViewerToolStripMenuItem.Click
        HtmlView = False
    End Sub

    Private Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click
        If HtmlView Then
            Call WebView21.CoreWebView2.Reload()
        Else
            Call ResetToolStripMenuItem_Click()
        End If
    End Sub
End Class

