Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Mzkit_win32.BasicMDIForm

Public Class FormVault

    Dim stdlib As SpectrumReader
    Dim spectrum As PeakMs2

    Public Sub OpenDatabase()
        Using file As New OpenFileDialog With {.Filter = "any file(*.*)|*.*"}
            If file.ShowDialog = DialogResult.OK Then
                If Not stdlib Is Nothing Then
                    Call stdlib.Dispose()
                End If

                stdlib = New SpectrumReader(file.OpenFile)
                Win7StyleTreeView1.Nodes.Clear()

                Call TaskProgress.RunAction(
                    run:=Sub(msg)
                             Call loadMetabolites(file.FileName.FileName, msg.Echo)
                         End Sub,
                    title:="Parse Library Tree",
                    info:="Parse the raw reference spectrum library file...",
                    host:=Me
                )
                Call Workbench.StatusMessage("Parse Reference Spectrum Library Success!")
            End If
        End Using
    End Sub

    Private Sub loadMetabolites(fileName As String, println As Action(Of String))
        Dim tree = Win7StyleTreeView1.Nodes.Add(stdlib.ToString)
        Dim allMass = stdlib.LoadMass.ToArray
        Dim i As i32 = 0

        For Each mass As MassIndex In allMass
            Dim metabolite = tree.Nodes.Add(mass.name & $" [{mass.size} spectrum]")
            metabolite.Tag = mass

            If allMass.Length Mod (++i) = 0 Then
                Call println(mass.ToString)
            End If
        Next
    End Sub

    Private Sub FormVault_Load(sender As Object, e As EventArgs) Handles Me.Load
        HookOpen = AddressOf OpenDatabase

        Text = "Library Viewer"
        TabText = Text
    End Sub

    Private Sub Win7StyleTreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterSelect
        Dim node = Win7StyleTreeView1.SelectedNode

        If node Is Nothing OrElse node.Tag Is Nothing Then
            Return
        End If

        If TypeOf node.Tag Is MassIndex AndAlso node.Nodes.Count = 0 Then
            Dim mass As MassIndex = node.Tag

            For Each i As Integer In mass.spectrum
                Dim pointer = node.Nodes.Add(stdlib.Libname(i))
                pointer.Tag = i
                pointer.ImageIndex = 1
                pointer.SelectedImageIndex = 1
            Next
        End If
    End Sub

    Private Sub Win7StyleTreeView1_MouseClick(sender As Object, e As MouseEventArgs) Handles Win7StyleTreeView1.MouseClick
        Dim node = Win7StyleTreeView1.SelectedNode

        If node Is Nothing OrElse node.Tag Is Nothing Then
            Return
        End If

        If TypeOf node.Tag Is MassIndex Then
            Return
        End If

        Dim p As Integer = node.Tag
        Dim spectrum As PeakMs2 = SpectrumReader.GetSpectrum(stdlib.GetSpectrum(p))
        Dim mat As New LibraryMatrix With {.ms2 = spectrum.mzInto, .name = $"{node.Text} {spectrum.lib_guid} {spectrum.mz}@{spectrum.rt}"}
        Dim img As Image = PeakAssign.DrawSpectrumPeaks(mat, size:="1920,1080").AsGDIImage

        Me.spectrum = spectrum
        Me.PictureBox1.BackgroundImage = img
    End Sub

    Private Sub SearchInSampleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchInSampleToolStripMenuItem.Click
        If spectrum Is Nothing Then
            Return
        End If

        Dim doc As SpectrumSearchPage = SpectrumSearchModule.ShowDocument

        Call doc.LoadMs2(spectrum)
        Call doc.RunSearch()
    End Sub
End Class
