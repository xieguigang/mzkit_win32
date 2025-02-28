﻿#Region "Microsoft.VisualBasic::9a307f512ea6b976e99dc19653b86b47, mzkit\src\mzkit\mzkit\forms\frmLicense.vb"

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

'   Total Lines: 22
'    Code Lines: 15
' Comment Lines: 0
'   Blank Lines: 7
'     File Size: 726.00 B


' Class frmLicense
' 
'     Sub: Button1_Click, frmLicense_Load, Label4_Click, PictureBox2_Click
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.Language

Public Class frmLicense

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub frmLicense_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = getLicenseText()
        Label4.Text = Label4.Text.Replace("%s", My.User.Name)
        Label6.Text = $"Built Time: {GetType(frmLicense).Assembly.FromAssembly.BuiltTime.ToString}"
    End Sub

    Public Shared Function getLicenseText() As String
        Dim text As String = getLicenseFile.ReadAllText(throwEx:=False) Or "Missing LICENSE file!".AsDefault
        Dim normalize As String = text.LineTokens.JoinBy(vbCrLf)

        Return normalize
    End Function

    Public Shared Function getLicenseFile() As String
        Dim path As Value(Of String) = ""

        If (path = $"{App.HOME}/LICENSE").FileExists Then
            Return path
        ElseIf (path = $"{App.HOME}/../../src/mzkit/LICENSE").FileExists Then
            Return path
        ElseIf (path = $"{App.HOME}/../../LICENSE").FileExists Then
            Return path
        Else
            Return Nothing
        End If
    End Function

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

    End Sub
End Class
