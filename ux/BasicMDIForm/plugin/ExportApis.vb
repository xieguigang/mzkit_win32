Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo("mzkit_win32")>

Public Module ExportApis

    Friend _openMSImagingFile As Action(Of String)
    Friend _openMSImagingViewer As Action
    Friend _getHEMapTool As Func(Of Form)

    Public Function GetHEMapTool() As Form
        If _getHEMapTool Is Nothing Then
            Return MZKitWorkbenchIsNotRunning()
        Else
            Return _getHEMapTool()
        End If
    End Function

    Public Sub OpenMSImagingViewer()
        If _openMSImagingViewer Is Nothing Then
            Call MZKitWorkbenchIsNotRunning()
        Else
            Call _openMSImagingViewer()
        End If
    End Sub

    Public Sub OpenMSImagingFile(file As String)
        If _openMSImagingFile Is Nothing Then
            Call MZKitWorkbenchIsNotRunning()
        Else
            Call _openMSImagingFile(file)
        End If
    End Sub

    Friend Function MZKitWorkbenchIsNotRunning() As Object
        Throw New InvalidProgramException("Call this api code required of the MZKit workbench application instance!")
    End Function
End Module