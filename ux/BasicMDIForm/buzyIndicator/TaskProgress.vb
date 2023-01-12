﻿#Region "Microsoft.VisualBasic::f352aacddb5d9ad2191c6c8ef7ce4a75, mzkit\src\mzkit\mzkit\forms\Task\frmTaskProgress.vb"

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

'   Total Lines: 164
'    Code Lines: 120
' Comment Lines: 9
'   Blank Lines: 35
'     File Size: 5.21 KB


' Class frmTaskProgress
' 
'     Function: (+2 Overloads) LoadData
' 
'     Sub: CloseWindow, frmImportTaskProgress_Paint, frmTaskProgress_Closed, frmTaskProgress_KeyDown, frmTaskProgress_Load
'          RunAction, (+2 Overloads) SetProgress, SetProgressMode, ShowProgressDetails, ShowProgressTitle
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Threading
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Public Interface ITaskProgress

    Sub SetProgressMode()
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="p">[0,100]</param>
    Sub SetProgress(p As Integer)
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="p">[0,100]</param>
    ''' <param name="msg"></param>
    Sub SetProgress(p As Integer, msg As String)
    Sub SetTitle(title As String)
    Sub SetInfo(message As String)
    Sub TaskFinish()

End Interface

Public Class TaskProgress : Implements ITaskProgress

    Dim dialogClosed As Boolean = False

    Public TaskCancel As Action

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        AutoScaleMode = AutoScaleMode.Dpi
    End Sub

    Public Sub SetProgressMode() Implements ITaskProgress.SetProgressMode
        TaskbarStatus.SetProgress(0)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="p">[0,100]</param>
    Public Sub SetProgress(p As Integer) Implements ITaskProgress.SetProgress
        Call Invoke(
            Sub()
                Dim ProgressValue = If(p < 100, p, 100)
                WebView21.CoreWebView2.ExecuteScriptAsync($"document.querySelector('#progress_val').innerHTML = {ProgressValue};")
                TaskbarStatus.SetProgress(ProgressValue)
            End Sub)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="p">[0,100]</param>
    ''' <param name="message"></param>
    Public Sub SetProgress(p As Integer, message As String) Implements ITaskProgress.SetProgress
        Call Invoke(
            Sub()
                Dim ProgressValue = If(p < 100, p, 100)
                Dim json As String = message.GetJson

                WebView21.CoreWebView2.ExecuteScriptAsync($"
document.querySelector('#progress_val').innerHTML = {ProgressValue};
document.querySelector('#info').innerHTML = JSON.parse('{message}');
")
                TaskbarStatus.SetProgress(ProgressValue)
            End Sub)
    End Sub

    Private Sub frmImportTaskProgress_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        e.Graphics.DrawRectangle(New Pen(Color.Black, 1), New Rectangle(0, 0, Width - 1, Height - 1))
    End Sub

    Private Sub WebView21_Paint(sender As Object, e As PaintEventArgs) Handles WebView21.Paint
        e.Graphics.DrawRectangle(New Pen(Color.Black, 1), New Rectangle(0, 0, Width - 1, Height - 1))
    End Sub

    Private Sub ShowProgressTitle(title As String) Implements ITaskProgress.SetTitle
        If Not dialogClosed Then
            Invoke(Sub()
                       Dim title_str As String

                       If TaskCancel Is Nothing Then
                           title_str = title
                       Else
                           title_str = $"{title} [Press ESC for cancel task]"
                       End If

                       WebView21.CoreWebView2.ExecuteScriptAsync($"document.querySelector('#title').innerHTML = JSON.parse('{title_str.GetJson}');")
                   End Sub)
        End If
    End Sub

    Dim titleDirect As String
    Dim messageDirect As String

    Private Sub ShowProgressDetails(message As String) Implements ITaskProgress.SetInfo
        If Not dialogClosed Then
            Invoke(Sub()
                       WebView21.CoreWebView2.ExecuteScriptAsync($"document.querySelector('#info').innerHTML = JSON.parse('{message.GetJson}');")
                   End Sub)
        End If

        Call Application.DoEvents()
        Call Workbench.StatusMessage(message)
    End Sub

    Private Sub frmTaskProgress_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown, WebView21.KeyDown
        If TaskCancel Is Nothing Then
            Return
        End If

        SyncLock WebView21
            If e.KeyCode = Keys.Escape Then
                WebView21.CoreWebView2.ExecuteScriptAsync($"document.querySelector('#title').innerHTML = 'Task Cancel...';")
                dialogClosed = True
                TaskCancel()
            End If
        End SyncLock
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        Call WebView21.CoreWebView2.NavigateToString(My.Resources.progress_bar)
        Call WebKit.DeveloperOptions(WebView21, enable:=False)
    End Sub

    Private Sub frmTaskProgress_Load(sender As Object, e As EventArgs) Handles Me.Load
        DoubleBuffered = True
        WebKit.Init(WebView21)
        TaskbarStatus.SetLoopStatus()
    End Sub

    Dim webkitLoaded As Boolean = False

    Public Shared Function LoadData(Of T)(streamLoad As Func(Of Action(Of String), T),
                                          Optional title$ = "Loading data...",
                                          Optional info$ = "Open a large raw data file...",
                                          Optional ByRef taskAssign As Thread = Nothing,
                                          Optional canbeCancel As Boolean = False) As T
        Return LoadData(Function(task)
                            Return streamLoad(task.Echo)
                        End Function, title, info, taskAssign, canbeCancel)
    End Function

    Public Shared Function LoadData(Of T)(streamLoad As Func(Of ITaskProgress, T),
                                          Optional title$ = "Loading data...",
                                          Optional info$ = "Open a large raw data file...",
                                          Optional ByRef taskAssign As Thread = Nothing,
                                          Optional canbeCancel As Boolean = False) As T
        Dim tmp As T
        Dim progress As New TaskProgress
        Dim mask As MaskForm = MaskForm.CreateMask(Workbench.AppHost)
        Dim task As ThreadStart =
            Sub()
                Do While Not progress.webkitLoaded
                    Call Thread.Sleep(30)
                Loop

                Call progress.ShowProgressTitle(title)
                Call progress.ShowProgressDetails(info)

                Try
                    tmp = Nothing
                    tmp = streamLoad(progress)
                Catch ex As Exception
                    Call App.LogException(ex)
                    Call progress.ShowProgressTitle("Task Error!")
                    Call progress.ShowProgressDetails(ex.Message)
                    Call Thread.Sleep(3 * 1000)
                Finally
                    Call progress.CloseWindow()
                End Try
            End Sub

        If progress.ParentForm Is Nothing Then
            progress.StartPosition = FormStartPosition.Manual
            progress.Location = Workbench.CenterToMain(progress)
        End If

        taskAssign = New Thread(task)
        taskAssign.Start()

        If canbeCancel Then
            Dim handle As Thread = taskAssign

            progress.TaskCancel =
                Sub()
                    Try
                        Call handle.Abort()
                    Catch ex As Exception
                        Call Workbench.Warning($"[{title}] task has been cancel.")
                    End Try
                End Sub
        End If

        Call mask.ShowDialogForm(progress)

        Return tmp
    End Function

    Public Shared Sub RunAction(run As Action(Of ITaskProgress),
                                Optional title$ = "Loading data...",
                                Optional info$ = "Open a large raw data file...",
                                Optional cancel As Action = Nothing,
                                Optional host As Control = Nothing)

        Dim progress As New TaskProgress With {
            .TaskCancel = cancel
        }
        Dim mask As MaskForm = MaskForm.CreateMask(Workbench.AppHost)

        Call progress.SetProgressMode()
        Call New Thread(
            Sub()
                Do While Not progress.webkitLoaded
                    Call Thread.Sleep(30)
                Loop

                Call progress.ShowProgressTitle(title)
                Call progress.ShowProgressDetails(info)

                Try
                    If host Is Nothing Then
                        Call run(progress)
                    Else
                        Call host.Invoke(Sub() Call run(progress))
                    End If
                Catch ex As Exception
                    Call App.LogException(ex)
                    Call progress.ShowProgressTitle("Task Error!")
                    Call progress.ShowProgressDetails(ex.Message)
                    Call Thread.Sleep(3 * 1000)
                Finally
                    Call progress.CloseWindow()
                End Try
            End Sub).Start()

        Call mask.ShowDialogForm(progress)
    End Sub

    Public Sub CloseWindow() Implements ITaskProgress.TaskFinish
        Call Me.Invoke(Sub() Call Me.Close())
    End Sub

    Private Sub TaskProgress_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        TaskbarStatus.Stop()
    End Sub

    Private Sub WebView21_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles WebView21.NavigationCompleted
        webkitLoaded = True
    End Sub


End Class
