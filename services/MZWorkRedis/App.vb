Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Parallel

<Package("RedisApp")>
Public Module App

    <ExportAPI("run")>
    Public Function Start(port As Integer) As Boolean
        Return Service.Start(port).Run
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="path"></param>
    ''' <param name="app"></param>
    ''' <returns>
    ''' all the redis key of the scan ms1 object
    ''' </returns>
    <ExportAPI("load.mzpack")>
    Public Function LoadMzPack(path As String, app As Integer) As Object
        Dim resq As New RequestStream(Service.Protocol, Protocols.LoadMzPack, path)
        Dim data = New TcpRequest(app).SendMessage(resq)

        If data.IsHTTP_RFC Then
            Return data.GetUTF8String
        Else
            Return data.GetUTF8String.LoadJSON(Of String())
        End If
    End Function

    <ExportAPI("scan1")>
    Public Function getScan1(key As String, app As Integer) As Object
        Dim hMemPointer = GetRedisMemoryMapping(key, app)

        If hMemPointer.Invalid Then
            Return hMemPointer.GetMappingFileName
        Else
            Return hMemPointer.GetObject(GetType(RedisScan1))
        End If
    End Function

    Public Function GetRedisMemoryMapping(key As String, app As Integer) As MapObject
        Dim res As New RequestStream(Service.Protocol, Protocols.GetValue, key)
        Dim data = New TcpRequest(app).SendMessage(res)

        If data.IsHTTP_RFC Then
            Return New UnmanageMemoryRegion With {
                .memoryFile = data.GetUTF8String,
                .size = -1
            }
        Else
            Return data.GetUTF8String _
                .LoadJSON(Of UnmanageMemoryRegion) _
                .GetMemoryMap
        End If
    End Function

    <ExportAPI("scan2")>
    Public Function getScan2(key As String, app As Integer) As Object
        Dim hMemPointer = GetRedisMemoryMapping(key, app)

        If hMemPointer.Invalid Then
            Return hMemPointer.GetMappingFileName
        Else
            Return hMemPointer.GetObject(GetType(ScanMS2))
        End If
    End Function
End Module
