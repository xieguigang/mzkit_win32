Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Text
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Excel = Microsoft.VisualBasic.MIME.Office.Excel.XLSX

Public Class SelectSheetName

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Strings.Trim(ComboBox1.Text).StringEmpty Then
            MessageBox.Show("A table sheet name is required!", "No table sheet is selected", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            DialogResult = DialogResult.OK
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Public Shared Sub OpenExcel(fileName As String)
        If fileName.ExtensionSuffix("csv") Then
            Call showFile(File.Load(fileName), fileName.FileName)
        ElseIf fileName.ExtensionSuffix("txt", "tsv") Then
            Call showFile(File.LoadTsv(fileName, Encodings.UTF8), fileName.FileName)
        Else
            Dim getSheetName As New SelectSheetName
            Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)
            Dim names As String() = Excel.GetSheetNames(fileName)

            For Each name As String In names
                Call getSheetName.ComboBox1.Items.Add(name)
            Next

            If mask.ShowDialogForm(getSheetName) = DialogResult.OK Then
                Dim sheetName As String = getSheetName.ComboBox1.Text
                Dim table = Excel.ReadTableAuto(fileName, sheetName:=sheetName)

                Call showFile(table, $"{fileName.FileName}[{sheetName}]")
            End If
        End If
    End Sub

    Public Shared Sub showFile(table As File, title As String)
        Call WindowModules.ShowTable(DataFrame.CreateObject(table), title)
    End Sub
End Class