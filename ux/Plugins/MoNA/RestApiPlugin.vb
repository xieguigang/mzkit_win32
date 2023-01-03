Imports Mzkit_win32.BasicMDIForm

Public Class RestApiPlugin : Inherits Plugin

    Public Overrides ReadOnly Property guid As Guid
        Get
            Return Guid.Parse("ED3BF83F-1047-4D4B-B797-1CACB116DFE4")
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "MoNA Online Service"
        End Get
    End Property

    Public Overrides ReadOnly Property Link As String
        Get
            Return "https://apps.mzkit.org/"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Query LCMS ms2 spectrum and metabolite annotation information from the MoNA database online service."
        End Get
    End Property

    Public Overrides Sub Exec()

    End Sub

    Public Overrides Function Init(println As Action(Of String)) As Boolean
        Return True
    End Function
End Class
