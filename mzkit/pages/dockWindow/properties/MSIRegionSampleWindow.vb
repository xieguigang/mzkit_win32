Imports ControlLibrary

Public Class MSIRegionSampleWindow

    Public ReadOnly Property IsNullOrEmpty As Boolean
        Get
            Return FlowLayoutPanel1.Controls.Count = 0
        End Get
    End Property

    Friend Sub Clear()
        FlowLayoutPanel1.Controls.Clear()
    End Sub

    ''' <summary>
    ''' 某一个样本区域可能是由多个不连续的区域所组成的
    ''' </summary>
    Friend Sub Add(selector As PixelSelector)
        Dim card As New RegionSampleCard

        card.SetPolygons(selector.GetPolygons)

        FlowLayoutPanel1.Controls.Add(card)
    End Sub

    Private Sub MSIRegionSampleWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Text

        Call ApplyVsTheme(ToolStrip1)
    End Sub

    Friend Function ToArray() As Rectangle()
        Throw New NotImplementedException()
    End Function
End Class