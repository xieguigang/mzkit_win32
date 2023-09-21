Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports RQL

''' <summary>
''' Model for local library file for supports the metabolite 
''' annotation on ms1/ms2 level
''' </summary>
Public Class RQLib

    ReadOnly query As Resource

    Sub New(file As Stream)
        query = New Resource(New StreamPack(file))
    End Sub
End Class
