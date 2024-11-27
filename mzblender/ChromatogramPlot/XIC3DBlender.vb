#Region "Microsoft.VisualBasic::65abae16a39b66d9d0f535ee1a058507, mzkit\mzblender\ChromatogramPlot\XIC3DBlender.vb"

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

    '   Total Lines: 38
    '    Code Lines: 32 (84.21%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (15.79%)
    '     File Size: 1.54 KB


    ' Class XIC3DBlender
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Rendering
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Drawing.CssInterop
Imports Microsoft.VisualBasic.Imaging
Imports Task
Imports Image = System.Drawing.Image

Public Class XIC3DBlender : Inherits Blender


    ReadOnly TICList As NamedCollection(Of ChromatogramTick)()

    Sub New(TICList As IEnumerable(Of NamedCollection(Of ChromatogramTick)))
        Me.TICList = TICList.ToArray
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim render As New ScanVisual3D(scans:=TICList, angle:=60, fillCurve:=True, fillAlpha:=120, drawParallelAxis:=True, theme:=New Theme With {
            .colorSet = args.GetColorSetName,
            .gridFill = args.gridFill.ToHtmlColor,
            .padding = args.GetPadding.ToString,
            .drawLegend = args.show_legend,
            .drawLabels = args.show_tag,
            .drawGrid = args.show_grid,
            .tagCSS = args.label_font.CreateCss.ToString
        }) With {
            .xlabel = args.xlabel,
            .ylabel = args.ylabel,
            .main = args.title
        }
        Dim visual = render.Plot($"{args.width},{args.height}", ppi:=100).AsGDIImage

        Return visual.CTypeGdiImage
    End Function
End Class
