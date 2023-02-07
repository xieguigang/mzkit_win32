Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON

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
        Dim resp = New TcpRequest(app).SendMessage(resq)

        If resp.IsHTTP_RFC Then
            Return resp.GetUTF8String
        Else
            Return resp.GetUTF8String.LoadJSON(Of String())
        End If
    End Function
End Module
