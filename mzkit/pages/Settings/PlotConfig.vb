#Region "Microsoft.VisualBasic::6339f84c46f6bbb021d0ff4c833098be, mzkit\src\mzkit\mzkit\pages\Settings\PlotConfig.vb"

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

'   Total Lines: 229
'    Code Lines: 162
' Comment Lines: 19
'   Blank Lines: 48
'     File Size: 8.61 KB


' Class PlotConfig
' 
'     Sub: Button1_Click, Button2_Click, clearAllColors, ComboBox1_SelectedIndexChanged, deleteColorButton_MouseDown
'          deleteColorButton_MouseEnter, deleteColorButton_MouseLeave, Label1_Click, ListBox1_DragDrop, ListBox1_DragEnter
'          ListBox1_DrawItem, ListBox1_MouseDown, ListBox1_MouseMove, ListBox1_SelectedIndexChanged, LoadSettings
'          PlotConfig_Load, SaveSettings, selectColor, ShowPage
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.mzkit_win32.Configuration
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Namespace SettingsPage

    Public Class PlotConfig

        Public Shared Sub SaveSettings(colorSet As String(), fill As Boolean)
            If Globals.Settings.viewer Is Nothing Then
                Globals.Settings.viewer = New RawFileViewerSettings
            End If

            Globals.Settings.viewer.colorSet = colorSet
            Globals.Settings.viewer.fill = fill
        End Sub

        Public Shared Function GetColors(name As String) As String()
            Dim colors As Color() = Nothing

            '        ColorBrewer Set1
            'ColorBrewer Paired1
            'ColorBrewer Accent
            'Cluster Colour
            'Material Palette
            'sciBASIC Category31

            Select Case Strings.Trim(name).ToLower
                Case "set1" : colors = Designer.GetColors("Set1:c8")
                Case "paired1" : colors = Designer.GetColors("Paired1:c8")
                Case "accent" : colors = Designer.GetColors("Accent:c8")
                Case "cluster" : colors = Designer.GetColors("Clusters")
                Case "material" : colors = Designer.GetColors("material")
                Case "scibasic" : colors = Designer.GetColors("scibasic.category31()")
                Case Else
                    colors = Designer.GetColors("Paper")
            End Select

            Return colors _
                .Select(Function(c) c.ToHtmlColor) _
                .ToArray
        End Function
    End Class
End Namespace