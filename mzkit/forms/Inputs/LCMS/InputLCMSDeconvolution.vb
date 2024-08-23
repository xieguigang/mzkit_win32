Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Linq

Public Class InputLCMSDeconvolution

    Public ReadOnly Property massDiff As Double
        Get
            Return NumericUpDown1.Value
        End Get
    End Property

    Public ReadOnly Property rt_win As Double()
        Get
            Return New Double() {NumericUpDown2.Value, NumericUpDown3.Value}
        End Get
    End Property

    Public ReadOnly Property n_threads As Integer
        Get
            Return TrackBar1.Value
        End Get
    End Property

    Public ReadOnly Property export_file As String
        Get
            Return TextBox1.Text.GetFullPath
        End Get
    End Property

    Public ReadOnly Property files As String()

    Public ReadOnly Property input_raw As String
        Get
            If files.Length = 1 Then
                Return files(0)
            Else
                Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".txt", App.PID, "LCMS_rawdatas")
                Call files.SaveTo(tempfile)
                Return tempfile
            End If
        End Get
    End Property

    Public Sub SetFiles(files As IEnumerable(Of String))
        _files = files.SafeQuery.Select(Function(path) path.GetFullPath).ToArray

        For Each file As String In files
            Dim row = ListView1.Items.Add(file.FileName)
            row.SubItems.Add(StringFormats.Lanudry(bytes:=file.FileLength))
        Next
    End Sub

    Private Sub InputLCMSDeconvolution_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TrackBar1_ValueChanged()
    End Sub

    Private Sub TrackBar1_ValueChanged() Handles TrackBar1.ValueChanged
        Label5.Text = $"{TrackBar1.Value} CPU"
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If TextBox1.Text.StringEmpty(, True) Then
            MessageBox.Show("The filepath for export the peaktable file should not be empty!", "No save path", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If
        If NumericUpDown2.Value > NumericUpDown3.Value Then
            MessageBox.Show("The min rt window size should not be greater than the max rt window size!", "Invalid rt window", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using file As New SaveFileDialog With {.Filter = "Excel table file(*.csv)|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                TextBox1.Text = file.FileName
            End If
        End Using
    End Sub
End Class