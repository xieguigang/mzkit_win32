Imports UMapx.Imaging

Partial Public Class PropertyWindow : Inherits ToolWindow

    Private hist As Integer()
    Private mouse As Boolean

    Public Property Main As FormEditMain

    Private Sub PropertyWindow_Load() Handles Me.Load
        ' histograms
        histogram1.Color = Color.DarkGray
        histogram1.AllowSelection = True

        histogram2.Color = Color.IndianRed
        histogram2.AllowSelection = False
        histogram3.Color = Color.LightGreen
        histogram3.AllowSelection = False
        histogram4.Color = Color.CornflowerBlue
        histogram4.AllowSelection = False

        ' labels
        label4.Text = Nothing
        label8.Text = Nothing

        ' channels
        comboBox2.Items.Add("Average")
        comboBox2.Items.Add(RGBA.Red)
        comboBox2.Items.Add(RGBA.Green)
        comboBox2.Items.Add(RGBA.Blue)
        comboBox2.SelectedIndex = 0

        ' trackbars

        AddHandler trackBar1.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar2.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar3.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar4.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar5.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar1.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar2.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar3.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar4.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar5.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
    End Sub

    Public Sub GetHistogram(image As Bitmap, Optional update As Boolean = True)
        ' check null
        If image IsNot Nothing Then
            ' histograms: r, g, b
            If update Then
                histogram2.Values = image.Histogram(RGBA.Red)
                histogram3.Values = image.Histogram(RGBA.Green)
                histogram4.Values = image.Histogram(RGBA.Blue)
            End If

            ' comboBox2
            Dim index = comboBox2.SelectedIndex

            ' switch
            Select Case index
                Case 1
                    hist = histogram2.Values
                Case 2
                    hist = histogram3.Values
                Case 3
                    hist = histogram4.Values
                Case Else
                    hist = image.Histogram
            End Select

            ' statistics
            Dim pixels As Double = Statistics.Sum(hist)
            Dim mean As Double = Statistics.Mean(hist)
            Dim median = Statistics.Median(hist)
            Dim std As Double = Statistics.StdDev(hist)

            ' set main histogram
            histogram1.Values = hist

            ' label
            label4.Text = mean.ToString("F2") & Microsoft.VisualBasic.Constants.vbLf & std.ToString("F2") & Microsoft.VisualBasic.Constants.vbLf & median & Microsoft.VisualBasic.Constants.vbLf & pixels
        End If
    End Sub

    Public Sub ResetAdjustments()
        trackBar5.Value = 0
        trackBar4.Value = 0
        trackBar3.Value = 0
        trackBar2.Value = 0
        trackBar1.Value = 0

        textBox5.Text = "0"
        textBox4.Text = "0"
        textBox3.Text = "0"
        textBox2.Text = "0"
        textBox1.Text = "0"

        Main.PictureImage = Main.Image
    End Sub

    Public Sub DisposeControls()
        label4.Text = Nothing
        label8.Text = Nothing
        Main.PictureImage = Nothing
        histogram1.Values = Nothing
        histogram2.Values = Nothing
        histogram3.Values = Nothing
        histogram4.Values = Nothing
    End Sub

    Public Sub ActivateControls(enabled As Boolean)

        ' scrolls
        button2.Enabled = enabled
        trackBar2.Enabled = enabled
        trackBar3.Enabled = enabled
        trackBar4.Enabled = enabled
        trackBar5.Enabled = enabled
        button1.Enabled = enabled
        trackBar1.Enabled = enabled
    End Sub

    Private Sub trackBar1_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar1.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar1.Value = 0
            trackBar1_Scroll(sender, e)
        End If
        Main.PictureImage = Main.Apply(Main.Image)
    End Sub
    Private Sub trackBar2_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar2.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar2.Value = 0
            trackBar2_Scroll(sender, e)
        End If
        Main.PictureImage = Main.Apply(Main.Image)
    End Sub
    Private Sub trackBar3_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar3.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar3.Value = 0
            trackBar3_Scroll(sender, e)
        End If
        Main.PictureImage = Main.Apply(Main.Image)
    End Sub
    Private Sub trackBar4_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar4.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar4.Value = 0
            trackBar4_Scroll(sender, e)
        End If
        Main.PictureImage = Main.Apply(Main.Image)
    End Sub
    Private Sub trackBar5_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar5.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar5.Value = 0
            trackBar5_Scroll(sender, e)
        End If
        Main.PictureImage = Main.Apply(Main.Image)
    End Sub

    Private Sub trackBar1_Scroll(sender As Object, e As EventArgs) Handles trackBar1.Scroll
        textBox1.Text = trackBar1.Value.ToString()
    End Sub
    Private Sub trackBar2_Scroll(sender As Object, e As EventArgs) Handles trackBar2.Scroll
        textBox2.Text = trackBar2.Value.ToString()
    End Sub
    Private Sub trackBar3_Scroll(sender As Object, e As EventArgs) Handles trackBar3.Scroll
        textBox3.Text = trackBar3.Value.ToString()
    End Sub
    Private Sub trackBar4_Scroll(sender As Object, e As EventArgs) Handles trackBar4.Scroll
        textBox4.Text = trackBar4.Value.ToString()
    End Sub
    Private Sub trackBar5_Scroll(sender As Object, e As EventArgs) Handles trackBar5.Scroll
        textBox5.Text = trackBar5.Value.ToString()
    End Sub

    Private Sub button1_Click(sender As Object, e As EventArgs) Handles button1.Click
        Main.Processor(CType(Main.PictureImage, Bitmap), Nothing)
    End Sub
    Private Sub button2_Click(sender As Object, e As EventArgs) Handles button2.Click
        ResetAdjustments()

    End Sub

#Region "Histogram"
    Private Sub comboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles comboBox2.SelectedIndexChanged
        GetHistogram(Main.Image, False)
    End Sub

    Private Sub checkBox1_CheckedChanged(sender As Object, e As EventArgs) Handles checkBox1.CheckedChanged
        histogram4.IsLogarithmicView = checkBox1.Checked
        histogram3.IsLogarithmicView = checkBox1.Checked
        histogram2.IsLogarithmicView = checkBox1.Checked
        histogram1.IsLogarithmicView = checkBox1.Checked
        Return
    End Sub

    Private Sub histogram1_MouseUp(sender As Object, e As MouseEventArgs) Handles histogram1.MouseUp
        Mouse = False
        label8.Text = Nothing
    End Sub
    Private Sub histogram1_MouseDown(sender As Object, e As MouseEventArgs) Handles histogram1.MouseDown
        Mouse = True
    End Sub
    Private Sub histogram1_SelectionChanged(sender As Object, e As HistogramEventArgs) Handles histogram1.SelectionChanged
        If Mouse Then
            Dim min = e.Min
            Dim max = e.Max
            Dim count = 0

            ' count pixels
            For i = min To max
                count += hist(i)
            Next

            ' print
            label8.Text = min.ToString() & "..." & max.ToString() & Microsoft.VisualBasic.Constants.vbLf & count.ToString() & Microsoft.VisualBasic.Constants.vbLf & (CSng(count) * 100 / Statistics.Sum(hist)).ToString("F2")
        End If
    End Sub
#End Region
End Class

