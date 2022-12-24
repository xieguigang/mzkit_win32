Imports WeifenLuo.WinFormsUI.Docking

''' <summary>
''' the main framework of the workbench desktop application and the plugin system base
''' </summary>
Public Interface AppHost

    ReadOnly Property DockPanel As DockPanel

    Sub SetWindowState(stat As FormWindowState)
    Function GetWindowState() As FormWindowState

End Interface