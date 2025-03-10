﻿Public Class SetMSIPlotParameters

    Public Property SetDir As Boolean = False

    Public ReadOnly Property FileName As String
        Get
            Return TextBox1.Text
        End Get
    End Property

    Public ReadOnly Property SelectedPath As String
        Get
            Return TextBox1.Text
        End Get
    End Property

    Public ReadOnly Property GetPlotSize As Size
        Get
            Return New Size(NumericUpDown1.Value, NumericUpDown2.Value)
        End Get
    End Property

    Public ReadOnly Property GetPlotDpi As Integer
        Get
            Return NumericUpDown3.Value
        End Get
    End Property

    Public ReadOnly Property GetPlotPadding As String
        Get
            Dim t = NumericUpDown4.Value
            Dim r = NumericUpDown5.Value
            Dim b = NumericUpDown6.Value
            Dim l = NumericUpDown7.Value

            Return $"padding: {t}px {r}px {b}px {l}px;"
        End Get
    End Property

    Public Function SetFileName(filename As String) As SetMSIPlotParameters
        TextBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) & "\" & filename & ".png"
        Return Me
    End Function

    Public Function SetFolder(dir As String) As SetMSIPlotParameters
        TextBox1.Text = dir
        Return Me
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If SetDir Then
            Using folder As New FolderBrowserDialog With {.ShowNewFolderButton = True}
                If folder.ShowDialog = DialogResult.OK Then
                    TextBox1.Text = folder.SelectedPath
                End If
            End Using
        Else
            Using file As New SaveFileDialog With {
                .FileName = TextBox1.Text,
                .Filter = "Image File(*.png)|*.png|Scalable Vector Graphics(*.svg)|*.svg|PDF image(*.pdf)|*.pdf"
            }
                If file.ShowDialog = DialogResult.OK Then
                    TextBox1.Text = file.FileName
                End If
            End Using
        End If
    End Sub

    ''' <summary>
    ''' w/h
    ''' </summary>
    Dim ratio As Double

    ''' <summary>
    ''' change the width
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        Dim size As Size = GetPlotSize

        If CheckBox1.Checked Then
            ' andalso change the height
            ' r = w / h -> h = w/r
            NumericUpDown2.Value = size.Width / ratio
        Else
            ratio = size.Width / size.Height
        End If
    End Sub

    ''' <summary>
    ''' change the height
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub NumericUpDown2_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown2.ValueChanged
        Dim size As Size = GetPlotSize

        If CheckBox1.Checked Then
            ' and also change the width
            ' r = w / h -> w = r * h
            NumericUpDown1.Value = size.Height * ratio
        Else
            ratio = size.Width / size.Height
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class