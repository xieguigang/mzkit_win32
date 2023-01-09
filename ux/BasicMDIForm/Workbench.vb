Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Tcp
Imports WeifenLuo.WinFormsUI.Docking

Public NotInheritable Class Workbench

    Public Shared ReadOnly Property AppHost As AppHost

    ''' <summary>
    ''' local http port for the ui view
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property WebPort As Integer = TCPExtensions.GetFirstAvailablePort(-1)
    Public Shared ReadOnly Property MSIServiceAppPort As Integer

    Private Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub SetMSIServicesAppPort(appPort As Integer)
        _MSIServiceAppPort = appPort
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub Hook(host As AppHost)
        _AppHost = host
    End Sub

    ''' <summary>
    ''' write text to the log output window
    ''' </summary>
    ''' <param name="text"></param>
    Public Shared Sub LogText(text As String)
        Call AppHost.LogText(text)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub Warning(msg As String)
        If AppHost Is Nothing Then
            Call ExportApis.MZKitWorkbenchIsNotRunning()
        Else
            Call _AppHost.Warning(msg)
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub StatusMessage(msg As String, Optional icon As Image = Nothing)
        If AppHost Is Nothing Then
            Call ExportApis.MZKitWorkbenchIsNotRunning()
        Else
            Call _AppHost.StatusMessage(msg, icon)
        End If
    End Sub

    Public Shared Function CenterToMain(target As Form) As Point
        Dim rect As Rectangle = AppHost.ClientRectangle
        Dim sizeBack = rect.Size
        Dim posBase = rect.Location
        Dim sizeFore = target.Size

        Return New Point(
            posBase.X + (sizeBack.Width - sizeFore.Width) / 2,
            posBase.Y + (sizeBack.Height - sizeFore.Height) / 2
        )
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="showExplorer">
    ''' do specific callback from this parameter delegate if the pointer value is nothing nothing
    ''' </param>
    Public Shared Function ShowSingleDocument(Of T As {New, DockContent})(Optional showExplorer As Action = Nothing) As T
        Dim DockPanel As DockPanel = AppHost.DockPanel
        Dim targeted As T = DockPanel.Documents _
            .Where(Function(doc) TypeOf doc Is T) _
            .FirstOrDefault

        If targeted Is Nothing Then
            targeted = New T
        End If

        If Not showExplorer Is Nothing Then
            Call showExplorer()
        End If

        targeted.Show(DockPanel)
        targeted.DockState = DockState.Document

        Return targeted
    End Function

    Public Shared Sub Dock(win As ToolWindow, prefer As DockState)
        Select Case win.DockState
            Case DockState.Hidden, DockState.Unknown
                win.DockState = prefer
            Case DockState.Float, DockState.Document,
                 DockState.DockTop,
                 DockState.DockRight,
                 DockState.DockLeft,
                 DockState.DockBottom

                ' do nothing 
            Case DockState.DockBottomAutoHide
                win.DockState = DockState.DockBottom
            Case DockState.DockLeftAutoHide
                win.DockState = DockState.DockLeft
            Case DockState.DockRightAutoHide
                win.DockState = DockState.DockRight
            Case DockState.DockTopAutoHide
                win.DockState = DockState.DockTop
        End Select
    End Sub

    ''' <summary>
    ''' create a new document tab page
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Shared Function ShowDocument(Of T As {New, DocumentWindow})(Optional status As DockState = DockState.Document,
                                                                       Optional title As String = Nothing) As T
        Dim newDoc As New T()

        newDoc.Show(AppHost.DockPanel)
        newDoc.DockState = status

        If Not title.StringEmpty Then
            newDoc.TabText = title
        End If

        Return newDoc
    End Function
End Class