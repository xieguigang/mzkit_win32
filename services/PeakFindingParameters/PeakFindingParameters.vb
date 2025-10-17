
Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

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
