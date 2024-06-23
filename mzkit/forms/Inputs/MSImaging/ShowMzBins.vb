Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports std = System.Math

Public Class ShowMzBins

    Dim hist As Image
    Dim quantile As Image
    Dim stat As IonStat

    Private Sub ShowMzBins_Load(sender As Object, e As EventArgs) Handles Me.Load
        PictureBox1.BackgroundImage = hist
        PictureBox2.BackgroundImage = quantile
        PropertyGrid1.SelectedObject = stat
        PropertyGrid1.Refresh()
    End Sub

    Public Sub SetData(layer As PixelData())
        Dim mz As Double

        hist = DrawHistogramLine(layer, 0.0001)
        quantile = DrawIntensityQuantile(layer)

        If layer.IsNullOrEmpty Then
            mz = 0
        Else
            mz = Aggregate spot As PixelData
                 In layer
                 Into Average(spot.mz)
        End If

        stat = SpatialIonStats.DoStat(New SingleIonLayer With {.IonMz = mz, .MSILayer = layer})
    End Sub

    Private Shared Function DrawIntensityQuantile(layer As PixelData()) As Image
        Dim into As Double() = layer.Select(Function(p) p.intensity).ToArray
        Dim quantile As New FastRankQuantile(into)
        Dim x As Double() = Enumerable.Range(0, 11).Select(Function(i) i / 10).ToArray
        Dim y As Double() = x.Select(Function(q) quantile.Query(q)).ToArray
        Dim line As New SerialData With {
            .color = "skyblue".TranslateColor,
            .lineType = DashStyle.Dash,
            .pointSize = 6,
            .shape = LegendStyles.Circle,
            .title = "Intensity Quantile",
            .width = 4,
            .pts = x.Select(Function(xi, i) New PointData(xi, y(i))).ToArray
        }

        Return Scatter.Plot(
            c:={line},
            size:="1900,1280",
            Xlabel:="quantile",
            Ylabel:="intensity",
            fill:=True,
            drawLine:=True,
            padding:="padding:100px 400px 150px 200px;",
            dpi:=150,
            YtickFormat:="F0",
            preferPositive:=True,
            xlim:=1,
            tickFontStyle:="font-style: normal; font-size: 16; font-family: " & FontFace.MicrosoftYaHei & ";",
            axisLabelCSS:="font-style: normal; font-size: 16; font-family: " & FontFace.SegoeUI & ";",
            interplot:=Interpolation.Splines.B_Spline,
            fillPie:=False,
            showGrid:=False,
            gridFill:="white"
        ).AsGDIImage
    End Function

    ''' <summary>
    ''' show mz ions range
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <param name="err"></param>
    ''' <returns></returns>
    Private Shared Function DrawHistogramLine(layer As PixelData(), err As Double) As Image
        Dim mzbins = layer.GroupBy(Function(p) p.mz, Function(x, y) std.Abs(x - y) < err).ToArray
        Dim hist = mzbins.OrderBy(Function(a) Val(a.name)).Select(Function(mzi) (mz:=Val(mzi.name), binbox:=mzi.ToArray)).ToArray
        Dim mzgroups = hist.GroupBy(Function(i) i.mz, offsets:=0.1).OrderBy(Function(a) Val(a.name)).ToArray
        Dim colors As Color() = Designer.GetColors("paper", n:=mzgroups.Length)

        If mzgroups.Length = 1 Then
            colors = {Color.SkyBlue}
        End If

        Dim serials As SerialData() = mzgroups _
            .Select(Function(si, i)
                        Return New SerialData With {
                            .color = colors(i),
                            .lineType = DashStyle.Dot,
                            .pointSize = 5,
                            .shape = LegendStyles.Circle,
                            .title = "m/z " & Val(si.name).ToString("F3"),
                            .width = 3,
                            .pts = si _
                                .Select(Function(xi) New PointData(xi.mz, xi.binbox.Length)) _
                                .OrderBy(Function(pi) pi.pt.X) _
                                .ToArray,
                             .DataAnnotations = .pts.OrderByDescending(Function(pi) pi.pt.Y) _
                                .Take(3) _
                                .Select(Function(a)
                                            Return New Annotation With {
                                                .X = a.pt.X,
                                                .Y = a.pt.Y,
                                                .Text = a.pt.X.ToString("F4"),
                                                .color = "black",
                                                .Font = "font-style: normal; font-size: 13; font-family: " & FontFace.SegoeUI & ";",
                                                .Legend = LegendStyles.Triangle,
                                                .size = New SizeF(20, 20)
                                            }
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        Return Scatter.Plot(
            c:=serials,
            size:="1900,1280",
            Xlabel:="m/z",
            Ylabel:="binbox size",
            fill:=True,
            drawLine:=True,
            padding:="padding:100px 400px 150px 200px;",
            dpi:=150,
            YtickFormat:="F0",
            preferPositive:=True,
            xlim:=mzbins.Select(Function(a) Val(a.name)).Max + 0.015,
            tickFontStyle:="font-style: normal; font-size: 16; font-family: " & FontFace.MicrosoftYaHei & ";",
            axisLabelCSS:="font-style: normal; font-size: 16; font-family: " & FontFace.SegoeUI & ";",
            interplot:=Interpolation.Splines.B_Spline,
            fillPie:=False,
            showGrid:=False,
            gridFill:="white"
        ).AsGDIImage
    End Function
End Class