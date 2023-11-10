Imports Microsoft.VisualBasic.ApplicationServices

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
        Public Property CommandLine As String


        Public Overrides Function ToString() As String
            Return $"[{PID}] {Name}"
        End Function

        Public Shared Function GetCommandLine(proc As Process) As String
            Return $"{proc.StartInfo.FileName.CLIPath} {proc.StartInfo.Arguments}"
        End Function

    End Class

End Namespace