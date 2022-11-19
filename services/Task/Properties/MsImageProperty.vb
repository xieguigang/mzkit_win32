#Region "Microsoft.VisualBasic::3c1902cfa348339902c17a93833d8b71, mzkit\src\mzkit\Task\Properties\MsImageProperty.vb"

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

'   Total Lines: 163
'    Code Lines: 103
' Comment Lines: 36
'   Blank Lines: 24
'     File Size: 6.86 KB


' Enum SmoothFilters
' 
'     Gauss, GaussMax, GaussMean, GaussMedian, GaussMin
'     Max, Mean, Median, Min, None
' 
'  
' 
' 
' 
' Class MsImageProperty
' 
'     Properties: background, colors, fileSize, lowerbound, mapLevels
'                 max, maxCut, method, min, pixel_height
'                 pixel_width, scale, scan_x, scan_y, tolerance
'                 upperbound, UUID
' 
'     Constructor: (+3 Overloads) Sub New
' 
'     Function: GetMSIInfo, GetTolerance
' 
'     Sub: Reset, SetIntensityMax
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap.hqx
Imports Microsoft.VisualBasic.Linq
Imports ServiceHub

Public Enum SmoothFilters
    None
    Gauss
    Median
    Mean
    Min
    Max
    GaussMedian
    GaussMean
    GaussMin
    GaussMax
End Enum

Public Class MsImageProperty

    <Description("The raw data file size.")>
    <Category("imzML")> Public ReadOnly Property fileSize As String
    <Category("imzML")> Public ReadOnly Property sourceFile As String

    <Description("the unique guid of the target imzML file/mzPack/SCiLS csv data file.")>
    <Category("imzML")> Public ReadOnly Property UUID As String
    <Description("The total pixel numbers in X axis.")>
    <Category("imzML")> Public ReadOnly Property scan_x As Integer
    <Description("The total pixel numbers in Y axis.")>
    <Category("imzML")> Public ReadOnly Property scan_y As Integer
    <Description("The scan resolution size of each pixel.")>
    <Category("imzML")> Public Property resolution As Double

    Public ReadOnly Property physical_width As String
        Get
            Return ((scan_x * resolution) / 1000).ToString("F2") & "mm"
        End Get
    End Property

    Public ReadOnly Property physical_height As String
        Get
            Return ((scan_y * resolution) / 1000).ToString("F2") & "mm"
        End Get
    End Property

    Public ReadOnly Property instrument As String

    <Category("Render")> Public Property background As Color
    <Category("Render")>
    <Description("The level of Hqx pixel scaler algorithm.")>
    Public Property Hqx As HqxScales = HqxScales.Hqx_2x

    <Description("The scaled color set palette name.")>
    <Category("Render")> Public Property colors As ScalerPalette = ScalerPalette.viridis
    <Description("the color depth levels of the color sequence which is generated from a specific scaler palette.")>
    <Category("Render")> Public Property mapLevels As Integer = 120
    <Description("knn fill range for the pixel data")>
    <Category("Render")> Public Property knn As Integer = 3
    <Category("Render")> Public Property knn_qcut As Double = 0.65
    <Category("Render")> Public Property scale As InterpolationMode = InterpolationMode.Bilinear
    <Category("Render")> Public Property enableFilter As Boolean = True
    ' <Category("Render")> Public Property overlap_TIC As Boolean = True

    <Description("The mass tolerance error threshold in delta dalton or ppm.")>
    <Category("Pixel M/z Data")> Public Property tolerance As Double = 0.1
    <Category("Pixel M/z Data")> Public Property method As ToleranceMethod = ToleranceMethod.Da

    <Category("Intensity")> Public ReadOnly Property min As Double
    <Category("Intensity")> Public ReadOnly Property max As Double

    <Description("The TrIQ cutoff threshold, value in range of [0,1]")>
    <Category("Intensity")> Public Property TrIQ As Double = 0.85
    <Description("Show the color map legend on the canvas display?")>
    <Category("Intensity")> Public Property showColorMap As Boolean = True

    Sub New(render As Drawer)
        scan_x = render.dimension.Width
        scan_y = render.dimension.Height
        background = Color.Black
        resolution = render.pixelReader.resolution

        If TypeOf render.pixelReader Is ReadIbd Then
            UUID = DirectCast(render.pixelReader, ReadIbd).UUID
            fileSize = DirectCast(render.pixelReader, ReadIbd) _
                .ibd _
                .size _
                .DoCall(AddressOf StringFormats.Lanudry)
        End If
    End Sub

    Sub New()
        background = Color.Black
        resolution = 17
    End Sub

    Sub New(info As Dictionary(Of String, String))
        scan_x = Integer.Parse(info!scan_x)
        scan_y = Integer.Parse(info!scan_y)
        UUID = info!uuid
        fileSize = info!fileSize
        sourceFile = info.TryGetValue("source", "in-memory sample")
        instrument = If(sourceFile.ExtensionSuffix("csv", "slx"), "Bruker", "Thermo Fisher")
        resolution = info.TryGetValue("resolution", [default]:=17)
        background = Color.Black
    End Sub

    Public Shared Function Empty(dimension As Size) As MsImageProperty
        Dim data As New Dictionary(Of String, String) From {
            {"scan_x", dimension.Width},
            {"scan_y", dimension.Height},
            {"uuid", Now.ToString.MD5},
            {"fileSize", "0B"},
            {"source", "n/a"},
            {"resolution", 17}
        }

        Return New MsImageProperty(data)
    End Function

    Public Sub Reset(MsiDim As Size, UUID As String, fileSize As String, resolution As Double)
        _scan_x = MsiDim.Width
        _scan_y = MsiDim.Height
        _background = Color.Black
        _UUID = UUID
        _fileSize = fileSize
        _resolution = resolution
    End Sub

    Public Shared Function GetMSIInfo(render As Drawer) As Dictionary(Of String, String)
        Return MSIProtocols.GetMSIInfo(render)
    End Function

    'Public Function Smooth(img As Bitmap) As Bitmap
    '    If imageSmooth = SmoothFilters.None Then
    '        Return img
    '    End If

    '    Select Case imageSmooth
    '        Case SmoothFilters.Gauss : Return GaussBlur.GaussBlur(img)
    '        Case SmoothFilters.GaussMax : Return New Filters.Matrix(GaussBlur.GaussBlur(img)).GetSmoothBitmap(Matrix2DFilters.Max)
    '        Case SmoothFilters.GaussMin : Return New Filters.Matrix(GaussBlur.GaussBlur(img)).GetSmoothBitmap(Matrix2DFilters.Min)
    '        Case SmoothFilters.GaussMean : Return New Filters.Matrix(GaussBlur.GaussBlur(img)).GetSmoothBitmap(Matrix2DFilters.Mean)
    '        Case SmoothFilters.GaussMedian : Return New Filters.Matrix(GaussBlur.GaussBlur(img)).GetSmoothBitmap(Matrix2DFilters.Median)
    '        Case SmoothFilters.Max : Return New Filters.Matrix(img).GetSmoothBitmap(Matrix2DFilters.Max)
    '        Case SmoothFilters.Min : Return New Filters.Matrix(img).GetSmoothBitmap(Matrix2DFilters.Min)
    '        Case SmoothFilters.Mean : Return New Filters.Matrix(img).GetSmoothBitmap(Matrix2DFilters.Mean)
    '        Case SmoothFilters.Median : Return New Filters.Matrix(img).GetSmoothBitmap(Matrix2DFilters.Median)
    '        Case Else
    '            Throw New NotImplementedException
    '    End Select
    'End Function

    'Public Function RenderingColorMapLegend(pixelFilter As IEnumerable(Of PixelScanIntensity)) As Image
    '    Return pixelFilter.Select(Function(p) New PixelData(p.x, p.y, p.totalIon)).DoCall(AddressOf RenderingColorMapLegend)
    'End Function

    'Public Function RenderingColorMapLegend(pixelFilter As IEnumerable(Of PixelData)) As Image
    '    Dim colorMapLegend As New ColorMapLegend(colors.Description, mapLevels) With {
    '        .format = "G3",
    '        .ticks = pixelFilter.Select(Function(p) p.intensity).Range.CreateAxisTicks,
    '        .tickAxisStroke = Stroke.TryParse(Stroke.AxisStroke).GDIObject,
    '        .tickFont = CSSFont.TryParse(CSSFont.Win7Normal).GDIObject(100),
    '        .title = "Intensity",
    '        .titleFont = CSSFont.TryParse(CSSFont.Win7Large).GDIObject(100),
    '        .noblank = True
    '    }

    '    Return colorMapLegend.Draw(New Size(600, 1500))
    'End Function

    Public Shared Function Validation(p As MsImageProperty, e As PropertyValueChangedEventArgs) As String
        If p.knn < 0 Then
            p.knn = e.OldValue
            Return "the knn range can not be negative value!"
        ElseIf p.knn > 13 Then
            p.knn = e.OldValue
            Return "the knn range number is too large!"
        End If

        Return Nothing
    End Function

    Public Sub SetIntensityMax(max As Double)
        _min = 0
        _max = max
    End Sub

    Public Sub SetInstrument(name As String)
        _instrument = name
    End Sub

    Public Function GetTolerance() As Tolerance
        If method = ToleranceMethod.Da Then
            Return Ms1.Tolerance.DeltaMass(tolerance)
        Else
            Return Ms1.Tolerance.PPM(tolerance)
        End If
    End Function
End Class
