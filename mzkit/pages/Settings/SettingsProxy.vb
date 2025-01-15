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
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Mzkit_win32.BasicMDIForm

Namespace SettingsPage

    ''' <summary>
    ''' host object proxy for webview
    ''' </summary>
    ''' 
    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class SettingsProxy

        Public host As frmSettings

        ReadOnly appConfig As New AppConfig
        ReadOnly preset As New PresetProfile
        ReadOnly network As New MolecularNetworking
        ReadOnly plot As New PlotConfig
        ReadOnly rawfile As New RawFileViewer

        ''' <summary>
        ''' load settings json config data
        ''' </summary>
        ''' <returns></returns>
        Public Async Function loadSettings() As Task(Of String)
            Dim settings As Settings = Globals.Settings
            Dim json As JsonObject = JSONSerializer.CreateJSONElement(Of Settings)(settings)

            If settings.viewer Is Nothing Then
                settings.viewer = New RawFileViewerSettings
            End If
            If settings.formula_search Is Nothing Then
                settings.formula_search = preset.LoadSettings
            End If

            Dim json_str As String = Await Threading.Tasks.Task.Run(Function() json.BuildJsonString)
            Return json_str
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Async Function getProfile(name As String) As Task(Of String)
            Dim profile = FormulaSearchProfile.GetProfile(name)
            Dim opt = ElementProfile.loadPresetProfile(profile)

            Return Await Threading.Tasks.Task.Run(Function() opt.GetJson(enumToStr:=True))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Async Function getAllAdducts() As Task(Of String)
            Dim pos = Provider.Positives
            Dim neg = Provider.Negatives
            Dim strings_json = Await Threading.Tasks.Task.Run(
            Function()
                Return pos.JoinIterates(neg) _
                    .Select(Function(a) a.ToString) _
                    .ToArray _
                    .GetJson(enumToStr:=True)
            End Function)

            Return strings_json
        End Function

        Public Async Function GetColors(name As String) As Task(Of String)
            Dim colors As String() = Await Task(Of String()).Run(Function() PlotConfig.GetColors(name))
            Dim json As String = colors.GetJson(enumToStr:=True)
            Return json
        End Function

        ''' <summary>
        ''' save all settings
        ''' </summary>
        Public Async Sub Save(value As String)
            Dim json As Settings = Await Threading.Tasks.Task.Run(Function() value.LoadJSON(Of Settings))
            Dim settings = Globals.Settings

            Call Workbench.LogText($"get configuration value from webview UI:")
            Call Workbench.LogText(value)

            If settings.ui Is Nothing Then
                settings.ui = New UISettings
            End If
            If settings.viewer Is Nothing Then
                settings.viewer = New RawFileViewerSettings
            End If

            Call appConfig.SaveSettings(json)
            Call preset.SaveSettings(json)

            settings.viewer.fill = json.viewer.fill
            settings.viewer.colorSet = json.viewer.colorSet
            settings.viewer.XIC_da = json.viewer.XIC_da

            Call settings.Save()
            Call Workbench.SuccessMessage("New settings value applied and saved!")
        End Sub

        Public Async Sub SaveAdducts(pos_str As String, neg_str As String)
            Dim pos As String() = pos_str.LoadJSON(Of String())(throwEx:=False)
            Dim neg As String() = neg_str.LoadJSON(Of String())(throwEx:=False)
            Dim settings = Globals.Settings

            If pos.IsNullOrEmpty Then
                Call Workbench.Warning("no positive adducts configs data!")
            End If
            If neg.IsNullOrEmpty Then
                Call Workbench.Warning("no negative adducts configs data!")
            End If

            If settings.precursor_search Is Nothing Then
                settings.precursor_search = PrecursorSearchSettings.GetDefault
            End If

            settings.precursor_search.positive = pos
            settings.precursor_search.negative = neg

            Await Threading.Tasks.Task.Run(Sub() settings.Save())

            Call Workbench.SuccessMessage("New settings value for precursor adducts applied and saved!")
        End Sub

        ''' <summary>
        ''' save a specific settings data
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="Status"></param>
        Public Async Sub SetStatus(id As String, Status As String)
            Select Case Strings.Trim(id).ToLower
                Case "save_elements"
                    Await Threading.Tasks.Task.Run(
                    Sub()
                        ElementProfile.SaveSettings(Status.LoadJSON(Of ElementProfile()))
                    End Sub)
            End Select
        End Sub

        Public Async Sub close()
            Await Threading.Tasks.Task.Run(Sub() host.Invoke(Sub() host.closePage()))
        End Sub
    End Class
End Namespace