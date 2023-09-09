Public Class SetMSIPlotParameters

    Public ReadOnly Property FileName As String
        Get
            Return TextBox1.Text
        End Get
    End Property

    Public Function SetFileName(filename As String) As SetMSIPlotParameters
        TextBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) & "/" & filename & ".png"
        Return Me
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using file As New SaveFileDialog With {.FileName = TextBox1.Text, .Filter = "Image File(*.png)|*.png"}
            If file.ShowDialog = DialogResult.OK Then
                TextBox1.Text = file.FileName
            End If
        End Using
    End Sub

    ''' <summary>
    ''' change the width
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        If CheckBox1.Checked Then
            ' andalso change the height

        Else

        End If
    End Sub

    ''' <summary>
    ''' change the height
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub NumericUpDown2_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown2.ValueChanged
        If CheckBox1.Checked Then
            ' and also change the width

        Else

        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class