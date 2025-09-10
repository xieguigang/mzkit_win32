Imports System.IO
Imports System.Text
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports any = Microsoft.VisualBasic.Scripting

Public Class frmMoleculeLibrary : Implements IFileReference, ISaveHandle

    Public Property libcsvfile As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {.Details = "Metabolite Library", .FileExt = ".csv", .MIMEType = "application/csv", .Name = "Metabolite Library"}
            }
        End Get
    End Property

    Private Sub frmMoleculeLibrary_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call ApplyVsTheme(ToolStrip1, AdvancedDataGridViewSearchToolBar1)

        If ToolStripComboBox1.Items.Count > 0 Then
            ToolStripComboBox1.SelectedIndex = 0
        End If

        ToolStripComboBox1_SelectedIndexChanged(Nothing, Nothing)
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
        InputDialog.Input(Of InputImportsMetaboliteLibrary)(
            Sub(args)
                Dim data As MetaInfoTable() = args.GetSource.ToArray

            End Sub)
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
        If Not libcsvfile.StringEmpty Then
            Call Process.Start(libcsvfile.ParentPath)
        End If
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
        Dim libfile As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/metabolites/{name}.csv"

        libcsvfile = libfile

        Call LoadLibrary()
    End Sub

    Dim loader As GridLoaderHandler
    Dim search As GridSearchHandler

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
        Dim rows As New List(Of MetaInfoTable)
        Dim src As BindingSource = AdvancedDataGridView1.DataSource
        Dim headers As New List(Of String)

        For i As Integer = 0 To AdvancedDataGridView1.Columns.Count - 1
            Call headers.Add(AdvancedDataGridView1.Columns(i).HeaderText)
        Next

        Dim id As Integer = headers.IndexOf(NameOf(MetaInfoTable.Id))
        Dim name As Integer = headers.IndexOf(NameOf(MetaInfoTable.CommonName))
        Dim formula As Integer = headers.IndexOf(NameOf(MetaInfoTable.Formula))
        Dim cas As Integer = headers.IndexOf(NameOf(MetaInfoTable.cas))
        Dim kegg As Integer = headers.IndexOf(NameOf(MetaInfoTable.kegg))
        Dim hmdb As Integer = headers.IndexOf(NameOf(MetaInfoTable.hmdb))
        Dim ds As System.Data.DataSet = src.DataSource
        Dim table As DataTable = ds.Tables.Item(src.DataMember)

        For j As Integer = 0 To table.Rows.Count - 1
            Dim rowObj As DataRow = table.Rows(j)

            Try
                Dim vec = rowObj.ItemArray

                If Not vec.IsNullOrEmpty Then
                    Call rows.Add(New MetaInfoTable With {
                        .cas = any.ToString(vec.ElementAtOrNull(cas)),
                        .CommonName = any.ToString(vec.ElementAtOrNull(name)),
                        .Formula = any.ToString(vec.ElementAtOrNull(formula)),
                        .hmdb = any.ToString(vec.ElementAtOrNull(hmdb)),
                        .Id = any.ToString(vec.ElementAtOrNull(id)),
                        .kegg = any.ToString(vec.ElementAtOrNull(kegg)),
                        .ExactMass = FormulaScanner.EvaluateExactMass(.Formula)
                    })
                End If
            Catch ex As Exception
                Call Workbench.Warning(ex.ToString)
            End Try
        Next

        Return rows.SaveTo(file, New Arguments With {.encoding = encoding, .silent = True, .strict = False})
    End Function
End Class