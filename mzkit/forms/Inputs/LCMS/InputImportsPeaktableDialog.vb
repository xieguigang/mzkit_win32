﻿Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports any = Microsoft.VisualBasic.Scripting

Public Class InputImportsPeaktableDialog

    Dim sampleinfo As New Dictionary(Of String, SampleInfo())
    Dim sampleGroups As New Dictionary(Of String, SampleGroup)
    Dim editMode As Boolean = False
    Dim current_group As String

    ''' <summary>
    ''' cancel
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    ''' <summary>
    ''' ok
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
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
        current_group = group_label

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

    ''' <summary>
    ''' edit
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        editMode = True
        TextBox1.ReadOnly = False
    End Sub

    ''' <summary>
    ''' save
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        If Not editMode Then
            Return
        Else
            editMode = False
            TextBox1.ReadOnly = True
        End If

        Dim old_group As SampleGroup = sampleGroups(current_group)
        old_group.sample_info = TextBox1.Text
        old_group.color = PictureBox1.BackColor.ToHtmlColor
        sampleGroups.Remove(current_group)
        current_group = old_group.sample_info
        sampleGroups.Add(current_group, old_group)
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
        Dim id As String = $"sample_group_{unix}"

        CheckedListBox1.Items.Add(id)
        sampleinfo.Add(id, {})
        sampleGroups.Add(id, New SampleGroup With {.sample_info = id, .color = "black"})

        Dim menu = AddToSampleGroupToolStripMenuItem.DropDownItems.Add(id)
        menu.Tag = id
        AddHandler menu.Click, AddressOf AddToSampleGroup
    End Sub

    Private Sub InputImportsPeaktableDialog_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call LoadSampleId("ID", "mz", "mzmin", "mzmax", "rt", "rtmin", "rtmax", "npeaks",
                          "NP_sample_18",
                          "Cal1", "Cal10", "Cal11", "Cal2", "Cal3", "Cal4", "Cal5", "Cal6", "Cal7", "Cal8", "Cal9",
                          "DZ1", "DZ2", "HB1", "HB2", "KB",
                          "NP_sample_1", "NP_sample_10", "NP_sample_11", "NP_sample_12", "NP_sample_13", "NP_sample_14", "NP_sample_15",
                          "NP_sample_16", "NP_sample_17", "NP_sample_19", "NP_sample_2", "NP_sample_20", "NP_sample_21", "NP_sample_22",
                          "NP_sample_23", "NP_sample_24", "NP_sample_25", "NP_sample_26", "NP_sample_27", "NP_sample_28", "NP_sample_29",
                          "NP_sample_3", "NP_sample_30", "NP_sample_31", "NP_sample_32", "NP_sample_33", "NP_sample_34", "NP_sample_35",
                          "NP_sample_36", "NP_sample_37", "NP_sample_38", "NP_sample_39", "NP_sample_4", "NP_sample_40", "NP_sample_41",
                          "NP_sample_42", "NP_sample_43", "NP_sample_44", "NP_sample_45", "NP_sample_46", "NP_sample_47", "NP_sample_48",
                          "NP_sample_49", "NP_sample_5", "NP_sample_50", "NP_sample_51", "NP_sample_52", "NP_sample_53", "NP_sample_54",
                          "NP_sample_55", "NP_sample_56", "NP_sample_57", "NP_sample_58", "NP_sample_59", "NP_sample_6", "NP_sample_60",
                          "NP_sample_61", "NP_sample_62", "NP_sample_63", "NP_sample_64", "NP_sample_65", "NP_sample_66", "NP_sample_7",
                          "NP_sample_8", "NP_sample_9",
                          "QC1", "QC10", "QC11", "QC12", "QC13", "QC14", "QC15", "QC16", "QC2", "QC3",
                          "QC4", "QC5", "QC6", "QC7", "QC8", "QC9")
    End Sub

    Public Sub LoadSampleId(ParamArray idset As String())
        For Each id As String In idset
            Call ListBox1.Items.Add(id)
        Next
    End Sub
End Class