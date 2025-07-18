﻿#Region "Microsoft.VisualBasic::c0c9a930e24852d71d6bce9becae0c84, mzkit\src\mzkit\Task\Properties\MRMROIProperty.vb"

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

'   Total Lines: 42
'    Code Lines: 36
' Comment Lines: 0
'   Blank Lines: 6
'     File Size: 1.43 KB


' Class MRMROIProperty
' 
'     Properties: baseline, peakArea, precursor, product, rt
'                 rtmax, rtmin
' 
'     Constructor: (+1 Overloads) Sub New
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.SignalReader.ChromatogramReader
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports rawChromatogram = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram
Imports std = System.Math

Public Class MRMROIProperty

    <Category("MRM IonPair")> Public ReadOnly Property precursor As Double
    <Category("MRM IonPair")> Public ReadOnly Property product As Double
    <Category("MRM IonPair")> Public ReadOnly Property id As String
    <Category("MRM IonPair")> Public ReadOnly Property name As String

    <Category("MRM IonPair")>
    <DisplayName("reference RT")>
    Public ReadOnly Property referRT As Double

    <Category("ROI Feature")> Public ReadOnly Property rtmin As Double
    <Category("ROI Feature")> Public ReadOnly Property rtmax As Double
    <Category("ROI Feature")> Public ReadOnly Property rt As Double

    <Category("ROI Feature")>
    <DisplayName("rt(min)")>
    Public ReadOnly Property rt_minute As Double
        Get
            Return std.Round(rt / 60, 2)
        End Get
    End Property

    <Category("ROI Feature")> Public ReadOnly Property peakArea As Double
    <Category("ROI Feature")> Public ReadOnly Property baseline As Double
    <Category("ROI Feature")> Public ReadOnly Property peakHeight As Double

    Sub New(ion As IonPair, peak As PeakFeature, xic As ChromatogramTick())
        Call Me.New(xic)

        precursor = ion.precursor
        product = ion.product
        id = If(ion.accession, "NA")
        name = If(ion.name, "No-Title")
        ' nullable value must have value, default set to zero
        referRT = If(ion.rt, 0)

        If Not peak Is Nothing Then
            rtmin = peak.rtmin
            rtmax = peak.rtmax
            rt = peak.rt
            peakArea = peak.area
            baseline = peak.baseline
            peakHeight = peak.maxInto
        End If
    End Sub

    Sub New(TIC As ChromatogramTick())
        Dim ROI = TIC.Shadows _
           .PopulateROI(
               baselineQuantile:=0.65,
               angleThreshold:=5,
               peakwidth:=New Double() {8, 30},
               snThreshold:=3
           ) _
           .OrderByDescending(Function(r) r.integration) _
           .FirstOrDefault

        If Not ROI Is Nothing Then
            rtmin = ROI.time.Min
            rtmax = ROI.time.Max
            rt = ROI.rt
            peakArea = ROI.integration
            baseline = ROI.baseline
            peakHeight = ROI.maxInto
        End If
    End Sub

    Sub New(chr As rawChromatogram)
        Call Me.New(chr.Ticks)

        If chr.precursor IsNot Nothing AndAlso chr.product IsNot Nothing Then
            precursor = chr.precursor.MRMTargetMz
            product = chr.product.MRMTargetMz
        End If
    End Sub
End Class
