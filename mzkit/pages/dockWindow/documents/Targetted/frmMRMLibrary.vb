#Region "Microsoft.VisualBasic::6c63df775d0545e383ce9a8b3c72d4cc, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmMRMLibrary.vb"

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

'   Total Lines: 100
'    Code Lines: 80
' Comment Lines: 1
'   Blank Lines: 19
'     File Size: 3.60 KB


' Class frmMRMLibrary
' 
'     Properties: FilePath, MimeType
' 
'     Function: (+2 Overloads) Save
' 
'     Sub: CopyFullPath, DeleteToolStripMenuItem_Click, frmMRMLibrary_Load, OpenContainingFolder, SaveDocument
'          TabPage1_KeyDown
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Windows.Forms.DataValidation.UIInteractive
Imports Mzkit_win32.BasicMDIForm
Imports any = Microsoft.VisualBasic.Scripting

''' <summary>
''' MRM
''' </summary>
Public Class frmMRMLibrary
    Implements ISaveHandle
    Implements IFileReference
    Implements MRMLibraryPage

    Public Property FilePath As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {.Details = "MRM Ion Pairs", .FileExt = ".csv", .MIMEType = "application/csv", .Name = "MRM Ion Pairs"}
            }
        End Get
    End Property

    Protected Overrides Sub OpenContainingFolder()
        If Not FilePath.StringEmpty Then
            Call Process.Start(FilePath.ParentPath)
        End If
    End Sub

    Protected Overrides Sub CopyFullPath()
        Call Clipboard.SetText(FilePath)
    End Sub

    ' HMDB0000097	Choline	103.765	60

    Private Sub TabPage1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown, DataGridView1.KeyDown
        If e.KeyCode = Keys.V AndAlso e.Control AndAlso Clipboard.ContainsText Then
            Call DataGridView1.PasteTextData()
        End If
    End Sub

    Private Sub frmMRMLibrary_Load(sender As Object, e As EventArgs) Handles Me.Load
        FilePath = Globals.Settings.MRMLibfile
        TabText = "MRM ions Library"
        Icon = My.Resources.DBFile

        If FilePath.StringEmpty(, True) Then
            FilePath = New Configuration.Settings().MRMLibfile
        End If

        Dim libfiles As String() = FilePath.ParentPath.ListFiles("*.csv").ToArray

        ToolStripComboBox1.Items.Clear()

        For Each file As String In libfiles
            Call ToolStripComboBox1.Items.Add(file.BaseName)
        Next

        Call LoadLibrary()
    End Sub

    Private Sub LoadLibrary()
        DataGridView1.Rows.Clear()

        For Each ion As IonPair In FilePath.LoadCsv(Of IonPair)
            DataGridView1.Rows.Add(ion.accession, ion.name, ion.rt, ion.precursor, ion.product)
        Next
    End Sub

    Protected Overrides Sub SaveDocument() Implements MRMLibraryPage.SaveLibrary
        Call Save(FilePath)
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Dim ions As New List(Of IonPair)
        Dim row As DataGridViewRow
        Dim ion As IonPair

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            row = DataGridView1.Rows.Item(i)
            ion = New IonPair With {
                .accession = any.ToString(row.Cells(0).Value),
                .name = any.ToString(row.Cells(1).Value),
                .rt = any.ToString(row.Cells(2).Value).ParseDouble,
                .precursor = any.ToString(row.Cells(3).Value).ParseDouble,
                .product = any.ToString(row.Cells(4).Value).ParseDouble
            }

            If ion.accession.StringEmpty AndAlso ion.name.StringEmpty Then
                Continue For
            ElseIf ion.precursor = 0.0 AndAlso ion.product = 0.0 Then
                Continue For
            End If

            ions += ion
        Next

        FilePath = path
        Globals.Settings.MRMLibfile = path.GetFullPath

        Return ions.SaveTo(path)
    End Function

    Public Function Save(s As Stream, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Throw New NotImplementedException
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        If DataGridView1.SelectedRows.Count > 0 Then
            For Each row As DataGridViewRow In DataGridView1.SelectedRows
                Try
                    DataGridView1.Rows.RemoveAt(row.Index)
                Catch ex As Exception

                End Try
            Next
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Add(id As String, name As String, q1 As Double, q2 As Double, rt As Double) Implements MRMLibraryPage.Add
        Call DataGridView1.Rows.Add(id, name, rt, q1, q2)
    End Sub

    Private Sub ImportsTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportsTableToolStripMenuItem.Click
        Using file As New OpenFileDialog With {.Filter = "Excel Table(*.csv)|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                Dim check As String() = MappingsHelper.CheckFieldMissing(Of IonPair)(file.FileName).ToArray

                If check.Any Then
                    MessageBox.Show($"The following fields are missing in the table file:{vbCrLf}{vbCrLf}" & check.JoinBy(vbCrLf) & vbCrLf & vbCrLf &
                                       "Please edit your ion table file and then try again.",
                                       "Imports Ions Table",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Error)
                    Return
                End If

                Dim unit_minute As Boolean = False

                If MessageBox.Show("Does the rt of the ion is in time data unit Seconds(Yes) or Minutes(No)?",
                                   "Set Time Unit",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.No Then
                    unit_minute = True
                End If

                Dim ions As IonPair() = file.FileName.LoadCsv(Of IonPair)
                Dim libs As IonPair() = Globals.LoadIonLibrary.AsEnumerable.ToArray

                If unit_minute Then
                    ' convert the rt from minutes to seconds
                    For Each ion As IonPair In ions
                        ion.rt *= 60
                    Next
                End If

                If libs.Any Then
                    If MessageBox.Show($"Load {ions.Length} from the table file, analso current exists {libs.Length} ions in library," & vbCrLf & "Going to merge with current library data(Yes) or replace of the current library data(No)?",
                                       "Imports Ions Table",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Information) = DialogResult.Yes Then

                        ions = ions.JoinIterates(libs).GroupBy(Function(a) a.accession).Select(Function(a) a.First).ToArray
                    End If
                End If

                Call ions.SaveTo(Globals.Settings.MRMLibfile)
                Call LoadLibrary()
            End If
        End Using
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Call ImportsTableToolStripMenuItem_Click(sender, e)
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Dim name As String = InputBox("Please input the library name of the new MRM ion set:", "Add New MRM Library")

        If name.StringEmpty(, True) Then
            Return
        Else
            DataGridView1.Rows.Clear()
            ToolStripComboBox1.Items.Add(name)
            ToolStripComboBox1.SelectedItem = name
        End If
    End Sub

    Private Sub ToolStripComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox1.SelectedIndexChanged
        If ToolStripComboBox1.SelectedIndex < 0 Then
            Return
        End If

        Dim name As String = ToolStripComboBox1.SelectedItem.ToString
        Dim libfile As String = Globals.Settings.MRMLibfile.ParentPath & $"/{name}.csv"

        FilePath = libfile
        Call LoadLibrary()
    End Sub
End Class
