#Region "Microsoft.VisualBasic::2c0e4b525fdd13a9adc1ed26be1cae4a, mzkit\src\mzkit\mzkit\application\settings\Settings.vb"

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

'   Total Lines: 64
'    Code Lines: 46
' Comment Lines: 4
'   Blank Lines: 14
'     File Size: 2.31 KB


'     Class Settings
' 
'         Properties: biodeep, configFile, formula_search, licensed, MRMLibfile
'                     network, precursor_search, QuantifyIonLibfile, random, recentFiles
'                     ui, viewer, workspaceFile
' 
'         Function: DefaultProfile, GetConfiguration, Reset, Save
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Configuration

    Public Class Settings

        Public Property precursor_search As PrecursorSearchSettings
        Public Property formula_search As FormulaSearchProfile
        Public Property ui As UISettings
        Public Property viewer As RawFileViewerSettings
        Public Property network As NetworkArguments
        ''' <summary>
        ''' license for each version?
        ''' </summary>
        ''' <returns></returns>
        Public Property licensed As New Dictionary(Of String, Boolean)
        ''' <summary>
        ''' current version that used for
        ''' </summary>
        ''' <returns></returns>
        Public Property version As Double

        Public Property random As String
        Public Property recentFiles As String()

        Public Property workspaceFile As String
        ''' <summary>
        ''' username|password
        ''' </summary>
        ''' <returns></returns>
        Public Property biodeep As String
        Public Property msi_filters As Filters

        Public Property MRMLibfile As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "/mzkit/MRM_IonPairs.csv"
        Public Property QuantifyIonLibfile As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "/mzkit/GCMS_QuantifyIons.ionPack"
        Public Property pubchemWebCache As String = App.AppSystemTemp & "/web/pubchem/"
        Public Shared ReadOnly Property configFile As String = App.LocalData & "/settings.json"

        Public Function Reset() As Settings
            precursor_search = New PrecursorSearchSettings With {.ppm = 5, .precursor_types = {"M", "M+H", "M-H", "M+H-H2O", "M-H2O-H"}}
            formula_search = Nothing
            ui = Nothing
            viewer = Nothing
            network = Nothing
            recentFiles = {}
            workspaceFile = Nothing
            MRMLibfile = Nothing
            QuantifyIonLibfile = Nothing
            licensed = New Dictionary(Of String, Boolean)
            version = -1
            random = RandomASCIIString(8)
            biodeep = Nothing
            msi_filters = Filters.DefaultFilters

            Return Me
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function DefaultProfile() As Settings
            Return New Settings().Reset
        End Function

        Public Shared Function GetConfiguration() As Settings
            Dim config As Settings

            Try
                config = configFile.LoadJsonFile(Of Settings)
            Catch ex As Exception
                config = Nothing
            End Try

            If config Is Nothing Then
                config = DefaultProfile()
            End If
            If config.msi_filters Is Nothing Then
                config.msi_filters = Filters.DefaultFilters
            End If

            Return config
        End Function

        Public Function Save() As Boolean
            Return Me.GetJson.SaveTo(configFile)
        End Function
    End Class
End Namespace
