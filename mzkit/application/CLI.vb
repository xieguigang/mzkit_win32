﻿#Region "Microsoft.VisualBasic::6f85c3f416cda31f903619c48074dabf, mzkit\src\mzkit\mzkit\application\CLI.vb"

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

'   Total Lines: 29
'    Code Lines: 24
' Comment Lines: 0
'   Blank Lines: 5
'     File Size: 958.00 B


' Module CLI
' 
'     Function: openDevTools, openRawFile
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.CommandLine
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.MSImagingViewerV2.DeepZoomBuilder
Imports WeifenLuo.WinFormsUI.Docking

Module CLI

    ''' <summary>
    ''' --debug-plugin --plugin xxxxx
    ''' </summary>
    ''' <param name="nameOrGuid"></param>
    ''' <returns></returns>
    Public Function debugPluginPage(nameOrGuid As String) As Integer
        Dim registry As PluginMgr = PluginMgr.Load
        Dim page As Plugin = registry.Query(nameOrGuid)

        If Not page Is Nothing Then
            MyApplication.afterLoad =
                Sub()
                    Call page.Init(AddressOf Workbench.StatusMessage)
                    Call page.Exec()
                End Sub
        End If

        Return 0
    End Function

    ''' <summary>
    ''' xxxx.raw
    ''' </summary>
    ''' <param name="filepath"></param>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Public Function openRawFile(filepath As String, args As CommandLine) As Integer
        MyApplication.afterLoad =
            Sub()
                Select Case filepath.ExtensionSuffix.ToLower
                    Case "mzpack", "raw", "msp", "mgf"
                        MyApplication.host.OpenFile(filepath, showDocument:=True)
                        WindowModules.panelMain.Show(MyApplication.host.m_dockPanel)
                        WindowModules.panelMain.DockState = DockState.Document
                    Case "mzwork"
                        Globals.loadWorkspace(mzwork:=filepath, fromStartup:=True)

                End Select
            End Sub

        Return 0
    End Function

    ''' <summary>
    ''' --devtools
    ''' </summary>
    ''' <returns></returns>
    Public Function openDevTools() As Integer
        Call RibbonEvents.openCmd()
        Call App.Exit()

        Return 0
    End Function

    ''' <summary>
    ''' --deep_zoom --image xxxxx.tiff
    ''' </summary>
    ''' <param name="img"></param>
    ''' <returns></returns>
    Public Function createDeepzoomImage(img As String) As Integer
        Dim tool As New DeepZoomCreator()
        Dim dzi As String = img.ChangeSuffix("dzi")

        Call tool.CreateSingleComposition(img, dzi, ImageType.Jpeg)
        Call App.Exit()

        Return 0
    End Function
End Module
