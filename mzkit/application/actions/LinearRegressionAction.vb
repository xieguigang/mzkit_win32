Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Mzkit_win32.BasicMDIForm
Imports SMRUCC.Rsharp.Runtime.Vectorization

Public Class LinearRegressionAction : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Create linear regression model for make targetted quantification data"
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, table As DataTable)
        Dim fieldNames As Index(Of String) = GetFieldNames(table).Indexing
        Dim requires As String() = {"name", "rtmin", "rtmax", "area", "baseline", "maxinto", "source"}

        ' check of the required field names
        If Not fieldNames.ValidateSchemaNames(requires) Then
            Call MessageBox.Show("Some required data fields is missing from your data table, check of the headers inside your table:" &
                            vbCrLf & vbCrLf &
                            requires.Select(Function(name, i) $"{i + 1}. {name}").JoinBy(vbCrLf),
                            "Missing Required Data",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Stop)
            Return
        End If

        Dim names As String() = CLRVector.asCharacter(table.getFieldVector("name"))
        Dim rtmin As Double() = CLRVector.asNumeric(table.getFieldVector("rtmin"))
        Dim rtmax As Double() = CLRVector.asNumeric(table.getFieldVector("rtmax"))
        Dim area As Double() = CLRVector.asNumeric(table.getFieldVector("area"))
        Dim baseline As Double() = CLRVector.asNumeric(table.getFieldVector("baseline"))
        Dim maxinto As Double() = CLRVector.asNumeric(table.getFieldVector("maxinto"))
        Dim source As String() = CLRVector.asCharacter(table.getFieldVector("source"))
        Dim ions As IonPeakTableRow() = names _
            .Select(Function(name, i)
                        Return New IonPeakTableRow With {
                            .TPA = area(i),
                            .base = baseline(i),
                            .maxinto = maxinto(i),
                            .Name = name(i),
                            .ID = .Name,
                            .rtmin = rtmin(i),
                            .rtmax = rtmax(i),
                            .raw = source(i)
                        }
                    End Function) _
            .ToArray
        Dim files = ions.GroupBy(Function(i) i.raw) _
            .Select(Function(i)
                        Return New DataFile With {.filename = i.Key, .ionPeaks = i.ToArray}
                    End Function) _
            .ToArray
        Dim page As QuantificationLinearPage = DirectCast(VisualStudio.ShowSingleDocument(Of frmTargetedQuantification), QuantificationLinearPage)

        Call page.RunLinearFileImports(files, type:=TargetTypes.MRM)
    End Sub
End Class
