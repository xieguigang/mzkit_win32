#Region "Microsoft.VisualBasic::6586b804162e2aeebb3a76a921391c38, mzkit\src\mzkit\mzkit\application\settings\PrecursorSearchSettings.vb"

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

    '   Total Lines: 55
    '    Code Lines: 39
    ' Comment Lines: 0
    '   Blank Lines: 16
    '     File Size: 1.63 KB


    '     Class PrecursorSearchSettings
    ' 
    '         Properties: ppm, precursor_types
    ' 
    '     Enum FormulaSearchProfiles
    ' 
    '         GeneralFlavone
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class FormulaSearchProfile
    ' 
    '         Properties: elements, naturalProductProfile, smallMoleculeProfile
    ' 
    '         Function: CreateOptions
    ' 
    '     Class PresetProfileSettings
    ' 
    '         Properties: isCommon, type
    ' 
    '     Class ElementRange
    ' 
    '         Properties: max, min
    ' 
    '         Function: AsDoubleRange
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq

Namespace Configuration

    Public Class PrecursorSearchSettings

        Public Property ppm As Double = 20
        Public Property precursor_types As String()

        Public Shared Function GetDefault() As PrecursorSearchSettings
            Return New PrecursorSearchSettings With {
                .ppm = 5,
                .precursor_types = {"M", "M+H", "M-H", "M+H-H2O", "M-H2O-H"}
            }
        End Function

    End Class

    Public Enum FormulaSearchProfiles
        <Description("Custom_Profile")> Custom
        <Description("Default_Profile")> [Default]
        <Description("Small_Molecule")> SmallMolecule
        <Description("Natural_Product")> NaturalProduct
        <Description("General_Falvone")> GeneralFlavone
    End Enum

    Public Class FormulaSearchProfile

        ''' <summary>
        ''' the element profiles for custom set
        ''' </summary>
        ''' <returns></returns>
        Public Property elements As Dictionary(Of String, ElementRange)
        Public Property smallMoleculeProfile As PresetProfileSettings
        Public Property naturalProductProfile As PresetProfileSettings

        Private Shared ReadOnly profileNames As Dictionary(Of String, FormulaSearchProfiles)

        Shared Sub New()
            profileNames = Enums(Of FormulaSearchProfiles) _
                .Select(Iterator Function(e) As IEnumerable(Of (String, FormulaSearchProfiles))
                            Yield (e.ToString, e)
                            Yield (e.Description, e)
                        End Function) _
                .IteratesALL _
                .ToDictionary(Function(e) e.Item1.ToLower,
                              Function(e)
                                  Return e.Item2
                              End Function)
        End Sub

        Public Shared Function GetProfile(key As String) As FormulaSearchProfiles
            Return profileNames.TryGetValue(Strings.Trim(key).ToLower, [default]:=FormulaSearchProfiles.Default)
        End Function

        ''' <summary>
        ''' load the custom set for the formula search
        ''' </summary>
        ''' <returns></returns>
        Public Function CreateOptions() As SearchOption
            Dim opts As New SearchOption(-99999, 99999, 5)

            For Each element In elements
                Call opts.AddElement(element.Key, element.Value.min, element.Value.max)
            Next

            Return opts
        End Function
    End Class

    Public Class PresetProfileSettings

        Public Property type As DNPOrWileyType
        Public Property isCommon As Boolean

    End Class

    ''' <summary>
    ''' [min, max]
    ''' </summary>
    Public Class ElementRange

        Public Property min As Integer
        Public Property max As Integer

        Public Function AsDoubleRange() As DoubleRange
            Return New DoubleRange(min, max)
        End Function
    End Class
End Namespace
