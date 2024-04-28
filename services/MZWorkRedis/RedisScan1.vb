#Region "Microsoft.VisualBasic::e743ad260905990941025e0a1fa07c03, E:/mzkit/src/mzkit/services/MZWorkRedis//RedisScan1.vb"

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

    '   Total Lines: 37
    '    Code Lines: 23
    ' Comment Lines: 9
    '   Blank Lines: 5
    '     File Size: 1.29 KB


    ' Class RedisScan1
    ' 
    '     Properties: BPC, meta, products, rt, TIC
    ' 
    '     Function: FromData
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.TagData

Public Class RedisScan1 : Inherits MSScan
    Implements ITimeSignal, IRetentionTime

    Public Overrides Property rt As Double Implements ITimeSignal.time, IRetentionTime.rt
    Public Property TIC As Double Implements ITimeSignal.intensity
    Public Property BPC As Double

    ''' <summary>
    ''' the redis key to the ms2 object
    ''' </summary>
    ''' <returns></returns>
    Public Property products As String()

    ''' <summary>
    ''' other meta data about this MS1 scan, likes
    ''' the [x,y] coordinate data of MSI scan data.
    ''' </summary>
    ''' <returns></returns>
    Public Property meta As Dictionary(Of String, String)

    Public Shared Function FromData(data As ScanMS1, keys2 As IEnumerable(Of String)) As RedisScan1
        Return New RedisScan1 With {
            .BPC = data.BPC,
            .into = data.into,
            .meta = data.meta,
            .mz = data.mz,
            .products = keys2.ToArray,
            .rt = data.rt,
            .scan_id = data.scan_id,
            .TIC = data.TIC
        }
    End Function
End Class
