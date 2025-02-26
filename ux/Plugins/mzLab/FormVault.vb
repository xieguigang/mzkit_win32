Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib.Validation
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.Framework.IO.CSVFile
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.Information
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Public Class FormVault

    Dim stdlib As SpectrumReader
    Dim spectrum As PeakMs2
    Dim allMass As MassIndex()
    Dim libfile As String

    Public Sub OpenDatabase()
        Using file As New OpenFileDialog With {.Filter = "any file(*.*)|*.*"}
            If file.ShowDialog = DialogResult.OK Then
                If Not stdlib Is Nothing Then
                    Call stdlib.Dispose()
                End If

                stdlib = New SpectrumReader(file.OpenFile)
                libfile = file.FileName
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

        allMass = stdlib.LoadMass.ToArray
        loadMetabolites(allMass, tree, println)
    End Sub

    Private Sub loadMetabolites(allMass As MassIndex(), tree As TreeNode, println As Action(Of String))
        Dim i As i32 = 1

        For Each mass As MassIndex In allMass
            Dim metabolite = tree.Nodes.Add(mass.name & $" [{mass.size} spectrum]")
            metabolite.Tag = mass

            If allMass.Length Mod (++i) = 0 Then
                Call println(mass.ToString)
            End If

            Call Application.DoEvents()
        Next
    End Sub

    Private Sub FormVault_Load(sender As Object, e As EventArgs) Handles Me.Load
        HookOpen = AddressOf OpenDatabase

        Text = "Library Viewer"
        TabText = Text

        Call ApplyVsTheme(ContextMenuStrip1, ContextMenuStrip2, ToolStrip1)
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
        Dim mat As New LibraryMatrix With {
            .ms2 = spectrum.mzInto,
            .name = $"{node.Text} {spectrum.lib_guid} {spectrum.mz}@{spectrum.rt}"
        }
        Dim img As Image = PeakAssign.DrawSpectrumPeaks(mat, size:="1920,1080", dpi:=200).AsGDIImage
        Dim props As New Dictionary(Of String, Object)
        Dim into As Vector = New Vector(spectrum.mzInto.Select(Function(m) m.intensity))

        Call props.Add("id", spectrum.lib_guid)
        Call props.Add("precursor_mz", spectrum.mz)
        Call props.Add("rt", spectrum.rt)
        Call props.Add("npeaks", spectrum.mzInto.Length)
        Call props.Add("entropy", (into / into.Sum).ShannonEntropy)
        Call props.Add("basePeak_mz", spectrum.mzInto.OrderByDescending(Function(m) m.intensity).First.mz)
        Call props.Add("total_ions", into.Sum)

        Me.spectrum = spectrum
        Me.PictureBox1.BackgroundImage = img

        Call Workbench.ShowProperties(DynamicType.Create(metadata:=props))
    End Sub

    Private Sub SearchInSampleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchInSampleToolStripMenuItem.Click
        If spectrum Is Nothing Then
            Return
        End If

        Dim doc As SpectrumSearchPage = SpectrumSearchModule.ShowDocument

        Call doc.LoadMs2(spectrum)
        Call doc.RunSearch()
    End Sub

    Private Sub ExportMGFIonsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportMGFIonsToolStripMenuItem.Click
        Using file As New SaveFileDialog With {.Filter = "MGF Ions(*.mgf)|*.mgf"}
            If file.ShowDialog = DialogResult.OK Then
                Dim node = Win7StyleTreeView1.SelectedNode

                Using mgf As New StreamWriter(file.FileName.Open(FileMode.OpenOrCreate, doClear:=True))
                    If TypeOf node.Tag Is MassIndex Then
                        Call SaveMass(node.Tag, mgf)
                    Else
                        Call TaskProgress.RunAction(
                            run:=Sub(proc As ITaskProgress)
                                     Call proc.SetProgressMode()
                                     Call proc.SetProgress(0)

                                     ' export for all metabolites
                                     For i As Integer = 0 To node.Nodes.Count - 1
                                         Dim metabo = node.Nodes.Item(i)
                                         Dim mass As MassIndex = metabo.Tag

                                         Call SaveMass(mass, mgf)
                                         Call proc.SetProgress(i / node.Nodes.Count * 100)
                                         Call proc.SetInfo(mass.name)
                                     Next
                                 End Sub,
                            title:="Export Mgf Ions",
                            info:="Export spectrum to mgf file..."
                        )
                    End If

                    Call mgf.Flush()
                End Using

                Call MessageBox.Show($"Spectrum data has been export to mgf file: {file.FileName}!",
                                     "Export Spectrum",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Private Iterator Function SaveRows(mass As MassIndex) As IEnumerable(Of RowObject)
        Dim spectrum_data As PeakMs2() = mass.spectrum _
            .Select(Function(i)
                        Return SpectrumReader.GetSpectrum(stdlib.GetSpectrum(i))
                    End Function) _
            .ToArray

        For Each ion As PeakMs2 In spectrum_data
            Dim row As New List(Of String) From {
                mass.name, mass.formula, mass.exactMass
            }

            Call row.Add(ion.lib_guid)
            Call row.Add(ion.precursor_type)
            Call row.Add(ion.mz)
            Call row.Add(ion.rt)
            Call row.Add(ion.scan)
            Call row.Add(ion.file)
            Call row.Add(ion.mzInto.OrderByDescending(Function(a) a.intensity).Select(Function(m) $"{m.mz}_{m.intensity}").JoinBy(" "))

            Yield New RowObject(row)
        Next
    End Function

    ''' <summary>
    ''' export for a specific metabolite
    ''' </summary>
    ''' <param name="mass"></param>
    ''' <param name="mgf"></param>
    Private Sub SaveMass(mass As MassIndex, mgf As TextWriter)
        Dim spectrum_data As Ions() = mass.spectrum _
            .Select(Function(i)
                        Return SpectrumReader.GetSpectrum(stdlib.GetSpectrum(i)).MgfIon
                    End Function) _
            .ToArray
        Dim title As String = $"{mass.name} ({mass.formula})"

        For Each ion As Ions In spectrum_data
            ion.Title = title
            ion.WriteAsciiMgf(mgf)
        Next
    End Sub

    Private Sub ToolStripSpringTextBox1_TextChanged(sender As Object, e As EventArgs) Handles ToolStripSpringTextBox1.TextChanged
        Dim text As String = Strings.Trim(ToolStripSpringTextBox1.Text)

        If text.StringEmpty Then
            Return
        End If

        Dim ref As Integer() = text.Select(Function(ch) AscW(ch)).ToArray
        Dim filter = allMass.AsParallel _
            .Select(Function(m) (LevenshteinDistance.ComputeDistance(ref, m.name), m)) _
            .Where(Function(m) Not m.Item1 Is Nothing) _
            .OrderByDescending(Function(i) i.Item1.MatchSimilarity) _
            .Take(100) _
            .ToArray
        Dim root = Win7StyleTreeView1.Nodes.Item(0)

        Call root.Nodes.Clear()

        For Each hit In filter
            Dim mass = hit.m
            Dim metabolite = root.Nodes.Add(mass.name & $" [{mass.size} spectrum]")

            metabolite.Tag = mass

            'If allMass.Length Mod (++i) = 0 Then
            '    Call println(mass.ToString)
            'End If

            Call Application.DoEvents()
        Next
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        ToolStripSpringTextBox1.Text = Nothing

        If allMass.IsNullOrEmpty Then
            Return
        ElseIf Win7StyleTreeView1.Nodes.Count = 0 Then
            Return
        End If

        Call TaskProgress.RunAction(
            run:=Sub(proc As ITaskProgress)
                     Call Me.Invoke(Sub() Call loadMetabolites(allMass, tree:=Win7StyleTreeView1.Nodes(0), AddressOf proc.SetInfo))
                 End Sub,
            title:="Reload library data",
            info:="Reload the reference spectrum library..."
        )
    End Sub

    Private Sub CopyNodeLabelToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyNodeLabelToolStripMenuItem.Click
        Dim node As TreeNode = Win7StyleTreeView1.SelectedNode

        If node Is Nothing OrElse node.Tag Is Nothing Then
            Return
        End If

        Dim index As MassIndex

        If TypeOf node.Tag Is MassIndex Then
            index = node.Tag
        Else
            index = node.Parent.Tag
        End If

        Dim content As New Dictionary(Of String, String) From {
            {"name", index.name},
            {"formula", index.formula},
            {"exact_mass", index.exactMass}
        }

        Call Clipboard.Clear()
        Call Clipboard.SetText(content.ToBEncodeString)
    End Sub

    ''' <summary>
    ''' open a folder for export one peaktable file and multiple raw data mzpack for run annotation validation test
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ExportValidationDataSetToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportValidationDataSetToolStripMenuItem.Click
        Using folder As New FolderBrowserDialog With {
            .Description = "Select a folder for save validation dataset.",
            .ShowNewFolderButton = True
        }
            If folder.ShowDialog = DialogResult.OK Then
                InputDialog.Input(Of InputDataSetSize)(
                    Sub(cfg)
                        Call TaskProgress.RunAction(
                            Sub(proc As ITaskProgress)
                                Call Me.Invoke(Sub() Call ExportValidationDataSet(args:=cfg.GetParameters, dir:=folder.SelectedPath, proc:=proc))
                            End Sub, title:="Export Validation DataSet",
                                     info:="Export raw data files for run validation analysis!")

                        Call MessageBox.Show("Validation DataSet Export Job Done!",
                                             "Export validation DataSet",
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Information)
                        Call Process.Start(folder.SelectedPath)
                    End Sub)
            End If
        End Using
    End Sub

    Private Sub ExportValidationDataSet(args As DataSetParameters, dir As String, proc As ITaskProgress)
        Dim dataset As New DataSetGenerator(stdlib, args)
        Dim i As i32 = 0

        Call proc.SetProgressMode()
        Call proc.SetProgress(0)
        Call proc.SetInfo("Start to export first raw data file!")

        For Each group In dataset.ExportRawDatas
            Dim filename As String = $"{dir}/raw/{group.name}.mzPack"

            Using file As Stream = filename.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Call New mzPack With {
                    .Application = FileApplicationClass.LCMS,
                    .MS = group.value,
                    .source = group.name
                }.Write(file, version:=1)
            End Using

            Call proc.SetProgress(100 * (++i / args.RawFiles))
            Call proc.SetInfo($"Export and save [{filename}]!")
        Next

        Call proc.SetInfo("Export ions peaktable...")
        Call dataset.GetPeaktable.SaveTo($"{dir}/peakdata.csv")
    End Sub

    Private Sub CopyImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyImageToolStripMenuItem.Click
        Clipboard.Clear()
        Clipboard.SetImage(PictureBox1.BackgroundImage)
        Workbench.SuccessMessage("Spectrum image has been copy to clipboard, you could paste it to a excel to PPT file!")
    End Sub

    Private Sub ShowInSpectrumViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowInSpectrumViewerToolStripMenuItem.Click
        If spectrum IsNot Nothing Then
            Call SpectralViewerModule.ViewSpectral(spectrum)
        End If
    End Sub

    Private Sub ExportExcelTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportExcelTableToolStripMenuItem.Click
        Using file As New SaveFileDialog With {.Filter = "Excel csv table(*.csv)|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                Dim node = Win7StyleTreeView1.SelectedNode
                Dim table As New IO.File

                Call table.Add(New RowObject From {
                    "name", "formula", "exactMass",
                    "lib_guid",
                    "precursor_type",
                    "mz",
                    "rt",
                    "scan",
                    "file",
                    "mzInto"
                })

                Using txt As Stream = file.FileName.Open(FileMode.OpenOrCreate, doClear:=True)
                    If TypeOf node.Tag Is MassIndex Then
                        For Each row In SaveRows(node.Tag)
                            Call table.Add(row)
                        Next
                    Else
                        Call TaskProgress.RunAction(
                            run:=Sub(proc As ITaskProgress)
                                     Call proc.SetProgressMode()
                                     Call proc.SetProgress(0)

                                     ' export for all metabolites
                                     For i As Integer = 0 To node.Nodes.Count - 1
                                         Dim metabo = node.Nodes.Item(i)
                                         Dim mass As MassIndex = metabo.Tag

                                         For Each row In SaveRows(mass)
                                             Call table.Add(row)
                                         Next

                                         Call proc.SetProgress(i / node.Nodes.Count * 100)
                                         Call proc.SetInfo(mass.name)
                                     Next
                                 End Sub,
                            title:="Export excel table",
                            info:="Export spectrum to excel table file..."
                        )
                    End If

                    Call StreamIO.SaveDataFrame(table, txt, autoCloseFile:=False)
                    Call txt.Flush()
                End Using

                Call MessageBox.Show($"Spectrum data has been export to excel table file: {file.FileName}!",
                                     "Export Spectrum",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Information)
            End If
        End Using
    End Sub
End Class
