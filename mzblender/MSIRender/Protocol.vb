#Region "Microsoft.VisualBasic::05d6b5a006eeb1dfed511190b0978e90, G:/mzkit/src/mzkit/mzblender//MSIRender/Protocol.vb"

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

    '   Total Lines: 23
    '    Code Lines: 11
    ' Comment Lines: 12
    '   Blank Lines: 0
    '     File Size: 554 B


    ' Enum Protocol
    ' 
    '     Destroy, GetTrIQIntensity, MSIRender, OpenSession, SetFilters
    '     SetHEmap, SetIntensityRange, SetSampleTag, Shutdown
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Public Enum Protocol
    ''' <summary>
    ''' Open a new session and load new MSI pixel layer data
    ''' </summary>
    OpenSession
    SetFilters
    SetHEmap
    SetSampleTag
    SetIntensityRange
    GetTrIQIntensity
    ''' <summary>
    ''' Rendering a new MS-imaging layer based on new parameters
    ''' </summary>
    MSIRender
    ''' <summary>
    ''' Destroy the current MSI blender session
    ''' </summary>
    Destroy
    ''' <summary>
    ''' Shutdown the blender server
    ''' </summary>
    Shutdown
End Enum

