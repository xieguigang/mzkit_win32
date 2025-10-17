Imports System.Runtime.CompilerServices
Imports Galaxy.Workbench
Imports Galaxy.Workbench.DockDocument.Presets
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualStudio.WinForms.Docking
Imports Mzkit_win32.BasicMDIForm.RibbonLib.Controls
Imports RibbonLib

''' <summary>
''' The winform framework for the workbench UI
''' </summary>
Public NotInheritable Class Workbench

    Public Shared ReadOnly Property AppHost As AppHost
        Get
            Return CommonRuntime.AppHost
        End Get
    End Property

    Public Shared ReadOnly Property AppHostForm As Form
        Get
            Return TryCast(CObj(AppHost), Form)
        End Get
    End Property

    Public Shared Property SplashBannerImage As Image = My.Resources.Home_Logo_Link

    ''' <summary>
    ''' local http port for the ui view
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property WebPort As Integer = TCPExtensions.GetFirstAvailablePort(-1)
    Public Shared ReadOnly Property MSIServiceAppPort As Integer
    Public Shared ReadOnly Property RibbonItems As RibbonItems
    Public Shared Property AppRunning As Boolean = True
    ''' <summary>
    ''' the user login status session id for use biodeep services
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property BioDeepSession As String

    Public Shared ReadOnly parametersTool As New AdjustParameters
    Public Shared ReadOnly propertyWin As New PropertyWindow

    Private Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub SetMSIServicesAppPort(appPort As Integer)
        _MSIServiceAppPort = appPort
    End Sub

    Public Shared Sub SetSessionId(ssid As String)
        _BioDeepSession = ssid
    End Sub

    Public Shared Function SetRibbonMenu(ribbon As Ribbon) As RibbonItems
        _RibbonItems = New RibbonItems(ribbon)
        Return RibbonItems
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub Hook(host As AppHost)
        Call CommonRuntime.Hook(host)
    End Sub

    ''' <summary>
    ''' write text to the log output window
    ''' </summary>
    ''' <param name="text"></param>
    Public Shared Sub LogText(text As String)
        If Not AppHost Is Nothing Then
            Call AppHost.LogText(text)
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub HostInvoke(act As Action)
        Call AppHostForm.Invoke(Sub() act())
    End Sub

    ''' <summary>
    ''' Show status bar message with a warning icon
    ''' </summary>
    ''' <param name="msg"></param>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub Warning(msg As String)
        If AppHost Is Nothing Then
            Call ExportApis.MZKitWorkbenchIsNotRunning()
        Else
            Call AppHost.Warning(msg)
        End If
    End Sub

    ''' <summary>
    ''' Show status bar message with a success icon
    ''' </summary>
    ''' <param name="msg"></param>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub SuccessMessage(msg As String)
        If AppHost Is Nothing Then
            Call ExportApis.MZKitWorkbenchIsNotRunning()
        Else
            Call AppHost.StatusMessage(msg, My.Resources._1200px_Checked_svg)
        End If
    End Sub

    ''' <summary>
    ''' Show status bar message with a default message icon, <paramref name="icon"/> 
    ''' could be changed via a specific image value has been specific at here.
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <param name="icon"></param>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub StatusMessage(msg As String, Optional icon As Image = Nothing)
        If AppHost Is Nothing Then
            Call ExportApis.MZKitWorkbenchIsNotRunning()
        Else
            Call AppHost.StatusMessage(msg, icon)
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

    Public Shared Sub ShowProperties(obj As Object)
        If BaseHook.showProperties IsNot Nothing Then
            Call BaseHook.showProperties(obj)
        End If
    End Sub

    ''' <summary>
    ''' get color palette that used for do chartting plots
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetPlotColors() As String
        Return BaseHook.getColorSet()
    End Function
End Class