Imports WeifenLuo.WinFormsUI.Docking

''' <summary>
''' the main framework of the workbench desktop application and the plugin system base
''' </summary>
Public Interface AppHost

    ReadOnly Property DockPanel As DockPanel
    ReadOnly Property ClientRectangle As Rectangle

    Event ResizeForm(newPos As Point, newSize As Size)
    Event CloseWorkbench(args As FormClosingEventArgs)

    Sub SetWorkbenchVisible(visible As Boolean)
    Sub SetWindowState(stat As FormWindowState)
    Sub Warning(msg As String)
    Sub StatusMessage(msg As String, Optional icon As Image = Nothing)
    Sub LogText(text As String)
    Sub ShowProperties(obj As Object)

    ''' <summary>
    ''' BioNovoGene MZKit Workbench [{title}]
    ''' </summary>
    ''' <param name="title"></param>
    Sub SetTitle(title As String)

    Function GetWindowState() As FormWindowState
    Function GetDesktopLocation() As Point
    Function GetClientSize() As Size

End Interface