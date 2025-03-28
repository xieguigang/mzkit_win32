﻿#Region "Microsoft.VisualBasic::9be69a205d7b1c29c8d8cbfc212f64a7, mzkit\src\mzkit\mzkit\forms\Inputs\InputMSIDimension.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 15
'    Code Lines: 11
' Comment Lines: 0
'   Blank Lines: 4
'     File Size: 506.00 B


' Class InputMSIDimension
' 
'     Properties: Dims
' 
'     Sub: Button1_Click
' 
' /********************************************************************************/

#End Region

Public Class InputMSIDimension

    Public ReadOnly Property Dims As String
    Public ReadOnly Property TotalScans As Integer
        Get
            Return Val(TextBox3.Text)
        End Get
    End Property

    Public Sub SetPredefineSize(size As Size)
        TextBox1.Text = size.Width
        TextBox2.Text = size.Height
    End Sub

    Public Sub SetTotalScans(n As Integer)
        TextBox3.Text = n
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text.StringEmpty OrElse TextBox2.Text.StringEmpty Then
            MessageBox.Show("Invalid size!", "Mzkit Win32", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        _Dims = $"{TextBox1.Text},{TextBox2.Text}"

        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    ''' <summary>
    ''' height
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        Dim h As Integer = Val(TextBox2.Text)
        Dim n = TotalScans

        If n > 0 Then
            TextBox1.Text = CInt(n / h)
        End If
    End Sub

    ''' <summary>
    ''' width
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Dim w As Integer = Val(TextBox1.Text)
        Dim n = TotalScans

        If n > 0 Then
            TextBox2.Text = CInt(n / w)
        End If
    End Sub
End Class
