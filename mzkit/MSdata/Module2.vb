﻿Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.MoleculeNetworking
Imports BioNovoGene.mzkit_win32.MSdata
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports Mzkit_win32.BasicMDIForm
Imports WeifenLuo.WinFormsUI.Docking
Imports stdNum = System.Math

Namespace MSdata

    Module Module2

        Public Sub ShowCluster(raw As PeakMs2(), vlabel As String)
            Dim rt As Double() = raw.Select(Function(p) p.rt).ToArray
            Dim rt_scan = raw.GroupBy(Function(a) a.rt, offsets:=5).ToArray
            Dim ms1 As ScanMS1() = rt_scan _
                .Select(Function(p) p.value.ClusterScan(vlabel)) _
                .OrderBy(Function(m) m.rt) _
                .ToArray
            Dim fakePack As New mzPack With {
                .Application = FileApplicationClass.LCMS,
                .source = vlabel,
                .MS = ms1
            }
            Dim fakeRaw As New Raw(inMemory:=fakePack) With {
                .cache = Nothing,
                .numOfScan1 = ms1.Length,
                .numOfScan2 = raw.Length,
                .rtmax = rt.Max,
                .rtmin = rt.Min,
                .source = vlabel
            }

            WindowModules.rawFeaturesList.LoadRaw(fakeRaw)
            VisualStudio.Dock(WindowModules.rawFeaturesList, DockState.DockLeft)
        End Sub

        Public Sub showMasssdiff(M As Double, mz2 As Array)
            Dim pa As New PeakAnnotation(0.05, isotopeFirst:=True)
            Dim mz As ms2() = mz2
            Dim massdiff = pa.RunAnnotation(M, mz.ToArray).products
            ' show result in table viewer
            Dim tblView = VisualStudio.ShowDocument(Of frmTableViewer)(title:=$"Mass Diff Analysis[m/z {M.ToString("F4")}]")

            Call tblView.LoadTable(
                Sub(subView)
                    Call subView.Columns.Add("precursor", GetType(Double))
                    Call subView.Columns.Add("cluster_size", GetType(Double))
                    Call subView.Columns.Add("mass_diff", GetType(String))

                    For Each row As ms2 In massdiff
                        Call subView.Rows.Add(row.mz, row.intensity, row.Annotation)
                    Next
                End Sub)
        End Sub

        <Extension>
        Friend Sub MolecularNetworkingTool(raw As PeakMs2(), progress As ITaskProgress, similarityCutoff As Double)
            Dim protocol As New Protocols(
                ms1_tolerance:=Tolerance.PPM(15),
                ms2_tolerance:=Tolerance.DeltaMass(0.3),
                treeIdentical:=Globals.Settings.network.treeNodeIdentical,
                treeSimilar:=Globals.Settings.network.treeNodeSimilar,
                intoCutoff:=Globals.Settings.viewer.GetMethod
            )
            Dim progressMsg As Action(Of String) = AddressOf progress.SetTitle

            ' filter empty spectrum
            raw = (From r As PeakMs2 In raw Where Not r.mzInto.IsNullOrEmpty).ToArray
            progress.SetTitle("run molecular networking....")

            Dim links = protocol.RunProtocol(raw, progressMsg).ProduceNodes.Networking.ToArray
            Dim net As IO.DataSet() = ProtocolPipeline _
                .Networking(Of IO.DataSet)(links, Function(a, b) stdNum.Min(a, b)) _
                .ToArray

            progress.SetTitle("run family clustering....")

            If net.Length < 3 Then
                Call Workbench.Warning("the ions data is not enough for create network!")
                Return
            End If

            Dim kn As Integer

            If net.Length > 9 Then
                kn = 9
            Else
                kn = CInt(net.Length / 2)
            End If

            Dim clusters = net.ToKMeansModels.Kmeans(expected:=kn, debug:=False)
            Dim rawLinks = links.ToDictionary(Function(a) a.reference, Function(a) a)
            Dim toolPage = MyApplication.host.mzkitMNtools

            progress.SetInfo("initialize result output...")

            MyApplication.host.Invoke(
                Sub()
                    Call toolPage.loadNetworkData(clusters, protocol, rawLinks, similarityCutoff)
                    Call MyApplication.host.ShowPage(toolPage)
                End Sub)
        End Sub
    End Module
End Namespace