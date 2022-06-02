Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary

Public Class SelectSheetName

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Trim(ComboBox1.Text).StringEmpty Then
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
        Else
            Dim getSheetName As New SelectSheetName
            Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

            If mask.ShowDialogForm(getSheetName) = DialogResult.OK Then
                Dim sheetName As String = getSheetName.ComboBox1.Text

            End If
        End If
    End Sub


End Class