#Region "Microsoft.VisualBasic::1411624225756b5b923737144eeb2c06, mzkit\src\mzkit\Task\Properties\PixelProperty.vb"

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

'   Total Lines: 65
'    Code Lines: 53
' Comment Lines: 0
'   Blank Lines: 12
'     File Size: 2.72 KB


' Class PixelProperty
' 
'     Properties: AverageIons, Gini, MaxIntensity, MinIntensity, NumOfIons
'                 Q1, Q1Count, Q2, Q2Count, Q3
'                 Q3Count, ScanId, ShannonEntropy, TopIonMz, TotalIon
'                 X, Y
' 
'     Constructor: (+1 Overloads) Sub New
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.SingleCells
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Math.Distributions.BinBox
Imports Microsoft.VisualBasic.Math.Information
Imports Microsoft.VisualBasic.Math.Quantile
Imports std = System.Math

''' <summary>
''' common used spot data model for ms-imaging/singlecells rawdata
''' </summary>
Public Class PixelProperty

    <Description("the base peak ion m/z of current pixel spot mass spectrum, which its ion intensity value is the max one in current spectrum")>
    <Category("Mass Spectrum")>
    Public ReadOnly Property TopIonMz As Double
    <Description("the max intensity of current pixel spot mass spectrum")>
    <Category("Mass Spectrum")>
    Public ReadOnly Property MaxIntensity As Double
    <Description("the min intensity of current pixel spot mass spectrum")>
    <Category("Mass Spectrum")>
    Public ReadOnly Property MinIntensity As Double
    <Description("the number of the ion peaks that detected in current pixel spot mass spectrum")>
    <Category("Mass Spectrum")>
    Public ReadOnly Property NumOfIons As Integer
    <Category("Intensity")> Public ReadOnly Property Q1 As Double
    <Category("Intensity")> Public ReadOnly Property Q2 As Double
    <Category("Intensity")> Public ReadOnly Property Q3 As Double
    <Category("Intensity")> Public ReadOnly Property Q1Count As Integer
    <Category("Intensity")> Public ReadOnly Property Q2Count As Integer
    <Category("Intensity")> Public ReadOnly Property Q3Count As Integer

    <Description("the max intensity value of current pixel MS1 scan data.")>
    <Category("Mass Spectrum")>
    Public ReadOnly Property AverageIons As Double
    <Description("total ions of current pixel MS1 scan data.")>
    <Category("Mass Spectrum")>
    Public ReadOnly Property TotalIon As Double

    <Description("the pixel coordinate X of current selected pixel spot.")>
    <Category("Pixel")> Public ReadOnly Property X As Double
    <Description("the pixel coordinate Y of current selected pixel spot.")>
    <Category("Pixel")> Public ReadOnly Property Y As Double
    <Description("the pixel coordinate Z of current selected pixel spot.")>
    <Category("Pixel")> Public ReadOnly Property Z As Double

    <Description("the MS1 scan id of current selected pixel spot, which generated from the MS scan instrument experiment.")>
    <Category("Pixel")> Public ReadOnly Property ScanId As String

    <Description("The shannon entropy value of current MS1 spectrum data.")>
    <Category("Information Theory")>
    Public ReadOnly Property ShannonEntropy As Double
    <Description("The gini value of current MS1 spectrum data.")>
    <Category("Information Theory")>
    Public ReadOnly Property Gini As Double
    Public ReadOnly Property SplashId As String

    ''' <summary>
    ''' show data for single cells
    ''' </summary>
    ''' <param name="cell"></param>
    Sub New(cell As ScanMS1)
        Dim cell_data = cell.LoadScanMeta

        X = cell_data.x
        Y = cell_data.y
        Z = cell_data.z
        ScanId = cell.scan_id

        Call loadData(cell.GetPeaks.ToArray)
    End Sub

    Sub New(pixel As PixelScan)
        X = pixel.X
        Y = pixel.Y
        ScanId = pixel.scanId

        Call loadData(pixel.GetMs)
    End Sub

    Private Sub loadData(ms As ms2())
        Dim into As Double() = ms.Select(Function(mz) mz.intensity).ToArray
        Dim spectrum As New LibraryMatrix With {.ms2 = ms}

        _SplashId = Splash.MSSplash.CalcSplashID(spectrum)

        If into.Length = 0 Then
        Else
            _NumOfIons = ms.Length
            _TopIonMz = std.Round(ms.OrderByDescending(Function(i) i.intensity).First.mz, 4)
            _MaxIntensity = std.Round(into.Max)
            _MinIntensity = std.Round(into.Min)
            _TotalIon = std.Round(into.Sum)
            _AverageIons = std.Round(into.Average)

            Dim quartile = into.Quartile

            _Q1 = std.Round(quartile.Q1)
            _Q2 = std.Round(quartile.Q2)
            _Q3 = std.Round(quartile.Q3)
            _Q1Count = into.Where(Function(i) i <= quartile.Q1).Count
            _Q2Count = into.Where(Function(i) i <= quartile.Q2).Count
            _Q3Count = into.Where(Function(i) i <= quartile.Q3).Count

            Dim bin = CutBins.FixedWidthBins(into, 10, Function(x) x).ToArray
            Dim probs As Double() = bin.Select(Function(n) n.Count / into.Length).ToArray

            _ShannonEntropy = std.Round(probs.ShannonEntropy, 4)
            _Gini = std.Round(probs.Gini, 4)
        End If
    End Sub

End Class
