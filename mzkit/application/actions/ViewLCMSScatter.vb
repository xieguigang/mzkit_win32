Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports SMRUCC.Rsharp.Runtime.Vectorization

Public Class ViewLCMSScatter : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "View LCMS scatter data"
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, table As DataTable)
        Dim fieldNames As IEnumerable(Of String) = GetFieldNames(table)
        Dim config As New InputLoadLCMSScatter

        Call config.SetDataFeilds(fieldNames)
        Call InputDialog.Input(
            setConfig:=Sub(fields)
                           Dim rt = CLRVector.asNumeric(table.getFieldVector(fields.RtField))
                           Dim mz = CLRVector.asNumeric(table.getFieldVector(fields.MzField))
                           Dim into = CLRVector.asNumeric(table.getFieldVector(fields.DataField))
                           Dim label As String() = Nothing

                           If Not fields.LabelField Is Nothing Then
                               label = CLRVector.asCharacter(table.getFieldVector(fields.LabelField))
                           End If

                           Dim viewer = VisualStudio.ShowDocument(Of frmLCMSScatterViewer)(title:="LCMS Scatter")
                           Dim scatter As Meta() = mz _
                               .Select(Function(mzi, i)
                                           Return New Meta(mzi, rt(i), into(i), label.ElementAtOrNull(i))
                                       End Function) _
                               .ToArray

                           Call viewer.loadRaw(scatter)
                       End Sub,
            config:=config)
    End Sub
End Class
