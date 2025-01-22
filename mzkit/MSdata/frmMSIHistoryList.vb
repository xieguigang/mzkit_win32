Imports System.ComponentModel
Imports Mzkit_win32.MSImagingViewerV2

Public Class frmMSIHistoryList

    Dim _list As New Queue(Of MSIRenderHistory)
    Dim WithEvents _current As MSIRenderHistory

    Public Property MaxHistoryQueueSize As Integer = 10
    Public ReadOnly Property CurrentRender As MSIRenderHistory
        Get
            Return _current
        End Get
    End Property

    Public Sub Add(history As MSIRenderHistory)
        _current = history
        _list.Enqueue(history)

        If _list.Count > MaxHistoryQueueSize Then
            Dim target = _list.Dequeue

            Call FlowLayoutPanel1.Controls.Remove(target)
            Call target.Dispose()
        End If

        Call FlowLayoutPanel1.Controls.Add(history)

        AddHandler history.TitleUpdated, AddressOf _current_TitleUpdated
        history.Width = FlowLayoutPanel1.Width * 0.95
        history.Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right
    End Sub

    Private Sub frmMSIHistoryList_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True

        Me.WindowState = FormWindowState.Minimized
        Me.Visible = False
    End Sub

    Private Sub _current_TitleUpdated(card As MSIRenderHistory, title As String) Handles _current.TitleUpdated
        ToolTip1.SetToolTip(card, title)
    End Sub
End Class