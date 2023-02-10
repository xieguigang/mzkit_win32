#Region "Microsoft.VisualBasic::9abc9ac48d547195ce6cdb0c3c786c52, mzkit\src\mzkit\mzblender\ChromatogramPlot\ChromatogramBlender.vb"

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
    '    Code Lines: 30
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 1.32 KB


    ' Class ChromatogramBlender
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Rendering
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Task

Public Class ChromatogramBlender : Inherits Blender

    ReadOnly TICList As NamedCollection(Of ChromatogramTick)()

    Sub New(TICList As NamedCollection(Of ChromatogramTick)())
        Me.TICList = TICList
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Return ChromatogramPlot.TICplot(
            ionData:=TICList.ToArray,
            colorsSchema:=args.GetColorSetName,
            fillCurve:=args.fill_curve,
            size:=$"{args.width},{args.height}",
            margin:=args.GetPadding.ToString,
            gridFill:=args.gridFill.ToHtmlColor,
            bg:=args.background.ToHtmlColor,
            showGrid:=args.show_grid,
            showLegends:=args.show_legend,
            showLabels:=args.show_tag,
            xlabel:=args.xlabel,
            ylabel:=args.ylabel,
            labelFontStyle:=New CSSFont(args.label_font).ToString,
            labelLayoutTicks:=-1
        ).AsGDIImage
    End Function
End Class
