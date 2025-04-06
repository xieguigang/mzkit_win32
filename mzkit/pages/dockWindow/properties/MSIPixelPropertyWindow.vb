#Region "Microsoft.VisualBasic::013e5f6d13b03f6bd3a820d1f06beca5, mzkit\src\mzkit\mzkit\pages\dockWindow\properties\MSIPixelPropertyWindow.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 57
'    Code Lines: 51
' Comment Lines: 0
'   Blank Lines: 6
'     File Size: 2.27 KB


' Class MSIPixelPropertyWindow
' 
'     Sub: MSIPixelPropertyWindow_Load, SetPixel
' 
' /********************************************************************************/

#End Region

Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Interpolation
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class MSIPixelPropertyWindow

    Public Sub SetPixel(pixel As PixelScan, ByRef props As PixelProperty)
        Select Case Me.DockState
            Case DockState.DockBottomAutoHide, DockState.DockLeftAutoHide, DockState.DockRightAutoHide, DockState.DockTopAutoHide, DockState.Hidden, DockState.Unknown
                props = New PixelProperty(pixel)
                Return
            Case Else
        End Select

        PropertyGrid1.SelectedObject = New PixelProperty(pixel)
        props = PropertyGrid1.SelectedObject

        Call plotIntensityQuantile(pixel.GetMs)
    End Sub

    Public Sub SetSingleCell(cell As ScanMS1, ByRef props As PixelProperty)
        Select Case Me.DockState
            Case DockState.DockBottomAutoHide, DockState.DockLeftAutoHide, DockState.DockRightAutoHide, DockState.DockTopAutoHide, DockState.Hidden, DockState.Unknown
                ' 20250324
                ' just returtns the object, not update ui
                props = New PixelProperty(cell)
                Return
            Case Else
        End Select

        PropertyGrid1.SelectedObject = New PixelProperty(cell)
        props = PropertyGrid1.SelectedObject

        Call plotIntensityQuantile(cell.GetMs)
    End Sub

    Dim serial As SerialData
    Dim q2 As Double
    Dim Q2line As Line

    Private Sub plotIntensityQuantile(spectrum As IEnumerable(Of ms2))
        Dim q As QuantileEstimationGK = spectrum.Select(Function(i) i.intensity).GKQuantile

        q2 = q.Query(0.5)
        serial = New SerialData With {
            .color = Color.SteelBlue,
            .lineType = DashStyle.Dash,
            .pointSize = 10,
            .shape = LegendStyles.Triangle,
            .title = "Intensity",
            .width = 5,
            .pts = seq(0, 1, 0.1) _
                .Select(Function(lv)
                            Return New PointData(lv, q.Query(lv))
                        End Function) _
                .ToArray
        }
        Q2line = New Line(New PointF(0, q2), New PointF(1, q2), New Stroke(Color.Red, 10))

        If DirectCast(PropertyGrid1.SelectedObject, PixelProperty).NumOfIons = 0 Then
            serial = Nothing
            PictureBox1.BackgroundImage = Nothing
        Else
            Call PictureBox1_SizeChanged(Nothing, Nothing)
        End If
    End Sub

    Private Sub MSIPixelPropertyWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "MSI Pixel Properties"
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
    End Sub

    Private Async Sub PictureBox1_SizeChanged(sender As Object, e As EventArgs) Handles PictureBox1.SizeChanged
        PictureBox1.BackgroundImage = Await System.Threading.Tasks.Task.Run(AddressOf Plot)
    End Sub

    Private Function Plot() As Image
        Dim scale As Single = 5
        Dim w As Integer = PictureBox1.Width * scale
        Dim h As Integer = PictureBox1.Height * scale

        If serial Is Nothing Then
            Return Nothing
        End If

        Try
            Return {serial}.Plot(
                size:=$"{w},{h}",
                padding:="padding:50px 50px 100px 250px;",
                fill:=True,
                ablines:={Q2line},
                YtickFormat:="G2",
                XtickFormat:="F1",
                showLegend:=False,
                interplot:=Splines.B_Spline,
                nticksX:=6,
                nticksY:=8,
                fillPie:=False
            ).AsGDIImage
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class
