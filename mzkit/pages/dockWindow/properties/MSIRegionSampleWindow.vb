Public Class MSIRegionSampleWindow

    Public ReadOnly Property IsNullOrEmpty As Boolean
        Get
            Return FlowLayoutPanel1.Controls.Count = 0
        End Get
    End Property

    Friend Sub Clear()
        FlowLayoutPanel1.Controls.Clear()
    End Sub

    Friend Sub Add(regionSelectin As Rectangle)

    End Sub

    Private Sub MSIRegionSampleWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Text

        Call ApplyVsTheme(ToolStrip1)
    End Sub

    Friend Function ToArray() As Rectangle()
        Throw New NotImplementedException()
    End Function
End Class