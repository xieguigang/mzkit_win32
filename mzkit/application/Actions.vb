#Region "Microsoft.VisualBasic::295ba5f2f1918f08eb52f012f54d7943, mzkit\src\mzkit\mzkit\application\Actions.vb"

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

'   Total Lines: 102
'    Code Lines: 88
' Comment Lines: 0
'   Blank Lines: 14
'     File Size: 5.16 KB


' Module Actions
' 
'     Properties: allActions
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: Register, registerKEGGEnrichment, registerMs1Search, RunAction
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Module Actions

    ReadOnly actions As New Dictionary(Of String, ActionBase)

    Public ReadOnly Property allActions As IEnumerable(Of NamedValue(Of String))
        Get
            Return actions _
                .Keys _
                .Select(Function(ref)
                            Return New NamedValue(Of String)(ref, actions(ref).Description)
                        End Function)
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Register(name As String, action As ActionBase)
        actions(name) = action
    End Sub

    Public Sub RunAction(name As String, fieldName As String, data As Array, table As DataTable)
        If actions.ContainsKey(name) Then
            Call actions(name).RunAction(fieldName, data, table)
        Else
            Call MyApplication.host.warning($"missing action '{name}'!")
        End If
    End Sub

    Sub New()
        Call Register("KEGG Enrichment", New KEGGEnrichmentAction)
        Call Register("Formula Query", New FormulaQueryAction)
        Call Register("Peak Finding", New PeakFindingAction)
        Call Register("Peak List Annotation", New PeakAnnotationAction)
        Call Register("KEGG Stats", New KEGGStatsAction)
    End Sub
End Module
