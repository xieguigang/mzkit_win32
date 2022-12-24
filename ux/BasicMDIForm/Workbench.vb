Public NotInheritable Class Workbench

    Public Shared ReadOnly Property AppHost As AppHost

    Private Sub New()
    End Sub

    Public Shared Sub Hook(host As AppHost)
        _AppHost = host
    End Sub

End Class