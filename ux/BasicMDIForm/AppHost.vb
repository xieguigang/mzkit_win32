Imports WeifenLuo.WinFormsUI.Docking

''' <summary>
''' the main framework of the workbench desktop application and the plugin system base
''' </summary>
Public Interface AppHost

    ReadOnly Property DockPanel As DockPanel

    Property WindowState As FormWindowState

End Interface