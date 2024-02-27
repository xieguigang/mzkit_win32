#Region "Microsoft.VisualBasic::4c12e10a0b1ae0213ab117be7f104961, mzkit\src\mzkit\mzkit\pages\Settings\ISaveSettings.vb"

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

'   Total Lines: 11
'    Code Lines: 7
' Comment Lines: 0
'   Blank Lines: 4
'     File Size: 158.00 B


' Interface ISaveSettings
' 
'     Sub: LoadSettings, SaveSettings
' 
' Interface IPageSettings
' 
'     Sub: ShowPage
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.mzkit_win32.Configuration
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Interface ISaveSettings

    Sub LoadSettings()
    Sub SaveSettings()

End Interface

Public Interface IPageSettings
    Sub ShowPage()

End Interface

''' <summary>
''' host object proxy for webview
''' </summary>
''' 
<ClassInterface(ClassInterfaceType.AutoDual)>
<ComVisible(True)>
Public Class SettingsProxy

    Public Function loadSettings() As String
        Dim settings = Globals.Settings
        Dim json_str As String = settings.GetJson

        Return json_str
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function getProfile(name As String) As String
        Return ElementProfile.loadPresetProfile(FormulaSearchProfile.GetProfile(name)).GetJson
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function getAllAdducts() As String
        Return Provider.Positives _
            .JoinIterates(Provider.Negatives) _
            .Select(Function(a) a.ToString) _
            .ToArray _
            .GetJson
    End Function

    ''' <summary>
    ''' save all settings
    ''' </summary>
    Public Sub Save(value As String)

    End Sub

    ''' <summary>
    ''' save a specific settings data
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="Status"></param>
    Public Sub SetStatus(id As String, Status As String)
        Select Case Strings.Trim(id).ToLower
            Case "save_elements"
                Call ElementProfile.SaveSettings(Status.LoadJSON(Of ElementProfile()))
        End Select
    End Sub
End Class