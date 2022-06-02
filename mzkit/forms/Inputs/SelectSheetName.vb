Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.Office

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
        Else
            Dim getSheetName As New SelectSheetName
            Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)
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

    Private Shared Sub showFile(table As File, title As String)
        Dim dataframe As DataFrame = DataFrame.CreateObject(table)
        Dim tblView = VisualStudio.ShowDocument(Of frmTableViewer)(title:=title)

        tblView.LoadTable(Sub(grid)
                              Dim numericFields As Index(Of String) = {"mz", "rt", "rtmin", "rtmax", "mzmin", "mzmax"}
                              Dim schema As New List(Of Type)
                              Dim i As i32 = Scan0

                              For Each name As String In dataframe.HeadTitles
                                  'If name Like numericFields Then
                                  '    grid.Columns.Add(name, GetType(Double))
                                  'Else
                                  grid.Columns.Add(name, GetType(String))
                                  ' End If
                                  Dim v As String() = dataframe.Column(++i).ToArray
                                  Dim type As Type = v.SampleForType

                                  Call schema.Add(type)
                              Next

                              For Each item As RowObject In dataframe.Rows
                                  Dim values = item _
                                    .Select(Function(str, idx)
                                                Select Case schema(idx)
                                                    Case GetType(Double) : Return Val(str)
                                                    Case GetType(Integer) : Return Integer.Parse(str)
                                                    Case GetType(Boolean) : Return str.ParseBoolean
                                                    Case GetType(Date) : Return str.ParseDate
                                                    Case Else
                                                        Return CObj(str)
                                                End Select
                                            End Function) _
                                    .ToArray
                                  Dim row = grid.Rows.Add(values)
                              Next
                          End Sub)
    End Sub
End Class