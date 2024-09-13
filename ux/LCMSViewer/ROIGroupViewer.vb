Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Math

Public Class ROIGroupViewer

    Dim samples As NamedCollection(Of ms1_scan)()
    Dim viewers As PictureBox()
    Dim mz As Double
    Dim rt As Double
    Dim current As NamedCollection(Of ms1_scan)
    Dim dt As Double = 7.5

    Public Property ROIViewerHeight As Integer = 100

    Public Event SelectFile(filename As String)

    Public Iterator Function GetXic() As IEnumerable(Of NamedCollection(Of ChromatogramTick))
        For Each file As NamedCollection(Of ms1_scan) In samples
            Dim xic_data = file _
                .GroupBy(Function(i) i.scan_time, offsets:=0.25) _
                .Select(Function(i)
                            Return New ChromatogramTick(Val(i.name), i.Average(Function(a) a.intensity))
                        End Function)
            Dim xic As New NamedCollection(Of ChromatogramTick)(file.name, xic_data)

            Yield xic
        Next
    End Function

    Public Async Function LoadROIs(mz As Double, rt As Double, samples As IEnumerable(Of NamedCollection(Of ms1_scan))) As Task(Of ROIGroupViewer)
        Call FlowLayoutPanel1.Controls.Clear()

        Me.mz = mz
        Me.rt = rt
        Me.samples = samples.ToArray
        Me.viewers = New PictureBox(Me.samples.Length - 1) {}

        For i As Integer = 0 To Me.samples.Length - 1
            Dim pic As New PictureBox

            FlowLayoutPanel1.Controls.Add(pic)
            pic.Height = ROIViewerHeight
            pic.Width = FlowLayoutPanel1.Width * 0.9
            pic.Tag = Me.samples(i)
            pic.BackgroundImageLayout = ImageLayout.Zoom

            AddHandler pic.Click, AddressOf PictureBox_Click

            viewers(i) = pic
        Next

        Await Me.Rendering()

        Return Me
    End Function

    Private Async Sub PictureBox_Click(sender As Object, e As EventArgs)
        Dim pic As PictureBox = sender

        ' rendering of the image
        current = DirectCast(pic.Tag, NamedCollection(Of ms1_scan))
        Await RenderingSelection()

        RaiseEvent SelectFile(current.name)
    End Sub

    Private Async Function Rendering() As Task
        ' resize all pictures to the size of left panel
        Dim newWidth As Integer = FlowLayoutPanel1.Width * 0.9

        If viewers.IsNullOrEmpty Then
            Return
        End If

        Dim scale As Double = 5
        Dim theme As New Theme With {.padding = "padding:100px 100px 100px 400px;"}
        Dim xic As ChromatogramTick()
        Dim rt_range As Double() = {rt - dt * 1.5, rt + dt * 1.5}
        Dim xic_data As NamedCollection(Of ChromatogramTick)
        Dim unifySize As String = $"{newWidth * scale },{ROIViewerHeight * scale }"
        Dim render As GraphicsData

        ' make rendering
        For i As Integer = 0 To viewers.Length - 1
            xic = DirectCast(viewers(i).Tag, NamedCollection(Of ms1_scan)) _
                .AsEnumerable _
                .GroupBy(Function(a) a.scan_time, offsets:=0.25) _
                .Select(Function(m1) New ChromatogramTick(Val(m1.name), m1.Average(Function(a) a.intensity))) _
                .OrderBy(Function(m1) m1.Time) _
                .ToArray
            xic_data = New NamedCollection(Of ChromatogramTick)(DirectCast(viewers(i).Tag, NamedCollection(Of ms1_scan)).name, xic)
            render = Await Task(Of GraphicsData).Run(Function()
                                                         Return New TICplot(xic_data,
                                                                timeRange:=rt_range,
                                                                intensityMax:=0,
                                                                isXIC:=True,
                                                                fillAlpha:=200,
                                                                fillCurve:=False,
                                                                labelLayoutTicks:=-1,
                                                                bspline:=2,
                                                                theme:=theme) With {.xlabel = "Retention Time(s)", .ylabel = "Intensity"} _
                                                        .Plot(unifySize, ppi:=200)
                                                     End Function)

            viewers(i).Width = newWidth
            viewers(i).BackgroundImage = render.AsGDIImage
        Next

        Await RenderingSelection()
    End Function

    Private Async Function RenderingSelection() As Task
        If current.IsEmpty Then
            Return
        End If

        Dim scale As Double = 2
        Dim size As String = $"{PictureBox1.Width * scale},{PictureBox1.Height * scale}"
        Dim theme As New Theme With {
            .pointSize = 15,
            .drawLegend = False,
            .padding = "padding:100px 100px 200px 200px;"
        }
        Dim density As New PlotMassWindowXIC(current, theme)
        Dim render As GraphicsData = Await Task(Of GraphicsData).Run(Function() density.Plot(size, ppi:=100))

        PictureBox1.BackgroundImage = render.AsGDIImage
    End Function

    Private Async Sub ROIGroupViewer_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        Await Rendering()
    End Sub

    Private Async Sub SplitContainer1_SplitterMoved(sender As Object, e As SplitterEventArgs) Handles SplitContainer1.SplitterMoved
        Await Rendering()
    End Sub
End Class
