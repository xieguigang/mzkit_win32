Imports BioNovoGene.mzkit_win32.Configuration
Imports Microsoft.VisualBasic.Imaging
Imports Mzkit_win32.MSImagingViewerV2.PolygonEditor

Public Class InputConfigTissueMap

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        saveSettings()
        DialogResult = DialogResult.OK
    End Sub

    Private Sub InputConfigTissueMap_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call loadSettings()
    End Sub

    Private Sub saveSettings()
        Dim configs = GetTissueMapViewerConfig()

        configs.region_prefix = txtPrefix.Text
        configs.opacity = numOpacity.Value
        configs.spot_size = Val(txtSpotSize.Text)
        configs.color_scaler = cbColorSet.SelectedItem.ToString

        Dim editorConfigs = configs.editor

        editorConfigs.point_size = numPointSize.Value
        editorConfigs.line_width = numLineWidth.Value
        editorConfigs.point_color = pointColor.BackColor.ToHtmlColor
        editorConfigs.line_color = lineColor.BackColor.ToHtmlColor
        editorConfigs.show_points = ckShowPoints.Checked
        editorConfigs.dash = ckDashLine.Checked

        configs.bootstrapping = New SampleBootstrapping With {
            .nsamples = Val(TextBox1.Text),
            .coverage = Val(TextBox2.Text)
        }

        Globals.Settings.Save()
    End Sub

    Private Sub loadSettings()
        Dim configs = GetTissueMapViewerConfig()

        txtPrefix.Text = configs.region_prefix
        numOpacity.Value = configs.opacity
        txtSpotSize.Text = configs.spot_size

        cbColorSet.SelectedIndex = 0

        For i As Integer = 0 To cbColorSet.Items.Count - 1
            If cbColorSet.Items(i) = configs.color_scaler Then
                cbColorSet.SelectedIndex = i
                Exit For
            End If
        Next

        Dim editorConfigs = configs.editor

        numPointSize.Value = editorConfigs.point_size
        numLineWidth.Value = editorConfigs.line_width
        pointColor.BackColor = editorConfigs.point_color.TranslateColor
        lineColor.BackColor = editorConfigs.line_color.TranslateColor
        ckShowPoints.Checked = editorConfigs.show_points
        ckDashLine.Checked = editorConfigs.dash

        Dim boot = Globals.MSIBootstrapping

        TextBox1.Text = boot.nsamples
        TextBox2.Text = boot.coverage
    End Sub

    Public Shared Function GetTissueMapViewerConfig() As TissueMap
        Dim configs = Globals.Settings.tissue_map

        If configs Is Nothing Then
            configs = TissueMap.GetDefault
            Globals.Settings.tissue_map = configs
            Globals.Settings.Save()
        End If

        Return configs
    End Function

    Public Shared Function GetPolygonEditorConfig() As PolygonEditorConfigs
        Dim configs = GetTissueMapViewerConfig()

        If configs.editor Is Nothing Then
            configs.editor = PolygonEditorConfigs.GetDefault
            Globals.Settings.tissue_map = configs
            Globals.Settings.Save()
        End If

        Return configs.editor
    End Function

    Private Sub pointColor_Click(sender As Object, e As EventArgs) Handles pointColor.Click
        Using colors As New ColorDialog With {
            .Color = pointColor.BackColor
        }
            If colors.ShowDialog = DialogResult.OK Then
                pointColor.BackColor = colors.Color
            End If
        End Using
    End Sub

    Private Sub lineColor_Click(sender As Object, e As EventArgs) Handles lineColor.Click
        Using colors As New ColorDialog With {
            .Color = lineColor.BackColor
        }
            If colors.ShowDialog = DialogResult.OK Then
                lineColor.BackColor = colors.Color
            End If
        End Using
    End Sub
End Class