Public Class InputConfigTissueMap

    Public Property RegionPrefix As String
        Get
            Return TextBox1.Text
        End Get
        Set(value As String)
            TextBox1.Text = value
        End Set
    End Property

    Public ReadOnly Property ColorSet As String
        Get
            Return ComboBox1.SelectedItem.ToString
        End Get
    End Property

    Public Property AlphaLevel As Double
        Get
            Return TrackBar1.Value
        End Get
        Set(value As Double)
            TrackBar1.Value = value
        End Set
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DialogResult = DialogResult.OK
    End Sub

    Private Sub InputConfigTissueMap_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 0
        TextBox1.Text = "region_"
        TrackBar1.Value = 80
    End Sub
End Class