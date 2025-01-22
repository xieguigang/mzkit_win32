Imports System.ComponentModel
Imports Mzkit_win32.MSImagingViewerV2

Public Class frmMSIHistoryList

    Dim _list As New Queue(Of MSIRenderHistory)

    Public Property MaxHistoryQueueSize As Integer = 10
    Public ReadOnly Property CurrentRender As MSIRenderHistory

    Public Sub Add(history As MSIRenderHistory)
        _CurrentRender = history
        _list.Enqueue(history)

        If _list.Count > MaxHistoryQueueSize Then
            Dim target = _list.Dequeue

            Call FlowLayoutPanel1.Controls.Remove(target)
            Call target.Dispose()
        End If

        Call FlowLayoutPanel1.Controls.Add(history)
    End Sub

    Private Sub frmMSIHistoryList_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True

        Me.WindowState = FormWindowState.Minimized
        Me.Visible = False
    End Sub
End Class