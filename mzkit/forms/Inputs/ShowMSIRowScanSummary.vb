Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader

Public Class ShowMSIRowScanSummary

    Public Property files As String()

    Public ReadOnly Property cutoff As Double
        Get
            Return Val(TextBox3.Text)
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
        Dim w As Integer = Await MeasureWidth(max)

        Me.Invoke(Sub() TextBox1.Text = w)
        Me.Invoke(Sub() TextBox2.Text = files.Length.ToString)
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

    Private Shared Function MeasureWidth(fileName As String) As Task(Of Integer)
        Dim background = New Task(Of Integer)(
            Function()
                Dim Xraw As New MSFileReader(fileName)
                Dim n As Integer = Xraw.ScanMax

                Return n
            End Function)
        background.Start()
        Return background
    End Function
End Class