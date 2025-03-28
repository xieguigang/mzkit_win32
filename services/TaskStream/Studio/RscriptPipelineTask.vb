﻿#Region "Microsoft.VisualBasic::a003ea46c51cfe72935d801deb579d96, mzkit\src\mzkit\Task\Studio\RscriptPipelineTask.vb"

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

'   Total Lines: 25
'    Code Lines: 15
' Comment Lines: 2
'   Blank Lines: 8
'     File Size: 709.00 B


' Class RscriptPipelineTask
' 
'     Function: GetRScript
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports TaskStream.CLI

<Assembly: InternalsVisibleTo("mzkit_win32")>
<Assembly: InternalsVisibleTo("ServiceHub")>

Public Class RscriptPipelineTask

    Friend Shared ReadOnly Rscript As Rscript = Rscript.FromEnvironment($"{App.HOME}/Rstudio/bin")

    Public Shared ReadOnly Property Host As String
        Get
            Return Rscript.Path
        End Get
    End Property

    ''' <summary>
    ''' App.HOME/Rstudio/bin
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property Root As String
        Get
            Return Host.ParentPath
        End Get
    End Property

    ''' <summary>
    ''' find script in Rstudio pipeline folder
    ''' </summary>
    ''' <param name="filename">should be a relative path to the rscript pipeline folder</param>
    ''' <returns>returns the fullname(file path) of the target script file</returns>
    Public Shared Function GetRScript(filename As String) As String
        Dim filepath As String = $"{App.HOME}/Rstudio/pipeline/{filename}"

        ' product version
        If filepath.FileLength > 10 Then
            Return filepath.GetFullPath
        End If

        ' development version
        filepath = $"{App.HOME}/../../src/mzkit/rstudio/pipeline/{filename}".GetFullPath

        Return filepath
    End Function

End Class
