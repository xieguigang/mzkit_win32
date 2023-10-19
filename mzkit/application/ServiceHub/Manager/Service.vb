Namespace ServiceHub.Manager

    Public Class Service

        Public Property PID As Integer
        Public Property Name As String
        Public Property Description As String
        Public Property Port As Integer
        Public Property Protocol As String
        Public Property StartTime As String
        Public Property CPU As Double
        Public Property Memory As Long
        Public Property isAlive As Boolean
        Public Property HouseKeeping As Boolean

        Public Overrides Function ToString() As String
            Return $"[{PID}] {Name}"
        End Function

    End Class

End Namespace