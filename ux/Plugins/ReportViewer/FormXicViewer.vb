Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Mzkit_win32.LCMSViewer

Public Class FormXicViewer

    Dim WithEvents viewer As ROIGroupViewer

    Private Sub FormXicViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        viewer = New ROIGroupViewer
        Controls.Add(viewer)
        viewer.Dock = DockStyle.Fill
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Async Function LoadXic(samples As IEnumerable(Of NamedCollection(Of ms1_scan)), mz As Double, rt As Double) As Task
        Await viewer.LoadROIs(mz, rt, samples)
    End Function
End Class