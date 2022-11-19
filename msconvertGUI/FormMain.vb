Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports CommonDialogs
Imports msconvertGUI.My
Imports Task

Public Class FormMain

    Public Property CurrentTask As FileApplicationClass = FileApplicationClass.LCMS
    Public Property arguments As New Dictionary(Of String, String)

    ''' <summary>
    ''' start run task
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If TextBox1.Text.StringEmpty OrElse Not TextBox1.Text.DirectoryExists Then
            MessageBox.Show("Invalid source folder path!", "Source Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        ElseIf TextBox2.Text.StringEmpty Then
            MessageBox.Show("No output folder path is specificed!", "Target Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        End If

        Dim source As String = TextBox1.Text
        Dim output As String = TextBox2.Text

        If CurrentTask = FileApplicationClass.MSImaging Then
            Dim cancel As Boolean = False
            Dim baseDir As String = source
            Dim fileDirName As String = baseDir.BaseName
            Dim load As New ShowMSIRowScanSummary With {
                .files = ImportsRawData.EnumerateRawtDataFiles(source).ToArray
            }

            arguments = New Dictionary(Of String, String)

            Call InputDialog.Input(
                Sub(creator)
                    Dim cutoff As Double = creator.cutoff
                    Dim basePeak As Double = creator.matrixMz

                    Call arguments.Add("cutoff", cutoff)
                    Call arguments.Add("matrix_basepeak", basePeak)
                End Sub, cancel:=Sub() cancel = True, config:=load)

            If cancel Then
                Return
            End If
        End If

        Call MyApplication.SubmitTask(source, output, Me).Start()
    End Sub

    Public Sub AddTask(task As TaskProgress)
        Call Me.Invoke(
            Sub()
                Call FlowLayoutPanel1.Controls.Add(task)
            End Sub)
    End Sub

    ''' <summary>
    ''' open source folder
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using folder As New FolderBrowserDialog
            If folder.ShowDialog = DialogResult.OK Then
                TextBox1.Text = folder.SelectedPath

                If TextBox2.Text.StringEmpty Then
                    TextBox2.Text = TextBox1.Text
                End If
            End If
        End Using
    End Sub

    ''' <summary>
    ''' open save folder
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles DropDownButton1.Click
        Using folder As New FolderBrowserDialog
            If folder.ShowDialog = DialogResult.OK Then
                TextBox2.Text = folder.SelectedPath
                CurrentTask = FileApplicationClass.LCMS
            End If
        End Using
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Call App.Exit()
    End Sub

    Private Sub ExportMSImagingFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportMSImagingFileToolStripMenuItem.Click
        Using file As New SaveFileDialog With {
            .Filter = "BioNovoGene mzPack Binary Stream(*.mzPack)|*.mzPack|Imaging mzML(*.imzML)|*.imzML"
        }
            If file.ShowDialog = DialogResult.OK Then
                TextBox2.Text = file.FileName
                CurrentTask = FileApplicationClass.MSImaging
            End If
        End Using
    End Sub

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        InputDialog.MyApplicationHost = Me
    End Sub
End Class
