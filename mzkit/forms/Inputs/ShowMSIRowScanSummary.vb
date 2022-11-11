Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports Microsoft.VisualBasic.Math
Imports stdNum = System.Math

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

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub ShowMSIRowScanSummary_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call loadMeasure()
    End Sub

    Private Async Sub loadMeasure()
        Dim max As String = Await getMaxFile(files)
        Dim w As (n As Integer, basePeak As Double) = Await MeasureWidth(max)

        Me.Invoke(Sub() TextBox1.Text = w.n)
        Me.Invoke(Sub() TextBox2.Text = files.Length.ToString)
        Me.Invoke(Sub() TextBox4.Text = w.basePeak)
        Me.Invoke(Sub() Label5.Text = "Done!")
    End Sub

    Private Shared Function getMaxFile(files As String()) As Task(Of String)
        Dim background = New Task(Of String)(
            Function()
                Return files _
                    .OrderByDescending(Function(path) path.FileLength) _
                    .First
            End Function)
        background.Start()
        Return background
    End Function

    Private Shared Function MeasureWidth(fileName As String) As Task(Of (n As Integer, basePeak As Double))
        Dim background = New Task(Of (n As Integer, basePeak As Double))(
            Function()
                Dim Xraw As New MSFileReader(fileName)
                Dim n As Integer = Xraw.ScanMax
                Dim allscans = New XRawStream(Xraw).StreamTo
                Dim basePeak As Double = allscans.MS _
                    .Select(Function(a) a.GetMs.OrderByDescending(Function(i) i.intensity).FirstOrDefault) _
                    .Where(Function(mzi) Not mzi Is Nothing) _
                    .GroupBy(Function(x) x.mz, offsets:=0.3) _
                    .OrderByDescending(Function(ni) ni.Length) _
                    .First _
                    .name _
                    .ParseDouble

                Return (n, basePeak)
            End Function)
        background.Start()
        Return background
    End Function

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged
        If Val(TextBox4.Text) <= 0 Then
            TextBox3.Text = stdNum.Min(0.001, Val(TextBox3.Text))
        Else
            TextBox3.Text = stdNum.Max(0.05, Val(TextBox3.Text))
        End If
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged

    End Sub
End Class