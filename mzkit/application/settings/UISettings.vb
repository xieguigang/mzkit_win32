#Region "Microsoft.VisualBasic::bf2f6b20dd349bc6e91a608b78386b31, mzkit\src\mzkit\mzkit\application\settings\UISettings.vb"

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

'   Total Lines: 52
'    Code Lines: 26
' Comment Lines: 14
'   Blank Lines: 12
'     File Size: 2.02 KB


'     Class UISettings
' 
'         Properties: featureListDock, fileExplorerDock, height, language, OutputDock
'                     propertyWindowDock, rememberLayouts, rememberWindowsLocation, taskListDock, width
'                     window, x, y
' 
'         Function: getLocation, getSize
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualStudio.WinForms.Docking

Namespace Configuration

    Public Class UISettings

        Public Property rememberLayouts As Boolean = True

        Public Property fileExplorerDock As DockState = DockState.DockLeft
        Public Property featureListDock As DockState = DockState.DockLeftAutoHide
        Public Property OutputDock As DockState = DockState.DockBottomAutoHide
        Public Property propertyWindowDock As DockState = DockState.DockRightAutoHide
        Public Property taskListDock As DockState = DockState.DockBottomAutoHide

    End Class
End Namespace
