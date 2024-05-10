Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Mzkit_win32.BasicMDIForm
Imports std = System.Math

Public Class XICFeatureViewer

    Dim XIC As ChromatogramTick()
    Dim features As PeakMs2()
    Dim time_range As DoubleRange
    Dim intomax As Double
    Dim mouse_cur As Point
    Dim selected_peak As PeakMs2
    Dim highlight As Boolean

    Public Property FillColor As Color = Color.Red

    Public Sub SetFeatures(source As String(), xic As IEnumerable(Of ChromatogramTick), features As IEnumerable(Of PeakMs2), rt_range As DoubleRange)
        Me.XIC = xic.ToArray
        Me.features = features.SafeQuery.ToArray

        If Not Me.XIC.IsNullOrEmpty Then
            Me.time_range = Me.XIC.TimeRange
            Me.intomax = Me.XIC.IntensityArray.Max

            Dim hipeak = Me.XIC.Shadows _
                .PopulateROI(peakwidth:=New DoubleRange(3, 20)) _
                .OrderByDescending(Function(r) r.maxInto) _
                .FirstOrDefault

            If Not hipeak Is Nothing Then
                source = source _
                    .JoinIterates({
                                  "RT: " & hipeak.rt.ToString("F0") & "sec",
                                 $"RT(min): {(hipeak.rt / 60).ToString("F1")}min"}) _
                    .ToArray
            End If
        End If
        If Not rt_range Is Nothing Then
            Me.time_range = New DoubleRange(rt_range)
        End If

        TextBox1.Text = source.JoinBy(vbCrLf)

        Call RenderViewer()

        If Not Me.features.IsNullOrEmpty Then
            Call RenderSpectrum(Me.features(0))
        End If
    End Sub

    Private Sub RenderViewer()
        Dim size = canvasXIC.Size
        Dim width As New DoubleRange(0, size.Width)
        Dim h As Integer = 3

        Using g = size.CreateGDIDevice
            Call g.FillRectangle(New SolidBrush(BackColor), New RectangleF(0, 0, size.Width, size.Height))

            If Not XIC.IsNullOrEmpty Then
                Call RtRangeSelector.DrawTIC(g.Graphics, XIC, time_range, intomax, FillColor, canvasXIC)
            End If

            For Each peak As PeakMs2 In features
                Dim color As Color = Color.Black

                If peak Is selected_peak Then
                    color = Color.Green
                End If

                Call g.FillRectangle(New SolidBrush(color), New RectangleF(time_range.ScaleMapping(peak.rt, width), size.Height - h, 2, h))
            Next

            If mouse_cur.X > 0 AndAlso mouse_cur.X < size.Width Then
                Dim rt As Double = width.ScaleMapping(mouse_cur.X, time_range)
                Dim font As Font = Me.Font
                Dim label As String = $"{CInt(rt)} sec [{(rt / 60).ToString("F2")}min]"
                Dim font_size As SizeF = g.MeasureString(label)

                Call g.FillRectangle(Brushes.Red, New RectangleF(mouse_cur.X, font_size.Height + 5, 2, size.Height))
                Call g.DrawString(label, font, Brushes.Red, New PointF(mouse_cur.X - font_size.Width / 2, 1))
            End If

            canvasXIC.BackgroundImage = g.ImageResource
        End Using
    End Sub

    Private Sub XICFeatureViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Timer1.Interval = 100
        'Timer1.Enabled = True
        'Timer1.Start()

        ResizeRedraw = True
        DoubleBuffered = False
    End Sub

    'Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

    'End Sub

    Private Sub canvasXIC_MouseMove(sender As Object, e As MouseEventArgs) Handles canvasXIC.MouseMove
        Dim cur = PointToClient(Cursor.Position)

        mouse_cur = e.Location
        highlight = True

        If cur.X < 0 OrElse cur.X > Width OrElse cur.Y < 0 OrElse cur.Y > Height Then
            highlight = False
        End If

        Call RenderViewer()
        Call System.Windows.Forms.Application.DoEvents()

        If highlight Then
            Me.BorderStyle = BorderStyle.FixedSingle
        Else
            Me.BorderStyle = BorderStyle.None
        End If

        'If Not Workbench.AppRunning Then
        '    Timer1.Stop()
        '    Timer1.Enabled = False
        'End If
    End Sub

    Private Sub canvasXIC_MouseHover() Handles canvasXIC.MouseClick
        Dim rt As Double = New DoubleRange(0, Width).ScaleMapping(mouse_cur.X, time_range)
        Dim peak As PeakMs2 = features _
            .Where(Function(i) std.Abs(i.rt - rt) < 60) _
            .OrderBy(Function(i) std.Abs(i.rt - rt)) _
            .FirstOrDefault

        highlight = True
        RenderSpectrum(peak)
    End Sub

    Private Sub RenderSpectrum(peak As PeakMs2)
        If Not peak Is Nothing Then
            Dim scale As Double = 8.5
            Dim msLib As New LibraryMatrix(peak.lib_guid, peak.mzInto)
            Dim size As New Size(PictureBox2.Width * scale, PictureBox2.Height * scale)
            Dim plot As Image = PeakAssign _
                .DrawSpectrumPeaks(msLib, size:=$"{size.Width},{size.Height}", dpi:=200) _
                .AsGDIImage

            selected_peak = peak
            PictureBox2.BackgroundImage = plot
        End If
    End Sub

    Private Sub PictureBox2_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox2.DoubleClick

    End Sub

    Public Event ViewSpectrum(spec As PeakMs2)

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        If Not selected_peak Is Nothing Then
            RaiseEvent ViewSpectrum(selected_peak)
        End If
    End Sub

    Private Sub PictureBox2_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox2.MouseMove
        highlight = True
    End Sub

    Private Sub ExportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportToolStripMenuItem.Click
        Using folder As New FolderBrowserDialog With {
            .Description = "Select a folder for export the spectrum data"
        }
            If folder.ShowDialog = DialogResult.OK Then
                Dim dir As String = folder.SelectedPath

                Call TaskProgress.RunAction(
                    run:=Sub(t)
                             Dim lines As String() = TextBox1.Text.LineTokens
                             Dim name As String = lines.First.BaseName

                             name = $"{name} [{lines(1)}] [{lines(2)}]"

                             Call t.SetInfo("export xic data table...")
                             Call XIC.SaveTo($"{dir}/XIC.csv", silent:=True)

                             Call t.SetInfo("save metadata...")
                             Call TextBox1.Text.SaveTo($"{dir}/metadata.txt")

                             Call t.SetInfo("export spectrum dataset as mgf ion file...")
                             Call features.SaveAsMgfIons(file:=$"{dir}/spectrum.mgf")

                             Call t.SetInfo("export xic plot...")
                             Call New NamedCollection(Of ChromatogramTick)(name, XIC) _
                                .TICplot(colorsSchema:="paper", gridFill:="white") _
                                .AsGDIImage _
                                .SaveAs($"{dir}/XIC.png")

                             Call t.SetInfo("done!")
                         End Sub,
                    title:="Export data...",
                    info:="Export spectrum data set...")
            End If
        End Using
    End Sub

    Public Function GetXICData() As NamedCollection(Of ChromatogramTick)
        Dim lines As String() = TextBox1.Text.LineTokens
        Dim name As String = lines.First.BaseName

        name = $"{name} [{lines(1)}] [{lines(2)}]"

        Return New NamedCollection(Of ChromatogramTick)(name, XIC.ToArray)
    End Function

    Private Sub XICFeatureViewer_MouseLeave(sender As Object, e As EventArgs) Handles Me.MouseLeave
        highlight = False

        If highlight Then
            Me.BorderStyle = BorderStyle.FixedSingle
        Else
            Me.BorderStyle = BorderStyle.None
        End If
    End Sub

    Private Sub XICFeatureViewer_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Call RenderViewer()
    End Sub
End Class
