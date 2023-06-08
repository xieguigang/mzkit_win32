Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Math
Imports stdNum = System.Math

Public Class ShowMzBins

    Public Property Layer As PixelData()

    Private Sub ShowMzBins_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call updateBinBox(err:=0.0001)
    End Sub

    Private Sub updateBinBox(err As Double)
        Dim mzbins = Layer.GroupBy(Function(p) p.mz, Function(x, y) stdNum.Abs(x - y) < err).ToArray
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

        PictureBox1.BackgroundImage = Scatter.Plot(
            c:=serials,
            size:="1900,1280",
            Xlabel:="m/z",
            Ylabel:="binbox size",
            fill:=True,
            drawLine:=True,
            padding:="padding:100px 350px 150px 150px;",
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
    End Sub
End Class