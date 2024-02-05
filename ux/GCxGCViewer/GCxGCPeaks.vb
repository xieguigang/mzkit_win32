
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class GCxGCPeak

    Public Property t1 As Double
    Public Property t2 As Double
    Public Property into As Double

    Sub New()
    End Sub

    Sub New(t1 As Double, t2 As Double, into As Double)
        _t1 = t1
        _t2 = t2
        _into = into
    End Sub

    Public Overrides Function ToString() As String
        Return $"[{t1}x{t2} -> {into}]"
    End Function

End Class

<ClassInterface(ClassInterfaceType.AutoDual)>
<ComVisible(True)>
Public Class GCxGCPeaks

    Friend colorScaler As ScalerPalette = ScalerPalette.FlexImaging
    Friend callback As Action(Of String, Double, Double)
    Friend rawdata As GCxGCPeak()

    Public Function GetLCMSScatter() As String
        If rawdata.IsNullOrEmpty Then
            Return "[]"
        Else
            Return rawdata.GetJson
        End If
    End Function

    Public Function GetColors() As String
        Return Designer.GetColors(colorScaler.Description, n:=30).Select(Function(a) a.ToHtmlColor).ToArray.GetJson
    End Function

    Public Sub Click(id As String)
        ' Dim meta = rawdata(id)

        ' Call callback(id, meta.mz, meta.scan_time)
    End Sub

    Public Sub SetMetadata(rawdata As D2Chromatogram())
        Me.rawdata = rawdata _
            .Select(Function(d)
                        Return d.chromatogram.Select(Function(ti) New GCxGCPeak(d.scan_time, ti.Time, ti.Intensity))
                    End Function) _
            .IteratesALL _
            .ToArray
    End Sub

    Public Sub LoadMesh(rawdata As D2Chromatogram(), Optional n As Integer = 500)
        Me.SetMetadata(rawdata)
        Me.rawdata = MeshGrid(Me.rawdata, n).ToArray
    End Sub

    Public Shared Iterator Function MeshGrid(rawdata As GCxGCPeak(), Optional n As Integer = 1500) As IEnumerable(Of GCxGCPeak)
        Dim mz_bin As New BlockSearchFunction(Of GCxGCPeak)(rawdata, Function(i) i.t1, 1, fuzzy:=True)
        Dim mz_range As New DoubleRange(rawdata.Select(Function(a) a.t1))
        Dim rt_bin As New BlockSearchFunction(Of GCxGCPeak)(rawdata, Function(i) i.t2, 5, fuzzy:=True)
        Dim rt_range As New DoubleRange(rawdata.Select(Function(a) a.t2))

        For Each mzi As Double In mz_range.Enumerate(n)
            Dim ls = mz_bin _
                .Search(New GCxGCPeak(mzi, 0, 0), 0.3) _
                .OrderBy(Function(a) a.t2) _
                .ToArray
            Dim line = ls _
                .Select(Function(a) New PointF(a.t2, a.into)) _
                .BSpline(degree:=3) _
                .ToArray

            Call Application.DoEvents()

            For Each p As PointF In line
                Yield New GCxGCPeak With {
                    .t1 = mzi,
                    .t2 = p.X,
                    .into = p.Y
                }
            Next
        Next

        For Each rti As Double In rt_range.Enumerate(n)
            Dim ls = rt_bin _
                .Search(New GCxGCPeak With {.t2 = rti}, 3) _
                .OrderBy(Function(a) a.t1) _
                .ToArray
            Dim line = ls _
                .Select(Function(a) New PointF(a.t1, a.into)) _
                .BSpline(degree:=3) _
                .ToArray

            Call Application.DoEvents()

            For Each p As PointF In line
                Yield New GCxGCPeak With {
                    .t1 = p.X,
                    .t2 = rti,
                    .into = p.Y
                }
            Next
        Next
    End Function

End Class
