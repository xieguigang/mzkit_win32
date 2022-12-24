
Imports System.Reflection

''' <summary>
''' this dll assembly file is a mzkit plugin 
''' package, sevral plugin module may be 
''' contains in this project assembly file.
''' </summary>
<AttributeUsage(AttributeTargets.Assembly, AllowMultiple:=False)>
Public Class MZKitPlugin : Inherits Attribute

    Public Shared Function GetFlag(asm As Assembly) As Boolean

    End Function
End Class