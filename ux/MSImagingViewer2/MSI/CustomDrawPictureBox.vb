Imports System.Drawing

Public Class CustomDrawPictureBox : Inherits PictureBox

    Public onDraw As Action(Of Graphics)

    Protected Overrides Sub OnPaint(pe As PaintEventArgs)
        MyBase.OnPaint(pe)

        If Not onDraw Is Nothing Then
            Call onDraw(pe.Graphics)
        End If
    End Sub

End Class