Imports System.ComponentModel
Imports Mzkit_win32.MSImagingViewerV2

Public Class frmMSIHistoryList

    Dim list As Queue(Of MSIRenderHistory)

    Public ReadOnly Property CurrentRender As MSIRenderHistory
        Get

        End Get
    End Property

    Public Sub Add()

    End Sub

    Private Sub frmMSIHistoryList_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True

        Me.WindowState = FormWindowState.Minimized
        Me.Visible = False
    End Sub
End Class