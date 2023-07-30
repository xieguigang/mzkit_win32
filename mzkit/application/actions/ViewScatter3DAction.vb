Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports SMRUCC.Rsharp.Runtime.Vectorization

Public Class ViewScatter3DAction : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "View cluster data in 3d scatter style, the input data should be contains 3 dimensions at least."
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, table As DataTable)
        Dim fieldNames As IEnumerable(Of String) = GetFieldNames(table)
        Dim config As New Input3DScatter
        Dim mask As MaskForm = MaskForm.CreateMask(frm:=Workbench.AppHost)

        Call config.LoadLabels(fieldNames)

        If mask.ShowDialogForm(config) = DialogResult.OK Then
            Dim points As IEnumerable(Of UMAPPoint) = createPoints(config, table)
            Dim viewer As New frm3DScatterPlotView()

            Call viewer.LoadScatter(points, Nothing)
            Call VisualStudio.ShowDocument(viewer)
        End If
    End Sub

    Private Iterator Function createPoints(config As Input3DScatter, table As DataTable) As IEnumerable(Of UMAPPoint)
        Dim fields = config.GetLabels
        Dim labels = CLRVector.asCharacter(table.getFieldVector(fields.labels))
        Dim cluster As String()
        Dim x = CLRVector.asNumeric(table.getFieldVector(fields.x))
        Dim y = CLRVector.asNumeric(table.getFieldVector(fields.y))
        Dim z = CLRVector.asNumeric(table.getFieldVector(fields.z))

        If fields.clusters Is Nothing Then
            cluster = New String(labels.Length - 1) {}
        Else
            cluster = CLRVector.asCharacter(table.getFieldVector(fields.clusters))
        End If

        For i As Integer = 0 To labels.Length - 1
            Yield New UMAPPoint With {
                .[class] = If(cluster(i), "NO_CLASS"),
                .label = labels(i),
                .x = x(i),
                .y = y(i),
                .z = z(i)
            }
        Next
    End Function
End Class
