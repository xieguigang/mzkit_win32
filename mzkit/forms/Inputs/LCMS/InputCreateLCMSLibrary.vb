Public Class InputCreateLCMSLibrary

    Public ReadOnly Property LibraryName As String
        Get
            Return TextBox1.Text.NormalizePathString(alphabetOnly:=False)
        End Get
    End Property

    Public ReadOnly Property FromImports As String
        Get
            Return TextBox2.Text
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text.StringEmpty Then
            MessageBox.Show("The library name could not be empty!", "No library name", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Me.DialogResult = DialogResult.OK
    End Sub

    Private Async Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Await SelectLibFile()
    End Sub

    Private Function SelectLibFile() As Threading.Tasks.Task
        Dim [select] As Action =
            Sub()
                Using file As New OpenFileDialog With {.Filter = "ASCII spectrum library file(*.msp)|*.msp"}
                    If file.ShowDialog = DialogResult.OK Then
                        TextBox2.Text = file.FileName
                    End If
                End Using
            End Sub

        Return Threading.Tasks.Task.Run([select])
    End Function
End Class