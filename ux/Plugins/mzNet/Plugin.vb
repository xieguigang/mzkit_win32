﻿Imports Galaxy.Workbench
Imports Mzkit_win32.BasicMDIForm

Public Class Plugin : Inherits Mzkit_win32.BasicMDIForm.Plugin

    Public Overrides ReadOnly Property guid As Guid
        Get
            Return Guid.Parse("0A3F853B-7ED7-D797-C990-808CAE5F1FE9")
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "Molecular Network Viewer Client"
        End Get
    End Property

    Public Overrides ReadOnly Property Link As String
        Get
            Return "https://mzkit.org/"
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Molecular Network Viewer Client"
        End Get
    End Property

    Public Overrides Sub Exec()
        Call CommonRuntime.ShowSingleDocument(Of FormViewer)()
    End Sub

    Public Overrides Function Init(println As Action(Of String)) As Boolean
        Return True
    End Function
End Class
