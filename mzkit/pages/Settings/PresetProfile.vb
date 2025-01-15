﻿#Region "Microsoft.VisualBasic::e39eae53ed5b0d56608a661e82da147f, mzkit\src\mzkit\mzkit\pages\Settings\PresetProfile.vb"

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

    '   Total Lines: 58
    '    Code Lines: 43
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 2.43 KB


    ' Class PresetProfile
    ' 
    '     Sub: LoadSettings, SaveSettings, ShowPage, TextBox1_TextChanged
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.mzkit_win32.Configuration

Namespace SettingsPage

    Public Class PresetProfile

        Public Function LoadSettings() As FormulaSearchProfile
            Dim profile = Globals.Settings.formula_search

            If profile Is Nothing Then
                Globals.Settings.formula_search = New FormulaSearchProfile With {
                    .elements = New Dictionary(Of String, ElementRange)
                }

                profile = Globals.Settings.formula_search
            End If

            If profile.smallMoleculeProfile Is Nothing Then
                profile.smallMoleculeProfile = New PresetProfileSettings With {.isCommon = True, .type = DNPOrWileyType.Wiley}
            End If
            If profile.naturalProductProfile Is Nothing Then
                profile.naturalProductProfile = New PresetProfileSettings With {.isCommon = True, .type = DNPOrWileyType.Wiley}
            End If

            Return profile
        End Function

        Public Sub SaveSettings(config As Settings)
            If Globals.Settings.formula_search Is Nothing Then
                Globals.Settings.formula_search = New FormulaSearchProfile
            End If

            Globals.Settings.formula_search.smallMoleculeProfile = config.formula_search.smallMoleculeProfile
            Globals.Settings.formula_search.naturalProductProfile = config.formula_search.naturalProductProfile
            Globals.Settings.Save()
        End Sub
    End Class
End Namespace