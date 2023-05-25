Public Class InputMSIPeakTableParameters

    Public ReadOnly Property TrIQCutoff As Double
        Get
            Return Val(TextBox3.Text)
        End Get
    End Property

    Public ReadOnly Property IntoCutoff As Double
        Get
            Return Val(TextBox2.Text)
        End Get
    End Property

    Public ReadOnly Property Mzdiff As Double
        Get
            Return Val(TextBox1.Text)
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class