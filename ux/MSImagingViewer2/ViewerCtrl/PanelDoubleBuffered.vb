
Imports System.ComponentModel

Public Class PanelDoubleBuffered : Inherits Panel
    Implements ISupportInitialize

    Public Sub New()
        DoubleBuffered = True
        UpdateStyles()
    End Sub

    Public Sub BeginInit() Implements ISupportInitialize.BeginInit

    End Sub

    Public Sub EndInit() Implements ISupportInitialize.EndInit

    End Sub
End Class