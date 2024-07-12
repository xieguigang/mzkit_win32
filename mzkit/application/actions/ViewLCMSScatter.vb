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
                           Dim viewer = VisualStudio.ShowDocument(Of frmLCMSScatterViewer)()
                           Dim scatter As Meta() = mz _
                               .Select(Function(mzi, i) New Meta(mzi, rt(i), into(i))) _
                               .ToArray

                           Call viewer.loadRaw(scatter)
                       End Sub,
            config:=config)
    End Sub
End Class
