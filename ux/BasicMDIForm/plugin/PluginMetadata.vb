
Imports System.Runtime.InteropServices

<ClassInterface(ClassInterfaceType.AutoDual)>
<ComVisible(True)>
Public Class PluginMetadata

    Public Property id As String
    Public Property name As String
    Public Property desc As String
    Public Property ver As String
    Public Property author As String
    Public Property url As String

    ''' <summary>
    ''' "active" | "disable" | "incompatible"
    ''' </summary>
    ''' <returns></returns>
    Public Property status As String

    Public Overrides Function ToString() As String
        Return name
    End Function

End Class
