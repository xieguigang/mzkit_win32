#Region "Microsoft.VisualBasic::4086d4f6d66f6f6efb414289897dad18, G:/mzkit/src/mzkit/mzblender//ChromatogramPlot/XIC3DBlender.vb"

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

    '   Total Lines: 34
    '    Code Lines: 29
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 1.40 KB


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
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Task

Public Class XIC3DBlender : Inherits Blender


    ReadOnly TICList As NamedCollection(Of ChromatogramTick)()

    Sub New(TICList As IEnumerable(Of NamedCollection(Of ChromatogramTick)))
        Me.TICList = TICList.ToArray
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Return New ScanVisual3D(scans:=TICList, angle:=60, fillCurve:=True, fillAlpha:=120, drawParallelAxis:=True, theme:=New Theme With {
            .colorSet = args.GetColorSetName,
            .gridFill = args.gridFill.ToHtmlColor,
            .padding = args.GetPadding.ToString,
            .drawLegend = args.show_legend,
            .drawLabels = args.show_tag,
            .drawGrid = args.show_grid,
            .tagCSS = New CSSFont(args.label_font).ToString
        }) With {
            .xlabel = args.xlabel,
            .ylabel = args.ylabel,
            .main = args.title
        }.Plot($"{args.width},{args.height}", ppi:=100) _
            .AsGDIImage
    End Function
End Class
