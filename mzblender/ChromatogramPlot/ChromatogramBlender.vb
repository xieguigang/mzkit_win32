#Region "Microsoft.VisualBasic::9ac41c18a3de5e61f72caa9dd7978355, mzkit\mzblender\ChromatogramPlot\ChromatogramBlender.vb"

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

    '   Total Lines: 43
    '    Code Lines: 38 (88.37%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (11.63%)
    '     File Size: 1.77 KB


    ' Class ChromatogramBlender
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: Rendering
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports MZKitWin32.Blender.CommonLibs
Imports Image = System.Drawing.Image

Public Class ChromatogramBlender : Inherits Blender

    ReadOnly TICList As NamedCollection(Of ChromatogramTick)()

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(TICList As IEnumerable(Of NamedCollection(Of ChromatogramTick)))
        Me.TICList = TICList.ToArray
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(name As String, ticks As IEnumerable(Of ChromatogramTick))
        Call Me.New({New NamedCollection(Of ChromatogramTick)(name, ticks)})
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
            labelFontStyle:=args.label_font.CreateCss.ToString,
            labelLayoutTicks:=-1,
            legend_split:=args.legend_block_size
        ).AsGDIImage.CTypeGdiImage
    End Function
End Class
