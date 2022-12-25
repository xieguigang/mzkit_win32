Imports WeifenLuo.WinFormsUI.Docking

''' <summary>
''' the main framework of the workbench desktop application and the plugin system base
''' </summary>
Public Interface AppHost

    ReadOnly Property DockPanel As DockPanel

    Sub SetWindowState(stat As FormWindowState)
    Sub Warning(msg As String)
    Sub StatusMessage(msg As String, Optional icon As Image = Nothing)

    Function GetWindowState() As FormWindowState

End Interface