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
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap.hqx
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON

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
    <Description("The source file path of current opened ms-imaging raw data.")>
    <Category("imzML")> Public ReadOnly Property sourceFile As String

    <Description("the unique guid of the target imzML file/mzPack/SCiLS csv data file.")>
    <Category("imzML")> Public ReadOnly Property UUID As String
    <Description("The total pixel numbers in X axis.")>
    <Category("imzML")> Public ReadOnly Property scan_x As Integer
    <Description("The total pixel numbers in Y axis.")>
    <Category("imzML")> Public ReadOnly Property scan_y As Integer
    <Description("The scan resolution size of each pixel.")>
    <Category("imzML")> Public Property resolution As Double

    <Description("The number of the ion which has metabolite annotation data in current ms-imaging data.")>
    Public ReadOnly Property num_annotations As Integer
    <Description("The data file type descriptor, which could be used for determine the file type for run imaging data visualization")>
    Public ReadOnly Property app As FileApplicationClass

    <Description("The width/height aspect ratio of the raw sample slide.")>
    Public Property scale_ratio As SizeF = New SizeF(1, 1)

    ''' <summary>
    ''' The calculated physical width of the slide image in the real world, this width is evaluated based on the scan number in x axis and the MSI resolution value
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <Description("The calculated physical width of the slide image in the real world, this width value is evaluated based on the scan number in x axis and the MSI resolution value.")>
    Public ReadOnly Property physical_width As String
        Get
            Return ((scan_x * resolution) / 1000).ToString("F2") & "mm"
        End Get
    End Property

    <Description("The calculated physical height of the slide image in the real world, this height value is evaluated based on the scan number in y axis and the MSI resolution value.")>
    Public ReadOnly Property physical_height As String
        Get
            Return ((scan_y * resolution) / 1000).ToString("F2") & "mm"
        End Get
    End Property

    <Description("MS instrument tag of current raw data")>
    Public ReadOnly Property instrument As String
    <Description("The m/z ion data polarity mode, this property value will affects the ion metabolite annotation function in MZKit!")>
    Public Property polarity As IonModes = IonModes.Positive

    <Description("Set the background color of the ms-imaging output from this option.")>
    <Category("Render")> Public Property background As Color
    <Category("Render")>
    <Description("The level of Hqx pixel scaler algorithm.")>
    Public Property Hqx As HqxScales = HqxScales.Hqx_2x

    <Description("The scaled color set palette name. This option will change the heatmap color maps.")>
    <Category("Render")> Public Property colors As ScalerPalette = ScalerPalette.viridis
    <Description("the color depth levels of the color sequence which is generated from a specific scaler palette.")>
    <Category("Render")> Public Property mapLevels As Integer = 250
    <Description("knn fill range for the pixel data")>
    <Category("Render")> Public Property knn As Integer = 3
    <Description("knn fill cutoff value for the pixel density around a pixel")>
    <Category("Render")> Public Property knn_qcut As Double = 0.65
    <Description("The up scale algorithm for display image on the canvas winform control.")>
    <Category("Render")> Public Property scale As InterpolationMode = InterpolationMode.Bilinear
    <Description("Enable the image filter processor algorithm for optimise the heatmap image output?")>
    <Category("Render")> Public Property enableFilter As Boolean = True
    <Description("Show the overlap of the physical width ruler on the ms-imaging output?")>
    <Category("Render")> Public Property showPhysicalRuler As Boolean = True
    <Description("Show the overlap of total ion imaging plot in grayscale when do single ion/rgb ion imaging?")>
    <Category("Render")> Public Property showTotalIonOverlap As Boolean = True

    <Description("The mass tolerance error threshold in delta dalton or ppm.")>
    <Category("Pixel M/z Data")> Public Property tolerance As Double = 0.1
    <Description("The ion m/z mass tolerance algorithm for measure two ion m/z value is equals to each other.")>
    <Category("Pixel M/z Data")> Public Property method As ToleranceMethod = ToleranceMethod.Da
    <Description("The x coordinate of the pixel which has the max intensity value.")>
    <Category("Intensity")> Public ReadOnly Property basePeak_x As Integer
    <Description("The y coordinate of the pixel which has the max intensity value.")>
    <Category("Intensity")> Public ReadOnly Property basePeak_y As Integer
    <Description("The min value of current intensity range.")>
    <Category("Intensity")> Public ReadOnly Property min As Double
    <Description("The max value of current intensity range.")>
    <Category("Intensity")> Public ReadOnly Property max As Double

    <Description("The TrIQ cutoff threshold, value in range of [0,1]")>
    <Category("Intensity")> Public Property TrIQ As Double = 0.85

    Sub New(info As Dictionary(Of String, String))
        scan_x = Integer.Parse(info!scan_x)
        scan_y = Integer.Parse(info!scan_y)

        If info.ContainsKey("uuid") Then
            UUID = info!uuid
        Else
            UUID = info!UUID
        End If

        fileSize = info!fileSize
        sourceFile = info.TryGetValue({"source", NameOf(sourceFile)}, [default]:="in-memory sample")
        instrument = If(sourceFile.ExtensionSuffix("csv", "slx"), "Bruker", "Thermo Fisher")
        resolution = info.TryGetValue("resolution", [default]:=17)
        background = info.TryGetValue(NameOf(background), [default]:="black").TranslateColor
        num_annotations = info.TryGetValue({"ion_annotations", "num_annotations"}, [default]:=0)
        app = [Enum].Parse(GetType(FileApplicationClass), info.TryGetValue("app", [default]:=FileApplicationClass.MSImaging.ToString))
        scale_ratio = info.TryGetValue("scale_ratio", [default]:="1,1").FloatSizeParser
        polarity = Provider.ParseIonMode(info.TryGetValue("polarity", [default]:="+"))
        Hqx = [Enum].Parse(GetType(HqxScales), info.TryGetValue(NameOf(Hqx), [default]:="None"))
        colors = [Enum].Parse(GetType(ScalerPalette), info.TryGetValue(NameOf(colors), [default]:="viridis"))
        mapLevels = info.TryGetValue(NameOf(mapLevels), [default]:=250)
        enableFilter = info.TryGetValue(NameOf(enableFilter), [default]:="true").ParseBoolean
        showPhysicalRuler = info.TryGetValue(NameOf(showPhysicalRuler), [default]:="true").ParseBoolean
        showTotalIonOverlap = info.TryGetValue(NameOf(showTotalIonOverlap), [default]:="true").ParseBoolean
    End Sub

    Sub New(render As Drawer)
        scan_x = render.dimension.Width
        scan_y = render.dimension.Height
        background = Color.Black
        resolution = render.pixelReader.resolution

        If TypeOf render.pixelReader Is ReadIbd Then
            UUID = DirectCast(render.pixelReader, ReadIbd).UUID
            fileSize = DirectCast(render.pixelReader, ReadIbd).ibd _
                .size _
                .DoCall(AddressOf StringFormats.Lanudry)
        End If
    End Sub

    Sub New()
        background = Color.Black
        resolution = 17
    End Sub

    Sub New(scan_x As Integer, scan_y As Integer)
        Call Me.New

        Me.scan_x = scan_x
        Me.scan_y = scan_y
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

    Public Sub SetIntensityMax(max As Double, basePeak_xy As Point)
        _min = 0
        _max = max
        _basePeak_x = basePeak_xy.X
        _basePeak_y = basePeak_xy.Y
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

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetMSIDimension() As Size
        Return New Size(scan_x, scan_y)
    End Function

    Shared ReadOnly css_data As PropertyInfo() = GetType(MsImageProperty) _
        .GetProperties(DataFramework.PublicProperty) _
        .Where(Function(p) p.GetIndexParameters.IsNullOrEmpty) _
        .ToArray

    Public Function GetJSON() As String
        Dim json As New Dictionary(Of String, String)
        Dim val As Object
        Dim str As String

        For Each p As PropertyInfo In css_data
            val = p.GetValue(Me)

            If val Is Nothing Then
                str = ""
            Else
                Select Case p.PropertyType
                    Case GetType(Color) : str = DirectCast(val, Color).ToHtmlColor
                    Case GetType(SizeF) : str = $"{DirectCast(val, SizeF).Width},{DirectCast(val, SizeF).Height}"
                    Case Else
                        If DataFramework.IsPrimitive(p.PropertyType) Then
                            str = val.ToString
                        ElseIf p.PropertyType.IsEnum Then
                            str = val.ToString
                        Else
                            Throw New NotImplementedException(p.PropertyType.FullName)
                        End If
                End Select
            End If

            json.Add(p.Name, str)
        Next

        Return json.GetJson(simpleDict:=True)
    End Function

    Public Shared Function ParseJSON(json As String) As MsImageProperty
        Dim js = json.LoadJSON(Of Dictionary(Of String, String))

        If js Is Nothing Then
            Return Nothing
        Else
            Return New MsImageProperty(js)
        End If
    End Function
End Class
