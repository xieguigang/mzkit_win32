Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports Mzkit_win32.BasicMDIForm

Public Class frmMoleculeLibrary : Implements IFileReference, ISaveHandle

    Public Property libcsvfile As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {}
        End Get
    End Property

    Private Sub frmMoleculeLibrary_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call ApplyVsTheme(ToolStrip1, AdvancedDataGridViewSearchToolBar1)
    End Sub

    ''' <summary>
    ''' add new
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim name As String = InputBox("Please input the library name of the new custom metabolite library:", "Add New Metabolite Library")

        If name.StringEmpty(, True) Then
            Return
        Else
            AdvancedDataGridView1.Rows.Clear()
            ToolStripComboBox1.Items.Add(name)
            ToolStripComboBox1.SelectedItem = name

            ToolStripComboBox1_SelectedIndexChanged(Nothing, Nothing)
        End If
    End Sub

    ''' <summary>
    ''' imports table file
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click

    End Sub

    ''' <summary>
    ''' save
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Call SaveDocument()
    End Sub

    Protected Overrides Sub SaveDocument()
        If libcsvfile.StringEmpty(, True) Then
            Call Workbench.Warning("Please open a library")
        Else
            Call Save(libcsvfile, Encodings.UTF8)
        End If
    End Sub

    Protected Overrides Sub OpenContainingFolder()
        Call Process.Start(libcsvfile.ParentPath)
    End Sub

    Protected Overrides Sub CopyFullPath()
        Call Clipboard.Clear()
        Call Clipboard.SetText(libcsvfile)
    End Sub

    Private Sub ToolStripComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox1.SelectedIndexChanged
        If ToolStripComboBox1.SelectedIndex < 0 Then
            Return
        End If

        Dim name As String = ToolStripComboBox1.SelectedItem.ToString
        Dim libfile As String = New Configuration.Settings().MRMLibfile.ParentPath & $"/{name}.csv"

        libcsvfile = libfile

        Call LoadLibrary()
    End Sub

    Sub LoadLibrary()

    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Using s As Stream = path.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Return Save(s, encoding)
        End Using
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function

    Public Function Save(file As Stream, encoding As Encoding) As Boolean Implements ISaveHandle.Save

    End Function
End Class