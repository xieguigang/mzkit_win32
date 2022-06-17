Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.GCModeller.Workbench.KEGGReport
Imports stdNum = System.Math

Public Class PeakFindingAction : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Do peak finding, the selected column data should be the signal intensity value."
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, table As DataTable)
        Throw New NotImplementedException()
    End Sub
End Class
