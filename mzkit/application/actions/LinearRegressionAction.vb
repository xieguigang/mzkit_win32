Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Patterns
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
                            .Name = names(i),
                            .ID = .Name,
                            .rtmin = rtmin(i),
                            .rtmax = rtmax(i),
                            .raw = source(i)
                        }
                    End Function) _
            .ToArray
        Dim files As DataFile() = ions.GroupBy(Function(i) i.raw) _
            .Select(Function(i)
                        Return New DataFile With {.filename = i.Key, .ionPeaks = i.ToArray}
                    End Function) _
            .ToArray
        Dim selCals As New InputReferencePointNames
        Dim sampleNames = New CommonTagParser(files.Keys).GetTagNames.ToArray

        selCals.SetNames(files.Keys)
        selCals.Input(Sub(list)
                          Dim getErrName As Value(Of String) = ""
                          Dim cals As String() = DirectCast(list, InputReferencePointNames) _
                              .GetReferencePointNames(getErrName) _
                              .ToArray

                          If getErrName.Value <> "" Then
                              MessageBox.Show($"One of the reference point sample name '{getErrName.Value}' is mis-matched with the original input sample names.",
                                              "Mis-Matched Sample Name",
                                              MessageBoxButtons.OK,
                                              MessageBoxIcon.Stop)
                          Else
                              Dim page As QuantificationLinearPage = DirectCast(VisualStudio.ShowSingleDocument(Of frmTargetedQuantification), QuantificationLinearPage)
                              Dim nameMaps = cals _
                                  .Select(Function(name, i)
                                              Dim name_str As String = name.StringReplace("\(\d+\)", "").Trim

                                              If Not (name_str.IsPattern("\d+") OrElse name_str.IsPattern("\d+[\Wa-zA-Z]+")) Then
                                                  name_str = CommonTagParser.RemoveLeadingNumbersAndSymbols(name_str)

                                                  If name_str = "" Then
                                                      name_str = sampleNames(i)
                                                  End If
                                              End If

                                              Return New NamedValue(Of String)(name_str, cals(i))
                                          End Function) _
                                  .ToArray
                              Dim filter_cals As Index(Of String) = cals.Indexing

                              Call page.SetCals(nameMaps)

                              Try
                                  Call page.RunLinearFileImports(files.Where(Function(a) a.filename Like filter_cals).ToArray, type:=TargetTypes.MRM)
                                  Call page.LoadSampleFiles(files, AddressOf Workbench.StatusMessage)
                              Catch ex As Exception
                                  Call App.LogException(ex)
                                  Call Workbench.Warning(ex.Message)
                                  Call DirectCast(CObj(page), Form).Close()
                                  Call MessageBox.Show(ex.ToString, "Load Linear Data Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                              End Try
                          End If
                      End Sub)
    End Sub
End Class
