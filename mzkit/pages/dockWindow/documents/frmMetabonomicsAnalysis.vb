Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.My.JavaScript
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Class frmMetabonomicsAnalysis

    Dim sampleinfo As SampleInfo()
    Dim properties As String()
    Dim peaks As PeakSet
    Dim metadata As New Dictionary(Of String, JavaScriptObject)

    Public Sub LoadData(table As DataTable)
        Dim groups = sampleinfo.GroupBy(Function(s) s.sample_info) _
            .Select(Function(g) (g.Key, list:=g.ToArray)) _
            .ToArray

        Call table.Columns.Add("xcms_id", GetType(String))

        For Each group In groups
            Call table.Columns.Add(group.Key, GetType(Double))
        Next

        For Each peak As xcms2 In peaks.peaks
            Dim row As Object() = New Object(groups.Length) {}
            row(0) = peak.ID

            For i As Integer = 0 To groups.Length - 1
                Dim group = groups(i).list
                Dim data As Double() = peak(group.Select(Function(s) s.ID))
                row(i + 1) = data.Average
            Next

            table.Rows.Add(row)
        Next
    End Sub

    Public Sub LoadData(table As File, title As String)
        Dim wizard As New InputImportsPeaktableDialog
        Dim df As DataFrame = DataFrame.CreateObject(table)

        Call wizard.LoadSampleId(df.HeadTitles)
        Call InputDialog.Input(
            Sub(config)
                sampleinfo = config.GetSampleInfo.ToArray
                properties = config.GetMetadata.ToArray
            End Sub, Nothing, wizard)

        ' show data
        Dim peaks As New List(Of xcms2)
        Dim peak As xcms2

        For Each row In df.EnumerateData
            peak = New xcms2 With {
                .ID = row.TryPopOut({"id", "ID", "xcms_id"}),
                .mz = row.TryPopOut({"mz", "m/z", "MZ"}),
                .mzmax = row.TryPopOut({"mzmax", "mz.max"}),
                .mzmin = row.TryPopOut({"mzmin", "mz.min"}),
                .rt = row.TryPopOut({"rt", "RT", "retention_time"}),
                .rtmax = row.TryPopOut({"rtmax", "rt.max"}),
                .rtmin = row.TryPopOut({"rtmin", "rt.min"}),
                .Properties = New Dictionary(Of String, Double)
            }

            For Each id In sampleinfo
                peak(id.ID) = row.TryPopOut(id.ID)
            Next

            Dim meta As New JavaScriptObject

            For Each item In row
                meta(item.Key) = item.Value
            Next

            peaks.Add(peak)
            metadata(peak.ID) = meta
        Next

        Me.peaks = New PeakSet With {
            .peaks = peaks.ToArray
        }

        Call loadTable()
    End Sub

    Dim memoryData As System.Data.DataSet

    Private Sub loadTable()
        memoryData = New System.Data.DataSet

        Dim table As DataTable = memoryData.Tables.Add("memoryData")

        Try
            Call Me.AdvancedDataGridView1.Columns.Clear()
            Call Me.AdvancedDataGridView1.Rows.Clear()
        Catch ex As Exception

        End Try

        Call LoadData(table)
        Call AdvancedDataGridView1.SetDoubleBuffered()

        'For Each column As DataGridViewColumn In AdvancedDataGridView1.Columns
        '    'Select Case table.Columns.Item(column.HeaderText).DataType
        '    '    Case GetType(String)
        '    '        AdvancedDataGridView1.SetSortEnabled(column, True)
        '    '    Case GetType(Double)
        '    '    Case GetType(Integer)
        '    '    Case Else
        '    '        ' do nothing 
        '    'End Select

        '    AdvancedDataGridView1.ShowMenuStrip(column)
        'Next

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName

        AdvancedDataGridView1.DataSource = BindingSource1
        AdvancedDataGridViewSearchToolBar1.SetColumns(AdvancedDataGridView1.Columns)
    End Sub

End Class