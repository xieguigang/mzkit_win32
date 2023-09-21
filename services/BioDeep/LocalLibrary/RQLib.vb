﻿Imports System.IO
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports RQL

''' <summary>
''' Model for local library file for supports the metabolite 
''' annotation on ms1/ms2 level
''' </summary>
Public Class RQLib : Implements IDisposable

    ReadOnly query As Resource

    Private disposedValue As Boolean

    Sub New(file As Stream)
        query = New Resource(New StreamPack(file))
    End Sub

    Public Function AddAnnotation(metabo As MetaLib)
        Dim packdata = MsgPackSerializer.SerializeObject(metabo)

    End Function

    ''' <summary>
    ''' just get metabolite annotation information
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns></returns>
    Public Function QueryMetabolites(name As String) As MetaLib()
        Dim list As NumericTagged(Of String)() = query.Get(query:=name).ToArray
        Dim keys As String() = list.Select(Function(m) m.value).Distinct.ToArray
        Dim result As MetaLib() = New MetaLib(keys.Length - 1) {}

        For i As Integer = 0 To keys.Length - 1
            result(i) = MsgPackSerializer.Deserialize(Of MetaLib)(query.ReadBuffer(keys(i)))
        Next

        Return result
    End Function


    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call query.Dispose()
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
