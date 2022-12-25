Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo("mzkit_win32")>

''' <summary>
''' export the function pointers from the mzkit workbench application
''' </summary>
Public Module ExportApis

#Region "mzkit_win32 api function pointers"
    Friend _openMSImagingFile As Action(Of String)
    Friend _openMSImagingViewer As Action
    Friend _getHEMapTool As Func(Of ToolStripWindow)
    Friend _getHEMapImage As Func(Of Bitmap)
#End Region

    Public Event OpenHEMapTool(tool As ToolStripWindow)

    Friend Sub _openHEMapTool(tool As ToolStripWindow)
        RaiseEvent OpenHEMapTool(tool)
    End Sub

    Public Function GetHEMapImage() As Bitmap
        If _getHEMapImage Is Nothing Then
            Return MZKitWorkbenchIsNotRunning()
        Else
            Return _getHEMapImage()
        End If
    End Function

    Public Function GetHEMapTool() As ToolStripWindow
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