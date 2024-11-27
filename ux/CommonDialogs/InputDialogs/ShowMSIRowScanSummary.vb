Imports Microsoft.VisualBasic.Windows.Forms.DataValidation
Imports std = System.Math

Public Class ShowMSIRowScanSummary

    Public Property files As String()

    Public ReadOnly Property cutoff As Double
        Get
            Return Val(TextBox3.Text)
        End Get
    End Property

    Public ReadOnly Property matrixMz As Double
        Get
            Return Val(TextBox4.Text)
        End Get
    End Property

    Public ReadOnly Property resolution As Double
        Get
            Return Val(TextBox5.Text)
        End Get
    End Property

    Public ReadOnly Property norm As Boolean
        Get
            Return CheckBox1.Checked
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim res = TextBox5.ValidateDouble
        Dim into = TextBox3.ValidateDouble(pip:=res)
        Dim ion = TextBox4.ValidateDouble(pip:=into)

        If {res, into, ion}.Any(Function(i) i Is Nothing) Then
            Return
        End If

        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub ShowMSIRowScanSummary_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call loadMeasure()
    End Sub

    Private Async Sub loadMeasure()
        Dim max As String = Await getMaxFile(files)
        Dim w As (n As Integer, basePeak As Double) = Await MeasureWidth(max)

        Call Me.Invoke(
            Sub()
                TextBox1.Text = w.n
                TextBox2.Text = files.Length.ToString
                TextBox4.Text = w.basePeak
                Label5.Text = "Done!"
                Button1.Enabled = True
            End Sub)
    End Sub

    Private Shared Async Function getMaxFile(files As String()) As Task(Of String)
        Return Await Task.Run(Function()
                                  Return files _
                                     .OrderByDescending(Function(path) path.FileLength) _
                                     .First
                              End Function)
    End Function

    Public Delegate Function CheckMatrixBaseIon(fileName As String) As (n As Integer, basePeak As Double)

    Shared m_checkMatrixBaseIon As CheckMatrixBaseIon

    Public Shared Sub HookIonReader(checkMatrixBaseIon As CheckMatrixBaseIon)
        m_checkMatrixBaseIon = checkMatrixBaseIon
    End Sub

    Private Shared Async Function MeasureWidth(fileName As String) As Task(Of (n As Integer, basePeak As Double))
        Return Await Task.Run(Function() m_checkMatrixBaseIon(fileName))
    End Function

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged
        If Val(TextBox4.Text) <= 0 Then
            TextBox3.Text = std.Min(0.001, Val(TextBox3.Text))
        Else
            TextBox3.Text = std.Max(0.05, Val(TextBox3.Text))
        End If
    End Sub
End Class