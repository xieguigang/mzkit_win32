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

    Public Shared Function CenterToMain(target As Form) As Point
        Dim rect As Rectangle = AppHost.ClientRectangle
        Dim sizeBack = rect.Size
        Dim posBase = rect.Location
        Dim sizeFore = target.Size

        Return New Point(
            posBase.X + (sizeBack.Width - sizeFore.Width) / 2,
            posBase.Y + (sizeBack.Height - sizeFore.Height) / 2
        )
    End Function
End Class