Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Imaging
Imports Mzkit_win32.BasicMDIForm

Public Class FormVault

    Dim stdlib As SpectrumReader

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
                             Call loadMetabolites(msg.Echo)
                         End Sub,
                    title:="Parse Library Tree",
                    info:="Parse the raw reference spectrum library file...",
                    host:=Me
                )
                Call Workbench.StatusMessage("Parse Reference Spectrum Library Success!")
            End If
        End Using
    End Sub

    Private Sub loadMetabolites(println As Action(Of String))
        Dim tree = Win7StyleTreeView1.Nodes.Add(stdlib.ToString)

        For Each mass As MassIndex In stdlib.LoadMass
            Dim metabolite = tree.Nodes.Add(mass.name & $" [{mass.size} spectrum]")
            metabolite.Tag = mass

            For Each i As Integer In mass.spectrum
                Dim pointer = metabolite.Nodes.Add(stdlib.Libname(i))
                pointer.Tag = i
                pointer.ImageIndex = 1
                pointer.SelectedImageIndex = 1
            Next

            Call println(mass.ToString)
        Next
    End Sub

    Private Sub FormVault_Load(sender As Object, e As EventArgs) Handles Me.Load
        HookOpen = AddressOf OpenDatabase

        Text = "Library Viewer"
        TabText = Text
    End Sub

    Private Sub Win7StyleTreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterSelect

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
        Dim mat As New LibraryMatrix With {.ms2 = spectrum.mzInto, .name = $"{spectrum.lib_guid} {spectrum.mz}@{spectrum.rt}"}
        Dim img As Image = PeakAssign.DrawSpectrumPeaks(mat, size:="1920,1080").AsGDIImage
        Dim pic As PictureBox = PictureBox1

        pic.BackgroundImage = img
    End Sub
End Class
