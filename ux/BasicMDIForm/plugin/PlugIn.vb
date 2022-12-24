Public MustInherit Class Plugin

    Public MustOverride ReadOnly Property Name As String
    Public MustOverride ReadOnly Property Link As String
    Public MustOverride ReadOnly Property Description As String

    Public MustOverride Sub Init()

    Public Shared Sub LoadPlugins(dir As String)

    End Sub

End Class