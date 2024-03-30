Imports Mzkit_win32.BasicMDIForm
Imports RibbonLib.Controls

Public Class RibbonEventBinding

    ReadOnly ribbon As RibbonButton

    ''' <summary>
    ''' binding an instance event to static ribbon event
    ''' </summary>
    Public evt As Action

    Sub New(btn As RibbonButton)
        ribbon = btn
        AddHandler ribbon.ExecuteEvent, Sub() Call exec_call()
    End Sub

    Private Sub exec_call()
        If Not evt Is Nothing Then
            Try
                Call evt()
            Catch ex As Exception
                Call Workbench.LogText($"[ribbon menu] error during exec for: {ribbon.Label}")
                Call Workbench.LogText(ex.ToString)
                Call App.LogException(ex)
            End Try
        Else
            Call Workbench.LogText($"[ribbon menu] no event handler was attached: {ribbon.Label}")
        End If
    End Sub

    Public Overrides Function ToString() As String
        Return ribbon.ToString
    End Function

End Class
