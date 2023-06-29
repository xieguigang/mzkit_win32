#Region "Microsoft.VisualBasic::dc28c510f6114085c9989a12f0869326, mzkit\src\mzkit\mzkit\forms\Inputs\InputXICTarget.vb"

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

'   Total Lines: 28
'    Code Lines: 23
' Comment Lines: 0
'   Blank Lines: 5
'     File Size: 809.00 B


' Class InputXICTarget
' 
'     Properties: XICTarget
' 
'     Sub: Button1_Click, Button2_Click, SetIons
' 
' /********************************************************************************/

#End Region

Public Class InputXICTarget

    ''' <summary>
    ''' get target m/z to plot XIC
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property XICTarget As Double
        Get
            If RadioButton1.Checked Then
                If ComboBox1.Items.Count = 0 OrElse ComboBox1.SelectedIndex = -1 Then
                    Return 0
                Else
                    Return Val(ComboBox1.SelectedItem)
                End If
            Else
                If TextBox1.Text.IsSimpleNumber Then
                    Return Val(TextBox1.Text)
                Else
                    Return 0
                End If
            End If
        End Get
    End Property

    Public Sub SetIons(mz As IEnumerable(Of Double))
        ComboBox1.Items.Clear()

        For Each mzi As Double In mz
            Call ComboBox1.Items.Add(mzi.ToString("F4"))
        Next
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged

    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        RadioButton2.Checked = True
        RadioButton1.Checked = False
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        RadioButton1.Checked = True
        RadioButton2.Checked = False
    End Sub
End Class
