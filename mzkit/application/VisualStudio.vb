﻿#Region "Microsoft.VisualBasic::d121be5fe83107dc141341d5fb1c2186, mzkit\src\mzkit\mzkit\application\VisualStudio.vb"

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

'   Total Lines: 92
'    Code Lines: 68
' Comment Lines: 6
'   Blank Lines: 18
'     File Size: 3.07 KB


' Class VisualStudio
' 
'     Properties: DockPanel
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: ShowDocument
' 
'     Sub: Dock, ShowProperties, ShowPropertyWindow, ShowRTerm, ShowSingleDocument
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.mzkit_win32.My
Imports Galaxy.Workbench
Imports Galaxy.Workbench.DockDocument
Imports Galaxy.Workbench.DockDocument.Presets
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualStudio.WinForms.Docking
Imports Mzkit_win32.BasicMDIForm

Public Class VisualStudio

    Public Shared ReadOnly Property DockPanel As DockPanel
        Get
            Return MyApplication.host.m_dockPanel
        End Get
    End Property

    Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub Dock(win As ToolWindow, prefer As DockState)
        Call CommonRuntime.Dock(win, prefer)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub ShowPropertyWindow()
        Call Dock(Workbench.propertyWin, DockState.DockRight)
    End Sub

    ''' <summary>
    ''' Show object properties
    ''' </summary>
    ''' <param name="item"></param>
    ''' <remarks>
    ''' thread safe method
    ''' </remarks>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub ShowProperties(item As Object)
        Dim form As PropertyWindow = Workbench.propertyWin
        Call form.Invoke(Sub() form.SetObject(item))
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="showExplorer">
    ''' do specific callback from this parameter delegate if the pointer value is nothing nothing
    ''' </param>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function ShowSingleDocument(Of T As {New, DockContent})(Optional showExplorer As Action = Nothing) As T
        Return CommonRuntime.ShowSingleDocument(Of T)(showExplorer)
    End Function

    ''' <summary>
    ''' create a new document tab page
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="title">
    ''' set the tab text of the dock page
    ''' </param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function ShowDocument(Of T As {New, DocumentWindow})(Optional status As DockState = DockState.Document,
                                                                       Optional title As String = Nothing) As T
        Return CommonRuntime.ShowDocument(Of T)(status, title)
    End Function

    Public Shared Sub ShowDocument(doc As DocumentWindow,
                                   Optional status As DockState = DockState.Document,
                                   Optional title As String = Nothing)

        doc.Show(Workbench.AppHost.GetDockPanel)
        doc.DockState = status

        Call Workbench.LogText($"open new document page: {If(title, "<No Name>")}")

        If Not title.StringEmpty Then
            doc.TabText = title
            doc.Text = title
        End If
    End Sub

    Public Shared Sub ShowRTerm()
        WindowModules.RtermPage.Show(MyApplication.host.m_dockPanel)
        WindowModules.RtermPage.DockState = DockState.Document

        MyApplication.host.Text = $"BioNovoGene Mzkit [{WindowModules.RtermPage.Text}]"
    End Sub

    ''' <summary>
    ''' init
    ''' </summary>
    Public Shared Sub InstallInternalRPackages()
        Dim script As String = $"{App.HOME}\Rstudio\packages\install_locals.cmd"

        If Not script.FileExists Then
            Return
        End If

        Dim task As New ProcessStartInfo With {
            .Arguments = $"/c CALL {script.GetFullPath.CLIPath}",
            .CreateNoWindow = False,
            .FileName = Environment.SystemDirectory & "\cmd.exe",
            .UseShellExecute = False,
            .WindowStyle = ProcessWindowStyle.Normal,
            .WorkingDirectory = script.ParentPath
        }

        Call Process.Start(task)

        Globals.Settings.version = Globals.BuildTime
        Globals.Settings.Save()
    End Sub
End Class
