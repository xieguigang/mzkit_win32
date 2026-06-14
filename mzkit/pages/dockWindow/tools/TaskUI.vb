#Region "Microsoft.VisualBasic::8b1558e49e88c9e590e9d5dbe8973d93, mzkit\src\mzkit\mzkit\pages\dockWindow\tools\TaskUI.vb"

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

'   Total Lines: 73
'    Code Lines: 53
' Comment Lines: 3
'   Blank Lines: 17
'     File Size: 2.39 KB


' Class TaskUI
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: Finish, ProgressMessage, Running, SetTaskFinishStatus, switchToFinishStatus
'          switchToRunningStatus
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.mzkit_win32.My

Public Class TaskRun

    Public Property text As String
    Public Property imageIndex As Integer
    Public Property status As String
    Public Property progress As String
    Public Property content As String
    Public Property time As String

End Class

''' <summary>
''' just provides the UI update interface to the task worker
''' </summary>
Public Class TaskUI

    Dim window As TaskListWindow
    Dim row As TaskRun

    Dim taskTitle, taskContent As String

    Sub New(task$, content$, list As TaskListWindow)
        row = New TaskRun With {.text = task, .imageIndex = 0, .status = "Pending", .progress = "...", .content = content, .time = Now.ToString}
        window = list
        window.Add(row)

        taskTitle = task
        taskContent = content
    End Sub

    ''' <summary>
    ''' 切换为执行中的状态
    ''' </summary>
    Public Sub Running()
        window.Invoke(Sub() switchToRunningStatus())
    End Sub

    Private Sub switchToRunningStatus()
        row.status = "Running..."
    End Sub

    Private Sub switchToFinishStatus()
        row.status = "Finished"
        row.progress = ""
    End Sub

    Public Sub ProgressMessage(message As String)
        window.Invoke(Sub() row.progress = message)
    End Sub

    Public Sub Finish()
        Dim message As String = $"{taskTitle} Job Done!{vbCrLf}{taskContent}"
        Dim main As frmMain = MyApplication.host

        window.Invoke(Sub() switchToFinishStatus())
        TaskListWindow.pending -= 1

        Call main.Invoke(Sub() SetTaskFinishStatus(main))
    End Sub

    Private Sub SetTaskFinishStatus(main As frmMain)
        main.ToolStripProgressBar1.Value += 1
        WindowModules.taskWin.UpdateProgress()

        If main.ToolStripProgressBar1.Value = main.ToolStripProgressBar1.Maximum Then
            main.ToolStripStatusLabel4.Image = My.Resources._1200px_Checked_svg
            main.ToolStripStatusLabel4.Text = "Job Done!"

            main.ToolStripProgressBar1.Value = 0
            main.ToolStripProgressBar1.Maximum = 0
        End If
    End Sub
End Class
