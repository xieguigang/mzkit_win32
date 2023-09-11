Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports std = System.Math

Public Class frmSetClusterNumbers

    Public ReadOnly Property TopNClusters As Integer
        Get
            Return CInt(NumericUpDown1.Value)
        End Get
    End Property

    Public ReadOnly Property GetIonMode As IonModes
        Get
            Return Provider.ParseIonMode(ComboBox1.SelectedValue.ToString)
        End Get
    End Property

    Public ReadOnly Property GetAdducts As MzCalculator()
        Get
            Dim list As New List(Of MzCalculator)

            For i As Integer = 0 To CheckedListBox1.Items.Count - 1
                If CheckedListBox1.GetItemChecked(i) Then
                    Call list.Add(CheckedListBox1.Items(i))
                End If
            Next

            Return list.ToArray
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TopNClusters <= 0 Then
            MessageBox.Show("Invalid cluster numbers! Number specificed must be a positive integer number!", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        Else

        End If

        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub frmSetClusterNumbers_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 0
        UpdateAdducts()
    End Sub

    Private Sub UpdateAdducts()
        Dim adducts As MzCalculator()

        Call CheckedListBox1.Items.Clear()

        If GetIonMode = IonModes.Positive Then
            adducts = Provider.Positives
        Else
            adducts = Provider.Negatives
        End If

        For Each item As MzCalculator In adducts
            Call CheckedListBox1.Items.Add(item)

            If std.Abs(item.charge) = 1 AndAlso item.M = 1 Then
                Call CheckedListBox1.SetItemChecked(CheckedListBox1.Items.Count - 1, True)
            Else
                Call CheckedListBox1.SetItemChecked(CheckedListBox1.Items.Count - 1, False)
            End If
        Next
    End Sub
End Class