
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D
Imports Microsoft.VisualBasic.Serialization.JSON

<ClassInterface(ClassInterfaceType.AutoDual)>
<ComVisible(True)>
Public Class LCMSScatter

    ''' <summary>
    ''' the scatter raw data in current view range
    ''' </summary>
    Dim rawdata As Dictionary(Of String, Meta)

    Friend colorScaler As ScalerPalette = ScalerPalette.FlexImaging
    Friend callback As Action(Of String, Double, Double)

    Public Function GetLCMSScatter() As String
        Return rawdata.Values.ToArray.GetJson
    End Function

    Public Function GetColors() As String
        Return Designer.GetColors(colorScaler.Description, n:=30).Select(Function(a) a.ToHtmlColor).ToArray.GetJson
    End Function

    Public Sub Click(id As String)
        Dim meta = rawdata(id)

        Call callback(id, meta.mz, meta.scan_time)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="meta">data type of this parameter may be inherits from this data type, JSON encode will crashed due to the reason of we set base type to the json encoder</param>
    Public Sub SetMetadata(meta As IEnumerable(Of Meta))
        rawdata = meta _
            .Select(Function(a) New Meta With {.id = a.id, .intensity = a.intensity, .mz = a.mz, .scan_time = a.scan_time}) _
            .ToDictionary(Function(m)
                              Return m.id
                          End Function)
    End Sub

    Public Sub LoadMesh(rawdata As Meta(), Optional n As Integer = 500)
        Me.rawdata = MeshGrid(rawdata, n) _
            .ToDictionary(Function(m)
                              Return m.id
                          End Function)
    End Sub

    Public Shared Iterator Function MeshGrid(rawdata As Meta(), Optional n As Integer = 1500) As IEnumerable(Of Meta)
        Dim mz_bin As New BlockSearchFunction(Of Meta)(rawdata, Function(i) i.mz, 1, fuzzy:=True)
        Dim mz_range As New DoubleRange(rawdata.Select(Function(a) a.mz))
        Dim rt_bin As New BlockSearchFunction(Of Meta)(rawdata, Function(i) i.scan_time, 5, fuzzy:=True)
        Dim rt_range As New DoubleRange(rawdata.Select(Function(a) a.scan_time))

        For Each mzi As Double In mz_range.Enumerate(n)
            Dim ls = mz_bin _
                .Search(New Meta With {.mz = mzi}, 0.3) _
                .OrderBy(Function(a) a.scan_time) _
                .ToArray
            Dim line = ls _
                .Select(Function(a) New PointF(a.scan_time, a.intensity)) _
                .BSpline(degree:=3) _
                .ToArray

            Call Application.DoEvents()

            For Each p As PointF In line
                Yield New Meta With {
                    .id = "",
                    .mz = mzi,
                    .scan_time = p.X,
                    .intensity = p.Y
                }
            Next
        Next

        For Each rti As Double In rt_range.Enumerate(n)
            Dim ls = rt_bin _
                .Search(New Meta With {.scan_time = rti}, 3) _
                .OrderBy(Function(a) a.mz) _
                .ToArray
            Dim line = ls _
                .Select(Function(a) New PointF(a.mz, a.intensity)) _
                .BSpline(degree:=3) _
                .ToArray

            Call Application.DoEvents()

            For Each p As PointF In line
                Yield New Meta With {
                    .id = "",
                    .mz = p.X,
                    .scan_time = rti,
                    .intensity = p.Y
                }
            Next
        Next
    End Function

End Class
