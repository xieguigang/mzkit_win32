Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemistry.NCBI.PubChem
Imports Microsoft.VisualBasic.Linq

Public Class InputPubChemProxy

    ReadOnly cids As New Dictionary(Of String, MetaLib)

    ''' <summary>
    ''' do pubchem search
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Strings.Trim(TextBox1.Text).StringEmpty Then
            Call MessageBox.Show("No query text input!", "PubChem Query", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Call cids.Clear()
            Call doSearch(Strings.Trim(TextBox1.Text))
        End If
    End Sub

    Private Sub doSearch(text As String)
        ' text to cid
        ' then query by cid
        Dim cids = Query.QueryCID(
            name:=text,
            cacheFolder:=$"{Globals.pubchemWebCache}/.cids/",
            offlineMode:=False,
            hitCache:=Nothing,
            interval:=0
        )
        Dim api As WebQuery = $"{Globals.pubchemWebCache}/pugViews/".GetQueryHandler(Of WebQuery)(offline:=False)

        For Each id As String In cids.SafeQuery
            Dim compound = api.Query(Of PugViewRecord)(id)
            Dim metadata = compound.GetMetaInfo

            Call Me.cids.Add(id, metadata)
        Next
    End Sub
End Class