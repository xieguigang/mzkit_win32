Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib

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

                Call loadMetabolites()
            End If
        End Using
    End Sub

    Private Sub loadMetabolites()
        Dim tree = Win7StyleTreeView1.Nodes.Add(stdlib.ToString)

        For Each mass As MassIndex In stdlib.LoadMass
            Call tree.Nodes.Add(mass.name & $" [{mass.size} spectrum]")
        Next
    End Sub

    Private Sub FormVault_Load(sender As Object, e As EventArgs) Handles Me.Load
        HookOpen = AddressOf OpenDatabase
    End Sub
End Class
