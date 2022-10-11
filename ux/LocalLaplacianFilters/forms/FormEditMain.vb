Imports System.Globalization
Imports System.IO
Imports LaplacianHDR.Filters
Imports LaplacianHDR.Helpers
Imports UMapx.Imaging


Partial Public Class FormEditMain
    Inherits Form
#Region "Constants"
    Const application As String = "Local Laplacian filters"
    Const formats As String = "BMP|*.bmp|" & "JPEG|*.jpg; *.jpeg|" & "PNG|*.png|" & "GIF|*.gif|" & "TIFF|*.tiff"


    Const originals As String = " Invented by " & Microsoft.VisualBasic.Constants.vbLf & " Tom Mertens, Jan Kautz, Frank Van Reeth," & Microsoft.VisualBasic.Constants.vbLf & " Sylvain Paris, Samuel W. Hasinoff, Mathieu Aubry" & Microsoft.VisualBasic.Constants.vbLf & " 2007-2014 " & Microsoft.VisualBasic.Constants.vbLf & Microsoft.VisualBasic.Constants.vbLf & " Developed by Valery Asiryan" & Microsoft.VisualBasic.Constants.vbLf & " 2019-2020 " & Microsoft.VisualBasic.Constants.vbLf & Microsoft.VisualBasic.Constants.vbLf & " Powered by UMapx.NET" & Microsoft.VisualBasic.Constants.vbLf & " Valery Asiryan" & Microsoft.VisualBasic.Constants.vbLf & " 2015-2020"
#End Region

#Region "Private data"
    Private form2 As FormEnhancement = New FormEnhancement()
    Private form3 As FormTemperature = New FormTemperature()
    Private form4 As FormAdjust = New FormAdjust()
    Private form5 As FormExposureFusion = New FormExposureFusion()
    Private openFile As OpenFileDialog = New OpenFileDialog()
    Private saveFile As SaveFileDialog = New SaveFileDialog()
    Private undo As Stack(Of Bitmap) = New Stack(Of Bitmap)()
    Private redo As Stack(Of Bitmap) = New Stack(Of Bitmap)()
    Private file As String()
    Private hist As Integer()
    Private mouse As Boolean
#End Region

#Region "Form voids"
    Public Sub New()
        InitializeComponent()

        ' main
        Threading.Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US")

        KeyPreview = True
        Text = application
        Size = New Size(1280, 800)

        ' owner
        form2.Owner = Me
        form3.Owner = Me
        form4.Owner = Me
        form5.Owner = Me
        form5.TopMost = True

        ' elements
        pictureBox1.AllowDrop = True

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

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' openfile
        openFile.Filter = formats & "|All supported formats|*.bmp; *.jpg; *.jpeg; *.png; *.tiff"
        openFile.FilterIndex = 6
        openFile.RestoreDirectory = True
        openFile.Multiselect = True

        ' savefile
        saveFile.Filter = formats
        saveFile.FilterIndex = 1
        saveFile.RestoreDirectory = True

        ' spaces
        comboBox1.Items.Add(Space.YCbCr)
        comboBox1.Items.Add(Space.HSB)
        comboBox1.Items.Add(Space.HSL)
        comboBox1.Items.Add(Space.Grayscale)
        comboBox1.SelectedIndex = 0

        ' channels
        comboBox2.Items.Add("Average")
        comboBox2.Items.Add(RGBA.Red)
        comboBox2.Items.Add(RGBA.Green)
        comboBox2.Items.Add(RGBA.Blue)
        comboBox2.SelectedIndex = 0
        Return
    End Sub

    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        ' hot keys
        If e.Control AndAlso e.KeyCode = Keys.O Then
            ' Stops other controls on the form receiving event.
            e.SuppressKeyPress = True
            openToolStripMenuItem_Click(sender, e)
            Return
        ElseIf e.Control AndAlso e.KeyCode = Keys.S AndAlso saveToolStripMenuItem.Enabled Then
            e.SuppressKeyPress = True
            saveToolStripMenuItem_Click(sender, e)
            Return
        ElseIf e.Control AndAlso e.KeyCode = Keys.X AndAlso closeToolStripMenuItem.Enabled Then
            e.SuppressKeyPress = True
            closeToolStripMenuItem_Click(sender, e)
            Return
        ElseIf e.Control AndAlso e.KeyCode = Keys.R AndAlso reloadToolStripMenuItem.Enabled Then
            e.SuppressKeyPress = True
            reloadToolStripMenuItem_Click(sender, e)
            Return
        ElseIf e.Control AndAlso e.KeyCode = Keys.Z AndAlso undoToolStripMenuItem.Enabled Then
            e.SuppressKeyPress = True
            undoToolStripMenuItem_Click(sender, e)
            Return
        ElseIf e.Control AndAlso e.KeyCode = Keys.Y AndAlso redoToolStripMenuItem.Enabled Then
            e.SuppressKeyPress = True
            redoToolStripMenuItem_Click(sender, e)
            Return
        End If
        Return
    End Sub

    Private Sub openToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles openToolStripMenuItem.Click
        If openFile.ShowDialog() = DialogResult.OK Then
            TryOpen(openFile.FileNames)
        End If
        Return
    End Sub

    Private Sub exposureFusionToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        openToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub pictureBox1_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles pictureBox1.MouseDoubleClick
        openToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub reloadToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles reloadToolStripMenuItem.Click
        TryOpen(file)
        Return
    End Sub

    Private Sub closeToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles closeToolStripMenuItem.Click
        TryOpen()
        Return
    End Sub

    Private Sub saveToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles saveToolStripMenuItem.Click
        If saveFile.ShowDialog() = DialogResult.OK Then
            TrySave(saveFile.FileName, saveFile.FilterIndex)
        End If
        Return
    End Sub

    Private Sub pictureBox1_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs) Handles pictureBox1.DragDrop
        TryOpen(CType(e.Data.GetData(DataFormats.FileDrop, True), String()))
    End Sub

    Private Sub pictureBox1_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs) Handles pictureBox1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.All
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub aboutToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles aboutToolStripMenuItem.Click
        MessageBox.Show(Me, originals, application & ": About", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.Close()
    End Sub

    Private Sub enhancementToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles localLaplacianToolStripMenuItem.Click
        form2.Image = Image
        form2.Space = Space

        If form2.ShowDialog() = DialogResult.OK Then
            Processor(Image, New Filter(AddressOf form2.Apply))
        End If
        Return
    End Sub

    Private Sub temperatureToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles temperatureToolStripMenuItem.Click
        form3.Image = Image

        If form3.ShowDialog() = DialogResult.OK Then
            Processor(Image, New Filter(AddressOf form3.Apply))
        End If
    End Sub

    Private Sub hslToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles exposureToolStripMenuItem.Click
        form4.Image = Image

        If form4.ShowDialog() = DialogResult.OK Then
            Processor(Image, New Filter(AddressOf form4.Apply))
        End If
    End Sub

    Private Sub comboBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles comboBox1.SelectedIndexChanged
        Space = GetSpace(comboBox1.SelectedIndex)
    End Sub
#End Region

#Region "Histogram"
    Private Sub comboBox2_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles comboBox2.SelectedIndexChanged
        GetHistogram(Image, False)
        Return
    End Sub

    Private Sub checkBox1_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles checkBox1.CheckedChanged
        histogram4.IsLogarithmicView = checkBox1.Checked
        histogram3.IsLogarithmicView = checkBox1.Checked
        histogram2.IsLogarithmicView = checkBox1.Checked
        histogram1.IsLogarithmicView = checkBox1.Checked
        Return
    End Sub

    Private Sub histogram1_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles histogram1.MouseUp
        mouse = False
        label8.Text = Nothing
    End Sub
    Private Sub histogram1_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles histogram1.MouseUp
        mouse = True
    End Sub
    Private Sub histogram1_SelectionChanged(ByVal sender As Object, ByVal e As HistogramEventArgs) Handles histogram1.SelectionChanged
        If mouse Then
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

#Region "Private voids"
    Private Sub TryOpen(ParamArray filenames As String())
        ' length
        Dim length = filenames.Length

        ' try to open
        Try
            If length = 0 Then
                ' dispose and clear
                DisposeControls()
                ActivateControls(False)
            ElseIf length = 1 Then
                ' single image
                Processor(Open(filenames(0)), Nothing, False)
                file = New String() {filenames(0)}
                Text = application & ": " & Path.GetFileName(file(0))
                ActivateControls(True)
            Else
                ' exposure fusion
                Dim array = Open(filenames)
                form5.Images = array
                BringToFront()

                If form5.ShowDialog() = DialogResult.OK Then
                    Processor(array, New MultiFilter(AddressOf form5.Apply))
                    file = filenames
                    Text = application & ": exposure fusion (" & file.Length & " images)"
                    ActivateControls(True)
                End If
            End If

            ' clear data
            ClearStacks()
        Catch exception As Exception
            MessageBox.Show(exception.Message, application & ": Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
        End Try
    End Sub

    Private Sub TrySave(ByVal filename As String, ByVal index As Integer)
        ' try to save
        Try
            Save(Image, filename, GetImageFormat(index))
            file = New String() {filename}
            Text = application & ": " & Path.GetFileName(filename)
        Catch exception As Exception
            MessageBox.Show(exception.Message, application & ": Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
        End Try
    End Sub

    Private Sub ActivateControls(ByVal enabled As Boolean)
        ' file
        saveToolStripMenuItem.Enabled = enabled
        closeToolStripMenuItem.Enabled = enabled
        reloadToolStripMenuItem.Enabled = enabled

        ' filters
        flipHorizontalToolStripMenuItem.Enabled = enabled
        flipVerticalToolStripMenuItem.Enabled = enabled
        exposureToolStripMenuItem.Enabled = enabled
        temperatureToolStripMenuItem.Enabled = enabled
        localLaplacianToolStripMenuItem.Enabled = enabled

        ' scrolls
        button2.Enabled = enabled
        trackBar2.Enabled = enabled
        trackBar3.Enabled = enabled
        trackBar4.Enabled = enabled
        trackBar5.Enabled = enabled
        button1.Enabled = enabled
        trackBar1.Enabled = enabled

        ' stacks
        redoToolStripMenuItem.Enabled = False
        undoToolStripMenuItem.Enabled = False
    End Sub

    Private Sub GetHistogram(ByVal image As Bitmap, ByVal Optional update As Boolean = True)
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

    Private Sub DisposeControls()
        file = Nothing
        Image = Nothing
        Text = application
        label4.Text = Nothing
        label8.Text = Nothing
        pictureBox1.Image = Nothing
        histogram1.Values = Nothing
        histogram2.Values = Nothing
        histogram3.Values = Nothing
        histogram4.Values = Nothing

    End Sub

    Private Sub ClearStacks()
        undo.Clear()
        redo.Clear()

    End Sub

    Private Sub ResetAdjustments()
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

        pictureBox1.Image = Image

    End Sub

    Private Sub Processor(ByVal bitmap As Bitmap, ByVal filter As Filter, ByVal Optional cache As Boolean = True)
        ' check if null
        If bitmap IsNot Nothing Then
            MyBase.Cursor = Cursors.WaitCursor

            ' cache to stack or not?
            If cache Then
                undoToolStripMenuItem.Enabled = True
                undo.Push(Image)
                redo.Clear()
            End If
            ' apply filter or not?
            Image = If(filter IsNot Nothing, filter(bitmap), bitmap)

            ' settings
            GetHistogram(Image)
            ResetAdjustments()
            pictureBox1.Image = Image
            MyBase.Cursor = Cursors.Arrow
        End If

    End Sub

    Private Sub Processor(ByVal bitmap As Bitmap(), ByVal filter As MultiFilter)
        ' check if null
        If bitmap IsNot Nothing Then
            MyBase.Cursor = Cursors.WaitCursor
            Image = If(filter IsNot Nothing, filter(bitmap), Nothing) ' not implemented
            GetHistogram(Image)
            ResetAdjustments()
            pictureBox1.Image = Image
            MyBase.Cursor = Cursors.Arrow
        End If

    End Sub
#End Region

#Region "Adjustments"
    Private scf As SaturationContrastBrightnessFilter = New SaturationContrastBrightnessFilter()
    Public Property Image As Bitmap
    Public Property Space As Space
    Public Function Apply(ByVal image As Bitmap) As Bitmap
        ' parsing
        Dim saturation As Single = Integer.Parse(textBox1.Text)
        Dim contrast = Single.Parse(textBox2.Text) / 100.0F
        Dim brightness = Single.Parse(textBox5.Text) / 100.0F
        Dim exposure = Single.Parse(textBox4.Text) / 100.0F
        Dim gamma As Single = Math.Pow(2, -3 * Single.Parse(textBox3.Text) / 100.0F)

        ' parameters
        scf.SetParams(saturation, contrast, brightness, exposure, gamma, Space)

        ' applying filter
        Dim filter = scf.Apply(image)
        GetHistogram(filter)
        Return filter
    End Function

    Private Sub trackBar1_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles trackBar1.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar1.Value = 0
            trackBar1_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(Image)
    End Sub
    Private Sub trackBar2_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles trackBar2.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar2.Value = 0
            trackBar2_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(Image)
    End Sub
    Private Sub trackBar3_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles trackBar3.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar3.Value = 0
            trackBar3_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(Image)
    End Sub
    Private Sub trackBar4_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles trackBar4.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar4.Value = 0
            trackBar4_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(Image)
    End Sub
    Private Sub trackBar5_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles trackBar5.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar5.Value = 0
            trackBar5_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(Image)
    End Sub

    Private Sub trackBar1_Scroll(ByVal sender As Object, ByVal e As EventArgs) Handles trackBar1.Scroll
        textBox1.Text = trackBar1.Value.ToString()
    End Sub
    Private Sub trackBar2_Scroll(ByVal sender As Object, ByVal e As EventArgs) Handles trackBar2.Scroll
        textBox2.Text = trackBar2.Value.ToString()
    End Sub
    Private Sub trackBar3_Scroll(ByVal sender As Object, ByVal e As EventArgs) Handles trackBar3.Scroll
        textBox3.Text = trackBar3.Value.ToString()
    End Sub
    Private Sub trackBar4_Scroll(ByVal sender As Object, ByVal e As EventArgs) Handles trackBar4.Scroll
        textBox4.Text = trackBar4.Value.ToString()
    End Sub
    Private Sub trackBar5_Scroll(ByVal sender As Object, ByVal e As EventArgs) Handles trackBar5.Scroll
        textBox5.Text = trackBar5.Value.ToString()
    End Sub

    Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button1.Click
        Processor(CType(pictureBox1.Image, Bitmap), Nothing)

    End Sub
    Private Sub button2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button2.Click
        ResetAdjustments()

    End Sub
#End Region

#Region "Edit"
    Private flip As FlipFilter = New FlipFilter()

    Private Sub undoToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles undoToolStripMenuItem.Click
        If undoToolStripMenuItem.Enabled AndAlso undo.Count > 0 Then
            redo.Push(Image)
            Image = undo.Pop()
            redoToolStripMenuItem.Enabled = redo.Count > 0
            undoToolStripMenuItem.Enabled = undo.Count > 0
            GetHistogram(Image)
            pictureBox1.Image = Image
        End If

    End Sub
    Private Sub redoToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles redoToolStripMenuItem.Click
        If redoToolStripMenuItem.Enabled AndAlso redo.Count > 0 Then
            undo.Push(Image)
            Image = redo.Pop()
            redoToolStripMenuItem.Enabled = redo.Count > 0
            undoToolStripMenuItem.Enabled = undo.Count > 0
            GetHistogram(Image)
            pictureBox1.Image = Image
        End If
        Return
    End Sub

    Private Sub flipVerticalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles flipVerticalToolStripMenuItem.Click
        flip.SetParams(False, True)
        Processor(Image, New Filter(AddressOf flip.Apply))
    End Sub
    Private Sub flipHorizontalToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles flipHorizontalToolStripMenuItem.Click
        flip.SetParams(True, False)
        Processor(Image, New Filter(AddressOf flip.Apply))
    End Sub
#End Region
End Class

