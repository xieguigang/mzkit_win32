#Region "Microsoft.VisualBasic::44bf0d86a0fd25b76033d333b12f0fe2, mzkit\src\mzkit\mzkit\pages\Settings\ElementProfile.vb"

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

'   Total Lines: 68
'    Code Lines: 53
' Comment Lines: 0
'   Blank Lines: 15
'     File Size: 2.79 KB


' Class ElementProfile
' 
'     Sub: Button1_Click, ComboBox1_SelectedIndexChanged, loadPresetProfile, LoadSettings, SaveSettings
'          ShowPage
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.mzkit_win32.Configuration

Namespace SettingsPage

    Public Class ElementProfile

        Public Property atom As String
        Public Property min As Double
        Public Property max As Double

        Public Shared Function LoadSettings() As Dictionary(Of String, ElementRange)
            If Globals.Settings.formula_search Is Nothing Then
                Globals.Settings.formula_search = New FormulaSearchProfile With {
                    .elements = New Dictionary(Of String, ElementRange)
                }
            End If

            Return Globals.Settings.formula_search.elements
        End Function

        Public Shared Sub SaveSettings(profiles As IEnumerable(Of ElementProfile))
            Dim elements As New Dictionary(Of String, ElementRange)

            For Each elementProfile As ElementProfile In profiles
                Dim atomName As String = elementProfile.atom

                If atomName.StringEmpty Then
                    Continue For
                End If

                elements.Add(atomName, New ElementRange With {.min = elementProfile.min, .max = elementProfile.max})
            Next

            Globals.Settings.formula_search.elements = elements
            Globals.Settings.Save()
        End Sub

        Public Shared Function loadPresetProfile(index As FormulaSearchProfiles) As SearchOption
            Select Case index
                Case FormulaSearchProfiles.Default
                    Return SearchOption.DefaultMetaboliteProfile
                Case FormulaSearchProfiles.GeneralFlavone
                    Return SearchOption.GeneralFlavone
                Case FormulaSearchProfiles.NaturalProduct
                    Return SearchOption.NaturalProduct(DNPOrWileyType.DNP, True)
                Case FormulaSearchProfiles.SmallMolecule
                    Return SearchOption.SmallMolecule(DNPOrWileyType.DNP, True)
                Case Else
                    ' returns the custom dataset
                    Return Globals.Settings.formula_search.CreateOptions
            End Select
        End Function
    End Class
End Namespace