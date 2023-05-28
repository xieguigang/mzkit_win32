Public Class InputConvert10x

    Public Function GetParameters() As (spots$, h5ad$, tag$, targets As String())
        Return (TextBox1.Text, TextBox2.Text, TextBox3.Text, TextBox4.Text.ReadAllLines)
    End Function

    ''' <summary>
    ''' spots.csv
    ''' </summary>
    Private Sub TextBox1_TextChanged() Handles TextBox1.Click
        Using file As New OpenFileDialog With {.Filter = "tissue_positions_list.csv|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                TextBox1.Text = file.FileName
            End If
        End Using
    End Sub

    ''' <summary>
    ''' h5ad
    ''' </summary>
    Private Sub TextBox2_TextChanged() Handles TextBox2.Click
        Using file As New OpenFileDialog With {.Filter = "raw_feature_bc_matrix.h5|*.h5;*.h5ad"}
            If file.ShowDialog = DialogResult.OK Then
                TextBox2.Text = file.FileName
            End If
        End Using
    End Sub

    ''' <summary>
    ''' targets.txt
    ''' </summary>
    Private Sub TextBox4_TextChanged() Handles TextBox4.Click
        Using file As New OpenFileDialog With {.Filter = "gene Names(*.txt)|*.txt"}
            If file.ShowDialog = DialogResult.OK Then
                TextBox4.Text = file.FileName
            End If
        End Using
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim pars = GetParameters()

        If Not pars.spots.FileExists Then
            Call MessageBox.Show("No tissue spot list table file!", "File Missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        ElseIf Not pars.h5ad.FileExists Then
            Call MessageBox.Show("No h5ad matrix file!", "File Missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If

        Me.DialogResult = DialogResult.OK
    End Sub
End Class