Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.nmrML
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmNmrSpectrumExplorer

    Dim nmrML As nmrML.XML

    ''' <summary>
    ''' load data and show explorer
    ''' </summary>
    ''' <param name="xml"></param>
    Public Sub LoadNmr(xml As String)
        Dim nmrfile = xml.LoadXml(Of nmrML.XML)
        Dim spectrums = nmrfile.spectrumList

        Win7StyleTreeView1.Nodes.Clear()

        Dim file = Win7StyleTreeView1.Nodes.Add(xml.FileName)

        nmrML = nmrfile

        For Each data As spectrumList In spectrums
            Dim spectrumNode = file.Nodes.Add(data.ToString)
            spectrumNode.Tag = data
        Next

        VisualStudio.Dock(Me, DockState.DockLeft)
    End Sub

    Private Sub frmNmrSpectrumExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "NMR Spectrum Features"

        ApplyVsTheme(ToolStrip1)
    End Sub

    Private Sub Win7StyleTreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterSelect
        If e.Node Is Nothing Then
            Return
        ElseIf TypeOf e.Node.Tag Is spectrumList Then
            Dim spectrum As spectrumList = e.Node.Tag
            Dim data = spectrum.spectrum1D(Scan0)
            Dim sw = nmrML.acquisition.First.acquisition1D.SW
            Dim matrix = data.ParseMatrix(SW:=sw)


        End If
    End Sub
End Class