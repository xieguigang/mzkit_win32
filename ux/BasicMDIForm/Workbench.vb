Imports System.Runtime.CompilerServices

Public NotInheritable Class Workbench

    Public Shared ReadOnly Property AppHost As AppHost

    Private Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub Hook(host As AppHost)
        _AppHost = host
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub Warning(msg As String)
        If AppHost Is Nothing Then
            Call ExportApis.MZKitWorkbenchIsNotRunning()
        Else
            Call _AppHost.Warning(msg)
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub StatusMessage(msg As String, Optional icon As Image = Nothing)
        If AppHost Is Nothing Then
            Call ExportApis.MZKitWorkbenchIsNotRunning()
        Else
            Call _AppHost.StatusMessage(msg, icon)
        End If
    End Sub

End Class