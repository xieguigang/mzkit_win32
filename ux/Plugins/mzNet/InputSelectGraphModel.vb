Imports BioNovoGene.BioDeep.MassSpectrometry.MoleculeNetworking.PoolData
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.My.JavaScript
Imports Mzkit_win32.BasicMDIForm

Public Class InputSelectGraphModel

    Const default_resource As String = "http://novocell.mzkit.org:83/taxonomy"

    Public ReadOnly Property GetModel As SpectrumGraphModel
        Get
            Return ComboBox1.SelectedItem
        End Get
    End Property

    Public ReadOnly Property GetCloudRootURL As String
        Get
            Return TextBox1.Text
        End Get
    End Property

    Private Sub InputSelectGraphModel_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not CheckCloudConnection($"{default_resource}/get_models/") Then
            MessageBox.Show("The default cloud resource is not avaiable at now, it may be move to a new network location, you could specific the new cloud service location and then try again.",
                            "Cloud Services not avaiable", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        Else
            TextBox1.Text = default_resource
        End If

        Call loadModelList()
    End Sub

    Private Sub loadModelList()
        Dim req = Restful.ParseJSON($"{TextBox1.Text}/get_models/".GET(timeoutSec:=1))

        If req.code > 0 Then
            Call MessageBox.Show(req.info, "Fetch spectrum graph model error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim list As Object() = req.info
        Dim models = list.SafeQuery _
            .Select(Function(js)
                        Return DirectCast(js, JavaScriptObject).CreateObject(Of SpectrumGraphModel)
                    End Function) _
            .ToArray

        If list Is Nothing Then
            Call MessageBox.Show("Sorry, the cloud service is unavailable!", "Server Down", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Call ComboBox1.Items.Clear()
        Call Workbench.SuccessMessage($"Load {models.Length} spectrum cluster graph models!")

        For Each graph As SpectrumGraphModel In models
            Call ComboBox1.Items.Add(graph)
        Next

        ComboBox1.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Check the cloud services is avaiable or not?
    ''' </summary>
    ''' <param name="urlCloudSvr"></param>
    ''' <returns></returns>
    Public Shared Function CheckCloudConnection(urlCloudSvr As String) As Boolean
        Try
            Dim e404 As Boolean = False

            ' try to get remote cloud service response
            Call urlCloudSvr.GET(is404:=e404, timeoutSec:=3)

            If e404 Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Call loadModelList()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedItem IsNot Nothing Then
            TextBox2.Text = DirectCast(ComboBox1.SelectedItem, SpectrumGraphModel).description
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If GetCloudRootURL.StringEmpty Then
            Return
        End If

        DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub
End Class

Public Class SpectrumGraphModel

    Public Property add_time As Date
    Public Property description As String
    Public Property flag As Integer
    Public Property id As Integer
    Public Property name As String
    Public Property parameters As String

    Public Overrides Function ToString() As String
        Return $"[{id}] {name}"
    End Function

End Class