#Region "Microsoft.VisualBasic::502b34f2b89d1b34c8520aea77193334, mzkit\mzblender\Blender.vb"

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

    '   Total Lines: 17
    '    Code Lines: 4 (23.53%)
    ' Comment Lines: 10 (58.82%)
    '    - Xml Docs: 90.00%
    ' 
    '   Blank Lines: 3 (17.65%)
    '     File Size: 463 B


    ' Class Blender
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports MZKitWin32.Blender.CommonLibs

Public MustInherit Class Blender

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="args">
    ''' the image plot arguments
    ''' </param>
    ''' <param name="target">
    ''' the size of the target control to show the rednering image result.
    ''' </param>
    ''' <returns></returns>
    Public MustOverride Function Rendering(args As PlotProperty, target As Size) As Image

End Class
