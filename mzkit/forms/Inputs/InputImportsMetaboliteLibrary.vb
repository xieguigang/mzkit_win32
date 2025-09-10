Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.MIME.Office
Imports Microsoft.VisualBasic.Text
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Public Class InputImportsMetaboliteLibrary

    Shared ReadOnly rtf As String =
        <rtf>
{\rtf1\ansi\ansicpg936\deff0\nouicompat\deflang1033\deflangfe2052{\fonttbl{\f0\fnil\fcharset0 Calibri;}{\f1\fnil Calibri;}}
{\*\generator Riched20 10.0.20348}\viewkind4\uc1 
\pard\sl240\slmult1\f0\fs22\lang9 The data source in table format for database import should be a CSV or XLSX file with the following structure:  \par
\par
\b Mandatory fields:  \par
\b0\par
\f1\bullet  \b id\b0 : The unique identifier of the metabolite.  \par
\bullet  \b name\b0 : The name of the metabolite.  \par
\bullet  \b formula\b0 : The chemical formula of the metabolite.  \par
\par
\b Optional fields \b0 (for additional import flexibility):  \par
\par
\bullet  \i kegg\i0 : The metabolite ID in the KEGG database.  \par
\bullet  \i hmdb\i0 : The metabolite ID in the HMDB database.  \par
\bullet  \i cas\i0 : The CAS registry number of the metabolite.  \par
\par
This structured format ensures seamless integration into the database while accommodating essential and supplementary metadata.\f0\par
} 
        </rtf>

    Public Iterator Function GetSource() As IEnumerable(Of MetaInfo)
        Dim source As DataFrameResolver

        If TextBox1.Text.ExtensionSuffix("csv") Then
            source = DataFrameResolver.Load(TextBox1.Text)
        Else
            source = DataFrameResolver.CreateObject(Excel.XLSX.ReadTableAuto(TextBox1.Text, sheetName))
        End If

        Dim id As Integer = source.GetOrdinal("id", "ID", "Id")
        Dim name As Integer = source.GetOrdinal("name", "Name", "NAME")
        Dim formula As Integer = source.GetOrdinal("formula", "Formula", "FORMULA")
        Dim kegg As Integer = source.GetOrdinal("kegg", "KEGG")
        Dim hmdb As Integer = source.GetOrdinal("hmdb", "HMDB")
        Dim cas As Integer = source.GetOrdinal("cas", "CAS", "cas_id")

        If id < 0 Then
            MessageBox.Show("Missing the required metabolite unique id!", "Invalid table", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If name < 0 Then
            MessageBox.Show("Missing the required metabolite name!", "Invalid table", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If formula < 0 Then
            MessageBox.Show("Missing the required metabolite formula data!", "Invalid table", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Do While source.Read

        Loop
    End Function

    Private Sub InputImportsMetaboliteLibrary_Load(sender As Object, e As EventArgs) Handles Me.Load
        RichTextBox1.Rtf = rtf.Trim(" "c, ASCII.CR, ASCII.LF, ASCII.TAB)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not TextBox1.Text.FileExists(True) Then
            Call MessageBox.Show("Data source is not defined or file not found. Please select a valid table source at first!", "No data source", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Dim sheetName As String

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Using file As New OpenFileDialog With {.Filter = "Excel Table(*.csv;*.xlsx)|*.csv;*.xlsx"}
            If file.ShowDialog = DialogResult.OK Then
                If file.FileName.ExtensionSuffix("xlsx") Then
                    Call SelectSheetName.SelectSheetTableName(
                        file.FileName, Sub(sheet)
                                           sheetName = sheet
                                           TextBox1.Text = file.FileName
                                           Label2.Text = $"({sheetName})"
                                       End Sub)
                Else
                    TextBox1.Text = file.FileName
                    Label2.Text = ""
                End If
            End If
        End Using
    End Sub
End Class