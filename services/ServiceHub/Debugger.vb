Imports System.Net
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("debugger")>
Module Debugger

    ReadOnly debug_local As IPEndPoint = New Microsoft.VisualBasic.Net.IPEndPoint("127.0.0.1", 0)

    <ExportAPI("load_session")>
    Public Function loadSession(raw As String) As ProtocolHandler
        Dim app As New MSI()
        Dim caller As New ProtocolHandler(app, debug:=True)

        Call caller.HandleRequest(MSIRequest.LoadMSIRawdata(raw), debug_local)

        Return caller
    End Function

End Module
