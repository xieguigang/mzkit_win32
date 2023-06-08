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
        Dim mzgroups = hist.GroupBy(Function(i) i.mz, offsets:=0.5).OrderBy(Function(a) Val(a.name)).ToArray
        Dim colors As Color() = Designer.GetColors("paper", n:=mzgroups.Length)
        Dim serials As SerialData() = mzgroups _
            .Select(Function(si, i)
                        Return New SerialData With {
                            .color = colors(i),
                            .lineType = DashStyle.Dot,
                            .pointSize = 5,
                            .shape = LegendStyles.Circle,
                            .title = si.name,
                            .width = 3,
                            .pts = si _
                                .Select(Function(xi) New PointData(xi.mz, xi.binbox.Length)) _
                                .OrderBy(Function(pi) pi.pt.X) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        PictureBox1.BackgroundImage = Scatter.Plot(
            c:=serials,
            size:="1600,1200",
            Xlabel:="m/z",
            Ylabel:="binbox size",
            fill:=True,
            drawLine:=True,
            padding:="padding:100px 300px 250px 250px;",
            dpi:=200,
            YtickFormat:="F0",
            preferPositive:=True,
            xlim:=mzbins.Select(Function(a) Val(a.name)).Max + 0.3
        ).AsGDIImage
    End Sub
End Class