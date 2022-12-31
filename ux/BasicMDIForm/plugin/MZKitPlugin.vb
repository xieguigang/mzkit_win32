
Imports System.Reflection

''' <summary>
''' this dll assembly file is a mzkit plugin 
''' package, sevral plugin module may be 
''' contains in this project assembly file.
''' </summary>
<AttributeUsage(AttributeTargets.Assembly, AllowMultiple:=False)>
Public Class MZKitPlugin : Inherits Attribute

    Public Overrides Function ToString() As String
        Dim asm As Assembly = [GetType].Assembly
        Dim dll As String = asm.Location.FileName

        Return $"[{dll}] this dll assembly file is a mzkit plugin package, sevral plugin module may be contains in this project assembly file."
    End Function

    Public Shared Function GetFlag(asm As Assembly) As Boolean
        Dim attr = asm.GetCustomAttribute(Of MZKitPlugin)
        Return Not attr Is Nothing
    End Function
End Class