Imports Mzkit_win32.BasicMDIForm

Public Class Plugin : Inherits Mzkit_win32.BasicMDIForm.Plugin

    Public Overrides ReadOnly Property guid As Guid
        Get
            Return Guid.Parse("073F853B-7ED7-D797-CB90-844CAE5F1FE9")
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "BioDeep Annotation Report Viewer"
        End Get
    End Property

    Public Overrides ReadOnly Property Link As String
        Get
            Return "https://apps.mzkit.org/"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "View the metabolite annotation result which comes from the biodeep annotation workflow."
        End Get
    End Property

    Public Overrides Sub Exec()
        Call Workbench.ShowSingleDocument(Of FormViewer)()
    End Sub

    Public Overrides Function Init(println As Action(Of String)) As Boolean
        Return True
    End Function
End Class
