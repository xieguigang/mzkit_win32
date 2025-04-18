﻿#Region "Microsoft.VisualBasic::49c37a59d098ab3a9c68062460b57109, mzkit\msconvertGUI\FormMain.vb"

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

    '   Total Lines: 200
    '    Code Lines: 142 (71.00%)
    ' Comment Lines: 16 (8.00%)
    '    - Xml Docs: 93.75%
    ' 
    '   Blank Lines: 42 (21.00%)
    '     File Size: 7.14 KB


    ' Class FormMain
    ' 
    '     Properties: AppHost_ClientRectangle, arguments, CurrentTask, DockPanel
    ' 
    '     Function: GetClientSize, GetDesktopLocation, getSourceDir, GetWindowState
    ' 
    '     Sub: AddTask, Button1_Click, Button2_Click, Button3_Click, ExitToolStripMenuItem_Click
    '          ExportMSImagingFileToolStripMenuItem_Click, FormMain_Closing, FormMain_Load, FormMain_Resize, ListBox1_SelectedIndexChanged
    '          LogText, SetTitle, SetWindowState, SetWorkbenchVisible, ShowProperties
    '          StatusMessage, TextBox1_TextChanged, Warning
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports CommonDialogs
Imports msconvertGUI.My
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Task

Public Class FormMain : Implements AppHost

    Public Property CurrentTask As FileApplicationClass = FileApplicationClass.LCMS
    Public Property arguments As New Dictionary(Of String, String)

    Public ReadOnly Property DockPanel As Global.WeifenLuo.WinFormsUI.Docking.DockPanel Implements AppHost.DockPanel
    Private ReadOnly Property AppHost_ClientRectangle As Rectangle Implements AppHost.ClientRectangle
        Get
            Return New Rectangle(Location, Size)
        End Get
    End Property

    Private Function getSourceDir() As String
        Return TextBox1.Text
    End Function

    ''' <summary>
    ''' start run task
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If TextBox1.Text.StringEmpty OrElse Not TextBox1.Text.DirectoryExists Then
            MessageBox.Show("Invalid source folder path!", "Source Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        ElseIf TextBox2.Text.StringEmpty Then
            MessageBox.Show("No output folder path is specificed!", "Target Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        End If

        Dim source As String = getSourceDir()
        Dim output As String = TextBox2.Text

        If CurrentTask = FileApplicationClass.MSImaging Then
            Dim cancel As Boolean = False
            Dim baseDir As String = source
            Dim fileDirName As String = baseDir.BaseName
            Dim load As New ShowMSIRowScanSummary With {
                .files = ImportsRawData.EnumerateRawtDataFiles(source).ToArray
            }

            arguments = New Dictionary(Of String, String)

            Call InputDialog.Input(
                Sub(creator)
                    Dim basePeak As Double = creator.matrixMz
                    Dim cutoff As Double = creator.cutoff

                    Call arguments.Add("resolution", creator.resolution)
                    Call arguments.Add("cutoff", cutoff)
                    Call arguments.Add("matrix_basepeak", basePeak)
                    Call arguments.Add("norm", creator.norm)

                End Sub, cancel:=Sub() cancel = True, config:=load)

            If cancel Then
                Return
            End If
        End If

        Call MyApplication.SubmitTask(source, output, Me).Start()
    End Sub

    Public Sub AddTask(task As TaskProgress)
        Call Me.Invoke(
            Sub()
                FlowLayoutPanel1.Controls.Add(task)
                task.Width = FlowLayoutPanel1.Width - 15
            End Sub)
    End Sub

    ''' <summary>
    ''' open source folder
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using folder As New FolderBrowserDialog
            If folder.ShowDialog = DialogResult.OK Then
                TextBox1.Text = folder.SelectedPath

                If TextBox2.Text.StringEmpty Then
                    TextBox2.Text = TextBox1.Text
                End If
            End If
        End Using
    End Sub

    ''' <summary>
    ''' open save folder
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles DropDownButton1.Click
        Using folder As New FolderBrowserDialog
            If folder.ShowDialog = DialogResult.OK Then
                TextBox2.Text = folder.SelectedPath
                CurrentTask = FileApplicationClass.LCMS
            End If
        End Using
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Call App.Exit()
    End Sub

    Private Sub ExportMSImagingFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportMSImagingFileToolStripMenuItem.Click
        Using file As New SaveFileDialog With {
            .Filter = "BioNovoGene mzPack Binary Stream(*.mzPack)|*.mzPack|Imaging mzML(*.imzML)|*.imzML"
        }
            If file.ShowDialog = DialogResult.OK Then
                TextBox2.Text = file.FileName
                CurrentTask = FileApplicationClass.MSImaging
            End If
        End Using
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged

    End Sub

    Public Sub SetWindowState(stat As FormWindowState) Implements AppHost.SetWindowState
        WindowState = stat
    End Sub

    Public Sub Warning(msg As String) Implements AppHost.Warning

    End Sub

    Public Sub StatusMessage(msg As String, Optional icon As Image = Nothing) Implements AppHost.StatusMessage

    End Sub

    Public Function GetWindowState() As FormWindowState Implements AppHost.GetWindowState
        Return WindowState
    End Function

    Public Function GetDesktopLocation() As Point Implements AppHost.GetDesktopLocation
        Return Location
    End Function

    Public Function GetClientSize() As Size Implements AppHost.GetClientSize
        Return Size
    End Function

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        Workbench.Hook(Me)
    End Sub

    Public Event ResizeForm As AppHost.ResizeFormEventHandler Implements AppHost.ResizeForm
    Public Event CloseWorkbench As AppHost.CloseWorkbenchEventHandler Implements AppHost.CloseWorkbench

    Public Sub LogText(text As String) Implements AppHost.LogText

    End Sub

    Private Sub FormMain_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        RaiseEvent ResizeForm(Location, Size)
    End Sub

    Public Sub ShowProperties(obj As Object) Implements AppHost.ShowProperties
        ' Throw New NotImplementedException()
    End Sub

    Public Sub SetTitle(title As String) Implements AppHost.SetTitle
        Call Invoke(Sub() Me.Text = title)
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Dim files = getSourceDir.ListFiles("*.raw")

        ListBox1.Items.Clear()

        For Each file As String In files
            Call ListBox1.Items.Add(file)
        Next
    End Sub

    Public Sub SetWorkbenchVisible(visible As Boolean) Implements AppHost.SetWorkbenchVisible
        Me.Visible = visible

        If visible Then
            Me.Activate()
        Else
            Me.Hide()
        End If
    End Sub

    Private Sub FormMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        RaiseEvent CloseWorkbench(e)
    End Sub
End Class
