Namespace nv

    Friend Class GraphicsAdapter
        Public Property Name As String
        Public Property DriverVersion As String
        Public Property SupportLevel As SupportLevel

        Public Sub New(name As String, Optional ver As String = "n/a")
            Me.Name = name
            DriverVersion = ver
        End Sub
    End Class

    Friend Enum SupportLevel
        None
        [Partial]
        Full
    End Enum
End Namespace
