Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.MSImagingViewerV2

Public Class frmMSIHistoryList

    Dim _list As New Queue(Of MSIRenderHistory)
    Dim _dims As Size
    Dim WithEvents _current As MSIRenderHistory

    Public Property MaxHistoryQueueSize As Integer = 10
    Public ReadOnly Property CurrentRender As MSIRenderHistory
        Get
            Return _current
        End Get
    End Property

    Public Sub Clear(dims As Size)
        _dims = dims
        _list.Clear()

        If Not _current Is Nothing Then
            _current.Dispose()
            _current = Nothing
        End If
    End Sub

    Public Sub Add(history As MSIRenderHistory, mzdiff As Tolerance)
        _current = history
        _list.Enqueue(history)

        If _list.Count > MaxHistoryQueueSize Then
            Dim target = _list.Dequeue

            Call FlowLayoutPanel1.Controls.Remove(target)
            Call target.Dispose()
        End If

        Call FlowLayoutPanel1.Controls.Add(history)

        AddHandler history.TitleUpdated, AddressOf _current_TitleUpdated
        AddHandler history.ExportMatrixCDF, AddressOf exportCDF
        AddHandler history.LoadViewer, AddressOf MakeRendering

        history.mzdiff = mzdiff
        history.Width = FlowLayoutPanel1.Width * 0.95
    End Sub

    Private Sub frmMSIHistoryList_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True

        Me.WindowState = FormWindowState.Minimized
        Me.Visible = False
    End Sub

    Private Sub _current_TitleUpdated(card As MSIRenderHistory, title As String) Handles _current.TitleUpdated
        ToolTip1.SetToolTip(card, title)
    End Sub

    Private Sub MakeRendering(card As MSIRenderHistory)
        Call ProgressSpinner.DoLoading(
            Sub()
                If card.rgb Is Nothing Then
                    Call WindowModules.msImageParameters.viewer.SetIonsHistory(card)
                Else
                    ' rgb rendering
                    Call WindowModules.msImageParameters.viewer.SetRgbHistory(card)
                End If
            End Sub)
    End Sub

    Private Sub exportCDF(card As MSIRenderHistory)
        Call frmMsImagingViewer.SaveMatrixCDF(card.data, _dims, card.rgb, card.mzdiff)
    End Sub

    Private Sub FlowLayoutPanel1_Resize(sender As Object, e As EventArgs) Handles FlowLayoutPanel1.Resize
        Dim w As Integer = FlowLayoutPanel1.Width * 0.95

        For Each card As MSIRenderHistory In FlowLayoutPanel1.Controls
            card.Width = w
        Next
    End Sub
End Class