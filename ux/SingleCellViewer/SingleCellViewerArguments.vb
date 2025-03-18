#Region "Microsoft.VisualBasic::25d9f8c16643fac1631810844db7f23d, mzkit\ux\SingleCellViewer\SingleCellViewerArguments.vb"

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

'   Total Lines: 20
'    Code Lines: 15 (75.00%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 5 (25.00%)
'     File Size: 738 B


' Class SingleCellViewerArguments
' 
'     Properties: background, cells, clusters, colorSet, pointSize
' 
'     Constructor: (+1 Overloads) Sub New
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq

Public Class SingleCellViewerArguments

    Public ReadOnly Property cells As Integer
    Public ReadOnly Property clusters As Integer
    Public Property colorSet As CategoryPalettes = CategoryPalettes.Paper
    Public Property pointSize As Single = 3
    Public Property background As Color = Color.White

    <Description("Rendering in molecular expression heatmap mode?")>
    Public ReadOnly Property heatmap As Boolean

    Sub New(cells As IEnumerable(Of UMAPPoint))
        Dim groups = cells.GroupBy(Function(c) c.class).Select(Function(a) a.ToArray).ToArray

        Me.clusters = groups.Length
        Me.cells = groups.IteratesALL.Count
    End Sub

    Public Sub SetHeatmapMode(mode As Boolean)
        _heatmap = mode
    End Sub

End Class
