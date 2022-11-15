Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP

Public Class frmCFMIDOutputViewer

    Dim msp As MspData()

    Public Sub SetCFMIDoutput(data As IEnumerable(Of MspData))
        msp = data.ToArray

    End Sub

    ''' <summary>
    ''' search spectrum
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click

    End Sub

    ''' <summary>
    ''' view spectrum
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click

    End Sub

    Private Sub frmCFMIDOutputViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Me.Text

        ApplyVsTheme(ContextMenuStrip1)
    End Sub
End Class