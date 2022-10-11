Imports System.Windows.Forms

Partial Public Class PropertyWindow
    Inherits ToolWindow

    Public Sub New()
        InitializeComponent()
        ComboBox.SelectedIndex = 0
        PropertyGrid.SelectedObject = PropertyGrid
    End Sub
End Class

