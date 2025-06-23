#Region "Microsoft.VisualBasic::c3bd73918006a5b3b4ba5ca4687b80ae, mzkit\src\mzkit\mzkit\pages\dockWindow\explorer\frmSRMIonsExplorer.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 188
'    Code Lines: 137
' Comment Lines: 15
'   Blank Lines: 36
'     File Size: 7.97 KB


' Class frmSRMIonsExplorer
' 
'     Function: GetFileTICOverlaps, GetIonTICOverlaps
' 
'     Sub: BPCToolStripMenuItem_Click, ClearFileSelectionsToolStripMenuItem_Click, ClearFilesToolStripMenuItem_Click, ClearIonSelectionsToolStripMenuItem_Click, frmSRMIonsExplorer_Load
'          ImportsFilesToolStripMenuItem_Click, LoadMRM, SelectAllFilesToolStripMenuItem_Click, SelectAllIonsToolStripMenuItem_Click, ShowTICOverlap3DToolStripMenuItem_Click
'          ShowTICOverlap3DToolStripMenuItem1_Click, ShowTICOverlapToolStripMenuItem1_Click, TICToolStripMenuItem_Click, Win7StyleTreeView1_AfterSelect
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.SignalReader
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Task
Imports TaskStream
Imports chromatogram = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram

Public Class frmSRMIonsExplorer

    ReadOnly filepath As New Dictionary(Of String, String)

    Dim maxrt As Double

    Private Sub ImportsFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportsFilesToolStripMenuItem.Click, ToolStripButton1.Click
        Using openfile As New OpenFileDialog With {
            .Filter = "LC-MSMS(*.mzML)|*.mzML|AB sciex wiff(*.wiff)|*.wiff",
            .Multiselect = True
        }
            If openfile.ShowDialog = DialogResult.OK Then
                Dim notMRM As New List(Of String)

                filepath.Clear()
                maxrt = 0

                Dim fileNames As String() = openfile.FileNames
                Dim check_mzML As Boolean = fileNames.All(Function(path) path.ExtensionSuffix("mzml"))
                Dim check_wiff As Boolean = fileNames.All(Function(path) path.ExtensionSuffix("wiff"))

                If check_wiff Then
                    Dim tempdir As String = $"{App.AppSystemTemp}/{App.PID}/{App.NextTempName}/"
                    Dim tempfiles As New List(Of String)

                    ' convert to mzml
                    For Each file As String In fileNames
                        Call proteowizardTask.ConvertWiffMRM(file, tempdir)
                        Call tempfiles.AddRange(tempdir.EnumerateFiles("*.mzML"))
                    Next

                    check_mzML = True
                    fileNames = tempfiles.ToArray
                ElseIf Not check_mzML Then
                    ' mzml + wiff?
                    Throw New InvalidDataException
                End If

                If check_mzML Then
                    Call ProgressSpinner.DoLoading(
                        Sub()
                            For Each file As String In fileNames
                                filepath(file.BaseName) = file

                                If RawScanParser.IsMRMData(file) Then
                                    Call LoadMRM(file)
                                Else
                                    Call Workbench.Warning($"{file} is not a MRM raw data file!")
                                    Call notMRM.Add(file.FileName)
                                End If
                            Next
                        End Sub, host:=Me)
                End If

                If notMRM.Any Then
                    MessageBox.Show($"There are {notMRM.Count} data files are not MRM data files, load of the files was ignored:" & vbCrLf & notMRM.JoinBy(vbCrLf),
                                    "MZKit MRM File Reader",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error)
                End If
            End If
        End Using
    End Sub

    Public Sub LoadMRM(data As mzPack)
        Dim sample_files = data.MS.GroupBy(Function(m) m.meta.TryGetValue(mzStreamWriter.SampleMetaName, [default]:=data.source)).ToArray
        Dim ionsLib As IonLibrary = Globals.LoadIonLibrary
        Dim display As String

        For Each sample In sample_files
            Dim TICRoot As TreeNode = Win7StyleTreeView1.Nodes.Add(sample.Key)
            Dim scan1 = sample.OrderBy(Function(d) d.rt).ToArray
            Dim tic As New BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram.Chromatogram With {
                .scan_time = scan1.Select(Function(m) m.rt).ToArray,
                .TIC = scan1.Select(Function(m) m.TIC).ToArray,
                .BPC = scan1.Select(Function(m) m.BPC).ToArray
            }
            Dim ions = scan1.Select(Function(s) s.meta.Keys) _
                .IteratesALL _
                .Distinct _
                .Where(Function(key) key.StartsWith("MRM: ")) _
                .ToArray

            TICRoot.Tag = tic
            TICRoot.ImageIndex = 0
            TICRoot.ContextMenuStrip = ContextMenuStrip1

            Dim max As Double = tic.scan_time.Max

            If max > maxrt Then
                maxrt = max
            End If

            For Each chr As String In ions
                Dim t As Double() = chr.Replace("MRM:", "") _
                    .Trim _
                    .Split("/"c) _
                    .Select(Function(d) d.Trim.ParseDouble) _
                    .ToArray
                Dim ionRef As New IonPair With {
                    .precursor = t(0),
                    .product = t(1)
                }
                Dim chromatogram As ChromatogramTick() = scan1 _
                    .Select(Function(s)
                                Dim into As Double = 0

                                If s.meta.ContainsKey(chr) Then
                                    Dim i As Integer = Integer.Parse(s.meta(chr))
                                    into = s.into(i)
                                End If

                                Return New ChromatogramTick(s.rt, into)
                            End Function) _
                    .ToArray

                display = ionsLib.GetDisplay(ionRef)

                With TICRoot.Nodes.Add(display)
                    .Tag = New MRMHolder With {.ion = ionRef, .TIC = chromatogram}
                    .ImageIndex = 1
                    .SelectedImageIndex = 1
                    .ContextMenuStrip = ContextMenuStrip2
                End With
            Next
        Next
    End Sub

    Private Class MRMHolder
        Public ion As IonPair
        Public TIC As ChromatogramTick()
    End Class

    Public Sub LoadMRM(file As String)
        Dim list = file.LoadChromatogramList.ToArray
        Dim TIC As BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram.Chromatogram = list.GetIonsChromatogram
        Dim TICRoot As TreeNode = Win7StyleTreeView1.Nodes.Add(file.FileName)

        TIC.name = file.BaseName
        TICRoot.Tag = TIC
        TICRoot.ImageIndex = 0
        TICRoot.ContextMenuStrip = ContextMenuStrip1

        Dim max = TIC.scan_time.Max

        If max > maxrt Then
            maxrt = max
        End If

        Dim ionsLib As IonLibrary = Globals.LoadIonLibrary
        Dim display As String

        For Each chr As chromatogram In list _
            .Where(Function(i)
                       Return Not (i.id.TextEquals("TIC") OrElse i.id.TextEquals("BPC"))
                   End Function)

            Dim ionRef As New IonPair With {
                .precursor = chr.precursor.MRMTargetMz,
                .product = chr.product.MRMTargetMz
            }

            display = ionsLib.GetDisplay(ionRef)

            With TICRoot.Nodes.Add(display)
                .Tag = chr
                .ImageIndex = 1
                .SelectedImageIndex = 1
                .ContextMenuStrip = ContextMenuStrip2
            End With
        Next
    End Sub

    Private Sub frmSRMIonsExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "MRM Ions"

        Call ApplyVsTheme(ContextMenuStrip1, ToolStrip1, ContextMenuStrip2, ContextMenuStrip3)
    End Sub

    Dim TIC As ChromatogramTick()
    Dim title As String

    Private Sub Win7StyleTreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterSelect
        Dim ticks As ChromatogramTick()

        If TypeOf e.Node.Tag Is BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram.Chromatogram Then
            ticks = DirectCast(e.Node.Tag, BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram.Chromatogram).GetTicks.ToArray
        ElseIf TypeOf e.Node.Tag Is MRMHolder Then
            Dim holder As MRMHolder = e.Node.Tag
            ticks = holder.TIC
            Dim props As New MRMROIProperty(holder.ion, ticks)
            Call VisualStudio.ShowProperties(props)
        Else
            Dim chr As chromatogram = e.Node.Tag
            ticks = chr.Ticks
            Dim proper As New MRMROIProperty(chr)

            Call VisualStudio.ShowProperties(proper)
        End If

        TIC = ticks
        title = e.Node.Text

        Call MyApplication.host.mzkitTool.ShowMRMTIC(e.Node.Text, ticks, maxrt)
    End Sub

    Private Sub TICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TICToolStripMenuItem.Click
        Call MyApplication.host.mzkitTool.TIC(GetFileTICOverlaps(False).ToArray, d3:=ShowTICOverlap3DToolStripMenuItem.Checked)
    End Sub

    Private Sub BPCToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BPCToolStripMenuItem.Click
        Call MyApplication.host.mzkitTool.TIC(GetFileTICOverlaps(True).ToArray, d3:=ShowTICOverlap3DToolStripMenuItem.Checked)
    End Sub

    ''' <summary>
    ''' get ions TIC
    ''' </summary>
    ''' <returns></returns>
    Private Iterator Function GetIonTICOverlaps() As IEnumerable(Of NamedCollection(Of ChromatogramTick))
        For Each rawfile As TreeNode In Win7StyleTreeView1.Nodes
            Dim fileName As String = rawfile.Text.BaseName

            For Each obj As TreeNode In rawfile.Nodes
                If Not obj.Checked Then
                    Continue For
                End If

                If TypeOf obj.Tag Is chromatogram Then
                    With DirectCast(obj.Tag, chromatogram)
                        Yield New NamedCollection(Of ChromatogramTick)($"[{fileName}] {obj.Text}", .Ticks)
                    End With
                Else
                    With DirectCast(obj.Tag, MRMHolder)
                        Yield New NamedCollection(Of ChromatogramTick)($"[{fileName}] {obj.Text}", .TIC)
                    End With
                End If
            Next
        Next
    End Function

    Private Iterator Function GetFileTICOverlaps(bpc As Boolean) As IEnumerable(Of NamedCollection(Of ChromatogramTick))
        For Each rawfile As TreeNode In Win7StyleTreeView1.Nodes
            Dim fileName As String = rawfile.Text.BaseName

            If Not rawfile.Checked Then
                Continue For
            End If

            With DirectCast(rawfile.Tag, BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram.Chromatogram)
                Yield New NamedCollection(Of ChromatogramTick)(fileName, .GetTicks(isbpc:=bpc))
            End With
        Next
    End Function

    Private Sub ShowTICOverlap3DToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowTICOverlap3DToolStripMenuItem.Click
        ShowTICOverlap3DToolStripMenuItem.Checked = Not ShowTICOverlap3DToolStripMenuItem.Checked
    End Sub

    Private Sub ClearFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearFilesToolStripMenuItem.Click, ClearFilesToolStripMenuItem1.Click
        Call Win7StyleTreeView1.Nodes.Clear()
    End Sub

    ''' <summary>
    ''' ions
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ShowTICOverlapToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ShowTICOverlapToolStripMenuItem1.Click
        Call MyApplication.host.mzkitTool.TIC(GetIonTICOverlaps.ToArray)
    End Sub

    ''' <summary>
    ''' ions
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ShowTICOverlap3DToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ShowTICOverlap3DToolStripMenuItem1.Click
        Call MyApplication.host.mzkitTool.TIC(GetIonTICOverlaps.ToArray, d3:=True)
    End Sub

    Private Sub ClearIonSelectionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearIonSelectionsToolStripMenuItem.Click
        Dim file = Win7StyleTreeView1.SelectedNode

        If Not file Is Nothing Then
            For i As Integer = 0 To file.Nodes.Count - 1
                file.Nodes(i).Checked = False
            Next
        End If
    End Sub

    Private Sub SelectAllIonsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllIonsToolStripMenuItem.Click
        Dim file = Win7StyleTreeView1.SelectedNode

        If Not file Is Nothing Then
            For i As Integer = 0 To file.Nodes.Count - 1
                file.Nodes(i).Checked = True
            Next
        End If
    End Sub

    Private Sub SelectAllFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllFilesToolStripMenuItem.Click
        For Each node As TreeNode In Win7StyleTreeView1.Nodes
            node.Checked = True
        Next
    End Sub

    Private Sub ClearFileSelectionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearFileSelectionsToolStripMenuItem.Click
        For Each node As TreeNode In Win7StyleTreeView1.Nodes
            node.Checked = False
        Next
    End Sub

    Private Sub PeakFindingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PeakFindingToolStripMenuItem.Click
        If Not TIC.IsNullOrEmpty Then
            Call MyApplication.host.mzkitTool.ShowMRMTIC(title, TIC, maxrt)
            Call RibbonEvents.CreatePeakFinding()
        End If
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Dim chrs As New List(Of BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram.Chromatogram)

        For Each rawfile As TreeNode In Win7StyleTreeView1.Nodes
            If rawfile.Checked Then
                Call chrs.Add(rawfile.Tag)
            End If
        Next

        If chrs.Count = 0 Then
            If MessageBox.Show("No data files was selected in the MRM file explorer for run the batch ion targetted processing, select all files for run the batch processing?",
                               "No Files For Processing",
                               MessageBoxButtons.OKCancel,
                               MessageBoxIcon.Exclamation) = DialogResult.OK Then
                ' select all files
                For Each rawfile As TreeNode In Win7StyleTreeView1.Nodes
                    Call chrs.Add(rawfile.Tag)
                Next
            Else
                Return
            End If
        End If

        Dim files As String() = chrs.Select(Function(c) filepath(c.name)).ToArray
        Dim ionsLib As IonLibrary = IonLibrary.LoadFile(New Configuration.Settings().MRMLibfile.ParentPath & $"/MRM/{setLibName}.csv")
        Dim workdir As String = TempFileSystem.GetAppSysTempFile(".html", App.PID, "batch_MRM_workdir").ParentPath

        If ionsLib.IsEmpty Then
            If MessageBox.Show("No ion pairs information was founded in the MRM ions library, please config the ions library at first!",
                            "No Ions Library",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Exclamation) Then

                Call VisualStudio.ShowSingleDocument(Of frmMRMLibrary)(Nothing)
            End If

            Return
        End If

        Call files.SaveTo($"{workdir}/files.txt")
        Call ionsLib.AsEnumerable.SaveTo($"{workdir}/ions.csv", silent:=True)

        ' call background task to run the batch processing
        If RscriptProgressTask.ExportMRMPeaks($"{workdir}/files.txt", $"{workdir}/ions.csv", workdir) Then
            Dim check1 = $"{workdir}/MRMIons.csv"
            Dim check2 = $"{workdir}/report.html"
            Dim check3 = $"{workdir}/peaktable.csv"

            Call RibbonEvents.OpenTable(check1)
            Call RibbonEvents.OpenTable(check3)

            ' show result html report
            Call VisualStudio.ShowDocument(Of frmHtmlViewer)(title:="MRM Report").LoadHtml(check2)
        End If
    End Sub

    Private Sub ToolStripLabel1_Click(sender As Object, e As EventArgs) Handles ToolStripLabel1.Click
        Dim repo As String = New Configuration.Settings().MRMLibfile.ParentPath
        Dim libfiles = repo.ListFiles("*.csv")
        Dim names As String() = libfiles.Select(Function(a) a.BaseName).ToArray

        Call SelectSheetName.SelectName(names, AddressOf updateIonNameDisplay)
    End Sub

    Dim setLibName As String

    Private Sub updateIonNameDisplay(libname As String)
        Dim libfile As String = New Configuration.Settings().MRMLibfile.ParentPath & $"/MRM/{libname}.csv"
        Dim ionsLib As IonLibrary = IonLibrary.LoadFile(libfile)

        setLibName = libname

        For Each sample As TreeNode In Win7StyleTreeView1.Nodes
            For Each ionNode As TreeNode In sample.Nodes
                Dim ion As MRMHolder = DirectCast(ionNode.Tag, MRMHolder)
                Dim display = ionsLib.GetDisplay(ion.ion)

                ionNode.Text = display
            Next
        Next
    End Sub
End Class
