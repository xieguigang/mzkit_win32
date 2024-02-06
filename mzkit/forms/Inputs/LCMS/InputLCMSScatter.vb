Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Class InputLCMSScatter

    Dim samplegroups As Dictionary(Of String, SampleInfo())

    Public Sub SetSamples(samples As IEnumerable(Of SampleInfo))
        samplegroups = samples.GroupBy(Function(s) s.sample_info).ToDictionary(Function(s) s.Key, Function(s) s.ToArray)
        ComboBox1.Items.Clear()
        ComboBox2.Items.Clear()

        For Each item In samplegroups
            ComboBox1.Items.Add(item.Key)
        Next

        Call SetGroup(samplegroups.First.Key, samplegroups.First.Value)
    End Sub

    Private Sub SetGroup(group As String, samples As SampleInfo())
        ComboBox1.SelectedIndex = ComboBox1.Items.IndexOf(group)
        ComboBox2.Items.Clear()

        For Each sample As SampleInfo In samples
            ComboBox2.Items.Add(sample.ID)
        Next

        ComboBox2.SelectedIndex = 0
    End Sub

    Public ReadOnly Property PlotSampleGroup As Boolean
        Get
            Return RadioButton1.Checked
        End Get
    End Property

    Public ReadOnly Property PlotSource As String
        Get
            If PlotSampleGroup Then
                Return ComboBox1.SelectedItem.ToString
            Else
                Return ComboBox2.SelectedItem.ToString
            End If
        End Get
    End Property

    Public Iterator Function GetCurrentSamples() As IEnumerable(Of String)
        For Each sample In samplegroups(ComboBox1.SelectedItem.ToString)
            Yield sample.ID
        Next
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class