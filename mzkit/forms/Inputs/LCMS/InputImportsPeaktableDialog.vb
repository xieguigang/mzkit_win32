Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports any = Microsoft.VisualBasic.Scripting

Public Class InputImportsPeaktableDialog

    Dim sampleinfo As New Dictionary(Of String, SampleInfo())
    Dim editMode As Boolean = False

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub CheckedListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CheckedListBox1.SelectedIndexChanged
        Dim group_label As String = any.ToString(CheckedListBox1.SelectedItem)

        If group_label.StringEmpty Then
            Return
        End If

        Dim samples = sampleinfo(group_label)

        Call ListBox2.Items.Clear()

        For Each sample In samples
            Call ListBox2.Items.Add(sample.ID)
        Next

        TextBox1.Text = group_label
        TextBox1.ReadOnly = True
        editMode = False

        If samples.Length > 0 Then
            PictureBox1.BackColor = samples(0).color.TranslateColor
        Else

        End If
    End Sub

    Private Sub ListBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox2.SelectedIndexChanged

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If Not editMode Then
            Return
        End If

        Using dialog As New ColorDialog With {.Color = PictureBox1.BackColor}
            If dialog.ShowDialog = DialogResult.OK Then
                PictureBox1.BackColor = dialog.Color
            End If
        End Using
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        editMode = True
        TextBox1.ReadOnly = False
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked

    End Sub

    ''' <summary>
    ''' delete a sample group
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RemoveToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles RemoveToolStripMenuItem1.Click

    End Sub

    ''' <summary>
    ''' delete a sample from group
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RemoveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveToolStripMenuItem.Click

    End Sub

    ''' <summary>
    ''' clear all sample group
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ClearToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem.Click

    End Sub

    ''' <summary>
    ''' Clear all sample from current group
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ClearToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem1.Click

    End Sub

    Private Sub AddToSampleGroup(sender As Object, e As EventArgs)
        Dim name As String = any.ToString(ListBox1.SelectedItem)

        If name.StringEmpty Then
            Return
        End If

        Dim groupName As String = DirectCast(sender, ToolStripItem).Tag
        Dim sample As New SampleInfo With {.ID = name, .sample_name = name}

    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged

    End Sub

    ''' <summary>
    ''' create new sample group
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CreateNewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateNewToolStripMenuItem.Click
        Dim unix As String = App.UnixTimeStamp.ToString.Replace(".", "")
        Dim id As String = $"sample_{unix}"

        CheckedListBox1.Items.Add(id)
        sampleinfo.Add(id, {})

        Dim menu = AddToSampleGroupToolStripMenuItem.DropDownItems.Add(id)
        menu.Tag = id
        AddHandler menu.Click, AddressOf AddToSampleGroup
    End Sub
End Class