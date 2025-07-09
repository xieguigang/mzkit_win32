#Region "Microsoft.VisualBasic::b6bfdb5c6bb36f99325ba07d49a9459d, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\QuantifyParameters.vb"

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

'   Total Lines: 49
'    Code Lines: 39
' Comment Lines: 0
'   Blank Lines: 10
'     File Size: 1.55 KB


' Class QuantifyParameters
' 
'     Properties: angle_threshold, peakMax, peakMin, tolerance, toleranceMethod
' 
'     Function: GetMRMArguments, GetTolerance
' 
' Enum ToleranceMethods
' 
'     da, ppm
' 
'  
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Reflection
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

Public Class IonPeakFindingParameters : Inherits PeakFindingParameters

    <Category("Ion Target")>
    <Description("Set individual arguments for this ion target")>
    Public ReadOnly Property ion As String
        Get
            Return accession
        End Get
    End Property

    <Browsable(False)>
    Public Property accession As String

    Sub New()
    End Sub

    Sub New(id As String, args As PeakFindingParameters)
        accession = id
        cloneValues(args)
    End Sub

    Private Sub cloneValues(args As PeakFindingParameters)
        Static props As PropertyInfo() = GetType(PeakFindingParameters).GetReadWriteProperties

        For Each p As PropertyInfo In props
            Call p.SetValue(Me, p.GetValue(args))
        Next
    End Sub

End Class

''' <summary>
''' peak finding arguments UI interop
''' </summary>
Public Class PeakFindingParameters

    <Category("Peak Width")>
    <DisplayName("min")>
    <Description("the min peak width in rt(seconds).")>
    Public Property peakMin As Double = 5
    <Category("Peak Width")>
    <DisplayName("max")>
    <Description("the max peak width in rt(seconds).")>
    Public Property peakMax As Double = 30

    <Category("Peak Finding")>
    <DisplayName("angle threshold")>
    <Description("The threshold value of sin(alpha) angle value, value of this parameter should be in range of [0,90]")>
    Public Property angle_threshold As Double = 6

    <Category("Peak Finding")>
    <DisplayName("S/N threshold")>
    Public Property sn_threshold As Double = 0

    <Category("Peak Finding")>
    <DisplayName("baseline threshold")>
    <Description("The quantile threshold for detects of the peak baseline(noise height).")>
    Public Property baseline_threshold As Double = 0.65
    <Category("Peak Finding")>
    <DisplayName("baseline method")>
    <Description("Select the baseline measurement threshold method, default false use the quantile threshold, tweaks this parameter value to true for use the percentage threshold method.")>
    Public Property baseline_percentage As Boolean = False

    <Category("Peak Finding")>
    <DisplayName("joint peaks")>
    Public Property joint_peaks As Boolean = True

    <Category("ROI Matches")>
    <DisplayName("time window size")>
    <Description("the time window size for matches the ROI with the reference RT if this data is existed for the quantify ion.")>
    Public Property time_window_size As Double = 5

    <Category("ROI Matches")>
    <DisplayName("time shift method")>
    Public Property time_shift_method As Boolean = False

    <Category("Quantify Ion")>
    <DisplayName("error type")>
    <Description("The mass error calculation method for matches Q1/Q3 ion")>
    Public Property toleranceMethod As MassToleranceType = MassToleranceType.Da
    <Category("Quantify Ion")>
    <Description("The mass error for matches Q1/Q3 ion")>
    Public Property tolerance As Double = 0.3

    <Category("Pre-Processing")>
    <Description("Make pre-processing of the XIC chromatogram data?")>
    Public Property preprocessing As Boolean
    <Category("Pre-Processing")>
    <DisplayName("B-spline degree")>
    <Description("The degree for the b-spline interpolate processing of the XIC chromatogram data.")>
    Public Property bspline_degree As Double = 2
    <Category("Pre-Processing")>
    <DisplayName("B-spline density")>
    Public Property bspline_density As Integer = 5

    <Category("Peak Area")>
    <DisplayName("Peak area method")>
    Public Property peakAreaMethod As PeakAreaMethods = PeakAreaMethods.TriangleArea

    ''' <summary>
    ''' Create with default values
    ''' </summary>
    Sub New()
        Dim [default] = MRMArguments.GetDefaultArguments

        peakMin = [default].peakwidth.Min
        peakMax = [default].peakwidth.Max
        angle_threshold = [default].angleThreshold
        toleranceMethod = [default].tolerance.Type
        tolerance = [default].tolerance.DeltaTolerance
        sn_threshold = [default].sn_threshold
        baseline_threshold = [default].baselineQuantile
        joint_peaks = [default].strict
        preprocessing = False
        bspline_degree = [default].bspline_degree
        bspline_density = [default].bspline_density
        time_window_size = [default].timeWindowSize
        time_shift_method = [default].time_shift_method
        baseline_percentage = [default].percentage_threshold
        peakAreaMethod = [default].peakAreaMethod
    End Sub

    Public Function GetTolerance() As Tolerance
        Select Case toleranceMethod
            Case MassToleranceType.Da
                Return New DAmethod(tolerance)
            Case Else
                Return New PPMmethod(tolerance)
        End Select
    End Function

    Public Function GetMRMArguments() As MRMArguments
        Dim args As MRMArguments = MRMArguments.GetDefaultArguments

        args.peakwidth = {peakMin, peakMax}
        args.angleThreshold = angle_threshold
        args.tolerance = GetTolerance()
        args.sn_threshold = sn_threshold
        args.baselineQuantile = baseline_threshold
        args.joint_peaks = joint_peaks
        args.bspline = preprocessing
        args.bspline_degree = bspline_degree
        args.bspline_density = bspline_density
        args.time_shift_method = time_shift_method
        args.timeWindowSize = time_window_size
        args.percentage_threshold = baseline_percentage
        args.peakAreaMethod = peakAreaMethod

        Return args
    End Function

End Class
