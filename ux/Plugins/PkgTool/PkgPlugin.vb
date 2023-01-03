Imports Mzkit_win32.BasicMDIForm

Public Class PkgPlugin : Inherits Plugin

    Public Overrides ReadOnly Property guid As Guid
        Get
            Return Guid.Parse("B08A1CC7-24DC-3767-00CF-F1929D9F788C")
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "MZKit Plugin Package Tool"
        End Get
    End Property

    Public Overrides ReadOnly Property Link As String
        Get
            Return "https://apps.mzkit.org/"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Tools for create mzkit plugin package."
        End Get
    End Property

    Public Overrides Sub Exec()
        Call Workbench.ShowSingleDocument(Of Form1)()
    End Sub

    Public Overrides Function Init(println As Action(Of String)) As Boolean
        Return True
    End Function
End Class