#Region "Microsoft.VisualBasic::e1069e91c49bc8f69e87080f46898cee, mzkit\src\mzkit\mzkit\pages\Settings\AppConfig.vb"

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

    '   Total Lines: 104
    '    Code Lines: 36
    ' Comment Lines: 43
    '   Blank Lines: 25
    '     File Size: 4.36 KB


    ' Class AppConfig
    ' 
    '     Sub: AppConfig_Load, CheckBox1_CheckedChanged, CheckBox2_CheckedChanged, ComboBox1_SelectedIndexChanged, LoadSettings
    '          SaveSettings, ShowPage
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.mzkit_win32.Configuration
Imports BioNovoGene.mzkit_win32.My

Namespace SettingsPage

    Public Class AppConfig

        Dim oldLanguageConfig As Languages

        Public Function LoadSettings() As UISettings
            If Globals.Settings.ui Is Nothing Then
                Globals.Settings.ui = New UISettings With {
                    .rememberWindowsLocation = True,
                    .rememberLayouts = True,
                    .language = Languages.System
                }
            End If

            Return Globals.Settings.ui
        End Function

        Public Sub SaveSettings(config As Settings)
            Globals.Settings.ui.language = config.ui.language
            Globals.Settings.ui.rememberLayouts = config.ui.rememberLayouts
            Globals.Settings.ui.rememberWindowsLocation = config.ui.rememberWindowsLocation

            Globals.Settings.Save()

            If Globals.Settings.ui.language <> CInt(oldLanguageConfig) Then
                Call MessageBox.Show(
                    MyApplication.getLanguageString("language", Globals.Settings.ui.language),
                    MyApplication.getLanguageString("msgbox_title", Globals.Settings.ui.language),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                )
            End If
        End Sub
    End Class
End Namespace