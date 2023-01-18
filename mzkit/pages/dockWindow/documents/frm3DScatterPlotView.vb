Imports Mzkit_win32.BasicMDIForm

Public Class frm3DScatterPlotView

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{Workbench.WebPort}/"
        End Get
    End Property

    Private Sub frm3DScatterPlotView_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "View Scatter Plot"
    End Sub


End Class