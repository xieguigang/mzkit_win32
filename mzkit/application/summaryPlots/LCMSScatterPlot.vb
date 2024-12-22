Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Imaging
Imports Mzkit_win32.MatrixViewer
Imports MZKitWin32.Blender.CommonLibs
Imports SMRUCC.Rsharp.Runtime.Vectorization

Public Class LCMSScatterPlot : Inherits SummaryPlot

    Public Overrides ReadOnly Property requiredFields As Dictionary(Of String(), String)
        Get
            Dim fields As New Dictionary(Of String(), String)

            fields({"mz", "MZ", "m/z"}) = "the mass to charge value for the ms1 ions"
            fields({"rt", "RT", "retention time", "retention_time"}) = "the retention time of the ms1 ions"
            fields({"intensity", "area", "into"}) = "the peak expression value of the ms1 ions"

            Return fields
        End Get
    End Property

    Public Overrides ReadOnly Property appName As String
        Get
            Return "LCMS scatter plot"
        End Get
    End Property

    Public Overrides Function Plot(table As DataTable) As Object
        Dim mz As Double() = CLRVector.asNumeric(getFieldVector(table, {"mz", "MZ", "m/z"}))
        Dim rt As Double() = CLRVector.asNumeric(getFieldVector(table, {"rt", "RT", "retention time", "retention_time"}))
        Dim intensity As Double() = CLRVector.asNumeric(getFieldVector(table, {"intensity", "area", "into"}))
        Dim raw As IEnumerable(Of ms1_scan) = mz.Select(Function(mzi, i) New ms1_scan(mzi, rt(i), intensity(i)))
        Dim matrix As New Ms1ScatterMatrix("LCMS Scatter Plot", raw)
        Dim args As New PlotProperty With {
            .point_size = 5
        }
        Dim size As New Size(2400, 1800)

        Return matrix.Plot(args, size).AsGDIImage
    End Function
End Class
