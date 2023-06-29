Imports BioNovoGene.Analytical.MassSpectrometry.Math.MoleculeNetworking.PoolData
Imports Microsoft.VisualBasic.My.JavaScript

Public Class InputSelectGraphModel

    Const default_resource As String = "http://novocell.mzkit.org:83/taxonomy"

    Public ReadOnly Property GetModel As SpectrumGraphModel
        Get
            Return ComboBox1.SelectedItem
        End Get
    End Property

    Private Sub InputSelectGraphModel_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not CheckCloudConnection($"{default_resource}/get_models/") Then
            MessageBox.Show("The default cloud resource is not avaiable at now, it may be move to a new network location, you could specific the new cloud service location and then try again.", "Cloud Services not avaiable", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        Else
            TextBox1.Text = default_resource
        End If

        Call loadModelList()
    End Sub

    Private Sub loadModelList()
        Dim list As Object() = Restful.ParseJSON($"{TextBox1.Text}/get_models/".GET).info
        Dim models = list _
            .Select(Function(js)
                        Return DirectCast(js, JavaScriptObject).CreateObject(Of SpectrumGraphModel)
                    End Function) _
            .ToArray

        Call ComboBox1.Items.Clear()

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