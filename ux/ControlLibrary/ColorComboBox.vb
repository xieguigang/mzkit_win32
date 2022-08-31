Imports Microsoft.VisualBasic.Imaging

Public Class ColorComboBox : Inherits ComboBox

    Sub New()
        Call MyBase.New()

        DrawMode = DrawMode.OwnerDrawFixed
        DropDownStyle = ComboBoxStyle.DropDownList

    End Sub

    Private Sub ColorComboBox_DrawItem(sender As Object, e As DrawItemEventArgs) Handles Me.DrawItem
        e.DrawBackground()
        e.DrawFocusRectangle()

        If e.Index < 0 Then
            Return
        End If

        Dim color As Color = Items(e.Index)
        Dim brush As New SolidBrush(color)
        Dim g = e.Graphics
        Dim rect = e.Bounds

        rect.Inflate(-2, -2)

        Dim rectColor As New Rectangle(rect.Location, New Size(20, rect.Height))

        g.DrawRectangle(New Pen(e.ForeColor), rectColor)
        g.FillRectangle(brush, rectColor)
        g.DrawString(color.ToHtmlColor, e.Font, New SolidBrush(e.ForeColor), rect.X + 22, rect.Y)
    End Sub
End Class
