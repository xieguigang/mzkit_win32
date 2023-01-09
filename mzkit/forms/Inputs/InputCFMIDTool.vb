Imports Task.Container

Public Class InputCFMIDTool

    Friend cfmid_folder As String


    Public ReadOnly Property struct As String
        Get
            Return TextBox1.Text
        End Get
    End Property
    Public ReadOnly Property param_config As String
        Get
            Return TextBox2.Text
        End Get
    End Property
    Public ReadOnly Property model As String
        Get
            Return TextBox3.Text
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text.StringEmpty Then
            MessageBox.Show("No structure text!", "CFM-ID Tool", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        ElseIf TextBox2.Text.StringEmpty Then
            MessageBox.Show("No param_config!", "CFM-ID Tool", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        ElseIf TextBox3.Text.StringEmpty Then
            MessageBox.Show("No cfm-id model file!", "CFM-ID Tool", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub InputCFMIDTool_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If AppEnvironment.IsDevelopmentMode Then
            cfmid_folder = $"{App.HOME}/../../src/mzkit/dist/cfm-id2/".GetDirectoryFullPath
        Else
            cfmid_folder = $"{App.HOME}/tools/cfm-id2/".GetDirectoryFullPath
        End If

        Dim ms2Folder As String = $"{cfmid_folder}/models/esi_msms_models/"
        Dim dirs As String() = ms2Folder.ListDirectory.Select(Function(d) d.BaseName).ToArray

        For Each name As String In dirs
            Call ListBox1.Items.Add(name)
        Next
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked Then
            ListBox1.Enabled = True
            ListBox2.Enabled = True

            Call ListBox1_SelectedIndexChanged(Nothing, Nothing)
        Else
            ListBox1.Enabled = False
            ListBox2.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' ei_ms_models
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked Then
            ListBox1.Enabled = False
            ListBox2.Enabled = False

            TextBox2.Text = $"{cfmid_folder}/models/ei_ms_models/ei_nn_iso_new/param_config.txt"
            TextBox3.Text = $"{cfmid_folder}/models/ei_ms_models/ei_nn_iso_new/model-2.txt"
        Else
            ListBox1.Enabled = True
            ListBox2.Enabled = True
        End If
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If ListBox1.Items.Count = 0 Then
            Return
        End If
        If ListBox1.SelectedIndex = -1 Then
            ListBox1.SelectedIndex = 0
        End If

        Dim folderName As String = ListBox1.SelectedItem.ToString
        Dim path As String = $"{cfmid_folder}/models/esi_msms_models/{folderName}"
        Dim modelNames As String() = path.ListFiles("model*.txt").ToArray

        ListBox2.Items.Clear()

        For Each file As String In modelNames
            ListBox2.Items.Add(file.FileName)
        Next

        ListBox2.SelectedIndex = 0
        TextBox2.Text = $"{path}/param_config.txt"

        Call ListBox2_SelectedIndexChanged(Nothing, Nothing)
    End Sub

    Private Sub ListBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox2.SelectedIndexChanged
        If ListBox2.Items.Count = 0 Then
            Return
        End If

        Dim folderName As String = ListBox1.SelectedItem.ToString
        Dim path As String = $"{cfmid_folder}/models/esi_msms_models/{folderName}"
        Dim modelfile As String = $"{path}/{ListBox2.SelectedItem}"

        TextBox3.Text = modelfile
    End Sub
End Class