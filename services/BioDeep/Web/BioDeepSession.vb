#Region "Microsoft.VisualBasic::0e73df1ec52deee6f31eaf33b829501d, mzkit\services\BioDeep\Web\BioDeepSession.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 123
    '    Code Lines: 88 (71.54%)
    ' Comment Lines: 9 (7.32%)
    '    - Xml Docs: 77.78%
    ' 
    '   Blank Lines: 26 (21.14%)
    '     File Size: 4.87 KB


    ' Class BioDeepSession
    ' 
    '     Properties: cookieName, ssid
    ' 
    '     Function: CheckSession, GetSessionInfo, GetString, headerProvider, Login
    '               Request, RequestStream
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.Net.Http

''' <summary>
''' 登录状态信息使用<see cref="SingletonHolder(Of BioDeepSession)"/>进行保存
''' </summary>
Public Class BioDeepSession

    Public Property cookieName As String
    Public Property ssid As String

    Private Function headerProvider() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {{"Cookie", $"PHPSESSID={ssid};"}}
    End Function

    ''' <summary>
    ''' 检查是否处于登录状态
    ''' </summary>
    ''' <returns></returns>
    Public Function CheckSession() As Boolean
        Dim url$ = "https://query.biodeep.cn/ping.vbs"
        Dim text As String = url.GET(headers:=headerProvider)
        Dim json As JsonObject = MessageParser.ParseMessage(text)
        Dim result As Boolean = json.success

        Return result
    End Function

    Public Function GetSessionInfo() As SessionInfo
        Dim result As JsonObject = Request(api:="http://my.biodeep.cn/services/session_info.vbs")

        If result.success Then
            Return DirectCast(result!info, JsonObject).CreateObject(Of SessionInfo)(decodeMetachar:=True)
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function GetString(url As String, Optional headers As Dictionary(Of String, String) = Nothing) As String
        If SingletonHolder(Of BioDeepSession).Instance Is Nothing OrElse SingletonHolder(Of BioDeepSession).Instance.ssid.StringEmpty(, True) Then
            Return Nothing
        End If

        Dim sessionHeader As Dictionary(Of String, String) = SingletonHolder(Of BioDeepSession).Instance.headerProvider()

        If Not headers.IsNullOrEmpty Then
            For Each item In headers
                sessionHeader(item.Key) = item.Value
            Next
        End If

        Return url.GET(headers:=sessionHeader)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Request(api As String, Optional headers As Dictionary(Of String, String) = Nothing) As JsonObject
        Return GetString(api, headers).DoCall(AddressOf MessageParser.ParseMessage)
    End Function

    Public Shared Function RequestStream(api As String, Optional headers As Dictionary(Of String, String) = Nothing) As Stream
        If SingletonHolder(Of BioDeepSession).Instance Is Nothing OrElse SingletonHolder(Of BioDeepSession).Instance.ssid.StringEmpty(, True) Then
            Return Nothing
        End If

        Dim sessionHeader As Dictionary(Of String, String) = SingletonHolder(Of BioDeepSession).Instance.headerProvider()
        Dim buffer As New MemoryStream

        If Not headers.IsNullOrEmpty Then
            For Each item In headers
                sessionHeader(item.Key) = item.Value
            Next
        End If

        Using webResponse As Stream = api.GetRequestRaw(headers:=sessionHeader)
            Dim chunk As Byte() = New Byte(4096 - 1) {}
            Dim nread As i32 = 0

            Do While True
                If (nread = webResponse.Read(chunk, Scan0, chunk.Length)) <= 0 Then
                    Exit Do
                Else
                    buffer.Write(chunk, Scan0, nread)
                End If
            Loop

            Call buffer.Seek(Scan0, SeekOrigin.Begin)
        End Using

        Return buffer '.UnGzipStream
    End Function

    Public Shared Function Login(account As String, passwordMd5 As String) As String
        Dim post As New NameValueCollection

        Call post.Add("account", account)
        Call post.Add("password", passwordMd5)

        Dim result As WebResponseResult = $"http://passport.biodeep.cn/passport/verify.vbs".POST(params:=post)
        Dim json As JsonObject = New JsonParser(result.html).OpenJSON()

        If json!code.AsString(decodeMetachar:=True) <> 0 Then
            Call MessageBox.Show("Account not found or incorrect password...", "BioDeep Login", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            ' session_id
            ' cookie_name
            json = json!debug

            SingletonHolder(Of BioDeepSession).Instance.cookieName = json!cookie_name.AsString(decodeMetachar:=True)
            SingletonHolder(Of BioDeepSession).Instance.ssid = json!session_id.AsString(decodeMetachar:=True)

            Return SingletonHolder(Of BioDeepSession).Instance.ssid
        End If

        Return Nothing
    End Function
End Class
