Imports System.Globalization
Imports System.IO
Imports LaplacianHDR.Filters
Imports LaplacianHDR.Helpers
Imports UMapx.Imaging
Imports WeifenLuo.WinFormsUI.Docking

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

    Dim canvas As ImageDocumentWindow
    Dim imageHistogram As PropertyWindow

    Private ReadOnly _toolStripProfessionalRenderer As ToolStripRenderer = New ToolStripProfessionalRenderer()

    Public Property PictureImage As Image
        Get
            Return canvas.pictureBox1.Image
        End Get
        Set(value As Image)
            canvas.pictureBox1.Image = value
        End Set
    End Property

#End Region

#Region "Form voids"

    Sub New(loadfile As String)
        Call Me.New()

        file = {loadfile}
    End Sub

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

        VisualStudioToolStripExtender1.DefaultRenderer = _toolStripProfessionalRenderer
        dockPanel.Theme = VS2015LightTheme1
        dockPanel.ShowDocumentIcon = True
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

        canvas = New ImageDocumentWindow With {.Main = Me}
        'canvas.Show(dockPanel)
        'canvas.DockState = DockState.Document
        imageHistogram = New PropertyWindow With {.Main = Me}
        imageHistogram.Show(dockPanel)
        imageHistogram.DockState = DockState.DockRight

        Me.EnableVSRenderer(VisualStudioToolStripExtender.VsVersion.Vs2015, VS2015LightTheme1)

        If Not file.IsNullOrEmpty Then
            Call TryOpen(file(Scan0))
        End If
    End Sub

    Private Sub EnableVSRenderer(ByVal version As VisualStudioToolStripExtender.VsVersion, ByVal theme As ThemeBase)
        VisualStudioToolStripExtender1.SetStyle(menuStrip1, version, theme)
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

    Public Sub openToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles openToolStripMenuItem.Click
        If openFile.ShowDialog() = DialogResult.OK Then
            TryOpen(openFile.FileNames)
        End If
    End Sub

    Private Sub exposureFusionToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        openToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub reloadToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles reloadToolStripMenuItem.Click
        TryOpen(file)
    End Sub

    Private Sub closeToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles closeToolStripMenuItem.Click
        TryOpen()
    End Sub

    Private Sub saveToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles saveToolStripMenuItem.Click
        If saveFile.ShowDialog() = DialogResult.OK Then
            TrySave(saveFile.FileName, saveFile.FilterIndex)
        End If
        Return
    End Sub

    Private Sub aboutToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles aboutToolStripMenuItem.Click
        MessageBox.Show(Me, originals, application & ": About", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
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

#Region "Private voids"
    Public Sub TryOpen(ParamArray filenames As String())
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
                canvas.TabText = file(0).FileName
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

        imageHistogram.ActivateControls(enabled)
        canvas.Show(dockPanel)
        canvas.DockState = DockState.Document

        ' stacks
        redoToolStripMenuItem.Enabled = False
        undoToolStripMenuItem.Enabled = False
    End Sub

    Private Sub DisposeControls()
        file = Nothing
        Image = Nothing
        Text = application
        canvas.TabText = ""
        canvas.DockState = DockState.Hidden
        imageHistogram.DisposeControls()
    End Sub

    Private Sub ClearStacks()
        undo.Clear()
        redo.Clear()
    End Sub

    Public Sub Processor(ByVal bitmap As Bitmap, ByVal filter As Filter, ByVal Optional cache As Boolean = True)
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
            imageHistogram.GetHistogram(Image)
            imageHistogram.ResetAdjustments()
            PictureImage = Image
            MyBase.Cursor = Cursors.Arrow
        End If
    End Sub

    Private Sub Processor(ByVal bitmap As Bitmap(), ByVal filter As MultiFilter)
        ' check if null
        If bitmap IsNot Nothing Then
            MyBase.Cursor = Cursors.WaitCursor
            Image = If(filter IsNot Nothing, filter(bitmap), Nothing) ' not implemented
            imageHistogram.GetHistogram(Image)
            imageHistogram.ResetAdjustments()
            PictureImage = Image
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
        Dim saturation As Single = Integer.Parse(imageHistogram.textBox1.Text)
        Dim contrast = Single.Parse(imageHistogram.textBox2.Text) / 100.0F
        Dim brightness = Single.Parse(imageHistogram.textBox5.Text) / 100.0F
        Dim exposure = Single.Parse(imageHistogram.textBox4.Text) / 100.0F
        Dim gamma As Single = Math.Pow(2, -3 * Single.Parse(imageHistogram.textBox3.Text) / 100.0F)

        ' parameters
        scf.SetParams(saturation, contrast, brightness, exposure, gamma, Space)

        ' applying filter
        Dim filter = scf.Apply(image)
        imageHistogram.GetHistogram(filter)
        Return filter
    End Function


#End Region

#Region "Edit"
    Private flip As FlipFilter = New FlipFilter()

    Private Sub undoToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles undoToolStripMenuItem.Click
        If undoToolStripMenuItem.Enabled AndAlso undo.Count > 0 Then
            redo.Push(Image)
            Image = undo.Pop()
            redoToolStripMenuItem.Enabled = redo.Count > 0
            undoToolStripMenuItem.Enabled = undo.Count > 0
            imageHistogram.GetHistogram(Image)
            PictureImage = Image
        End If

    End Sub
    Private Sub redoToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles redoToolStripMenuItem.Click
        If redoToolStripMenuItem.Enabled AndAlso redo.Count > 0 Then
            undo.Push(Image)
            Image = redo.Pop()
            redoToolStripMenuItem.Enabled = redo.Count > 0
            undoToolStripMenuItem.Enabled = undo.Count > 0
            imageHistogram.GetHistogram(Image)
            PictureImage = Image
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

