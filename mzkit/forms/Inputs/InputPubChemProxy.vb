Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemistry.NCBI.PubChem
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm

Public Class InputPubChemProxy

    Dim cids As New Dictionary(Of String, MetaLib)
    Dim target As String
    Dim source_biodeep As Boolean = False

    Public ReadOnly Property GetAnnotation As MetaLib
        Get
            Return cids(target)
        End Get
    End Property

    Public ReadOnly Property GetTolerance As Tolerance
        Get
            If RadioButton1.Checked Then
                Return Tolerance.PPM(Val(txtPPM.Text))
            Else
                Return Tolerance.DeltaMass(Val(txtDa.Text))
            End If
        End Get
    End Property

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
            Call ListView1.Items.Clear()
            Call ProgressSpinner _
                .DoLoading(Sub()
                               Call Me.Invoke(
                                   Sub()
                                       source_biodeep = False
                                       Call doSearch(Strings.Trim(TextBox1.Text))
                                   End Sub)
                           End Sub)
        End If
    End Sub

    ''' <summary>
    ''' do search of pubchem
    ''' </summary>
    ''' <param name="text"></param>
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

        Call api.Clear404URLIndex()

        For Each id As String In cids.SafeQuery
            Dim compound = api.Query(Of PugViewRecord)(id)
            Dim metadata = compound.GetMetaInfo

            If metadata Is Nothing Then
                Continue For
            End If

            Call Me.cids.Add(id, metadata)

            Dim cid = ListView1.Items.Add(id)

            cid.SubItems.Add(metadata.name)
            cid.SubItems.Add(metadata.formula)
            cid.SubItems.Add(metadata.exact_mass)
            cid.SubItems.Add(metadata.xref.CAS.SafeQuery.Distinct.JoinBy("; "))

            Call Application.DoEvents()
        Next
    End Sub

    Private Sub doSearchBioDeep(q As String)
        Dim result = Global.BioDeep.query.biodeep.cn.Query.search(q)

        If result Is Nothing OrElse result.data.IsNullOrEmpty Then
            MessageBox.Show("Sorry, no result was matched.", "No data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        For Each hit In result.data
            Dim metadata As New MetaLib With {
                .ID = hit.biodeep_id,
                .name = hit.name,
                .formula = hit.formula,
                .description = hit.description,
                .exact_mass = hit.exact_mass,
                .IUPACName = hit.iupac_name,
                .xref = New xref
            }

            Call Me.cids.Add(metadata.ID, metadata)

            Dim cid = ListView1.Items.Add(metadata.ID)

            cid.SubItems.Add(metadata.name)
            cid.SubItems.Add(metadata.formula)
            cid.SubItems.Add(metadata.exact_mass)
            cid.SubItems.Add(metadata.xref.CAS.SafeQuery.Distinct.JoinBy("; "))

            Call Application.DoEvents()
        Next
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        Call setSelect()
    End Sub

    Private Sub setSelect()
        For Each i As ListViewItem In ListView1.Items
            i.Checked = False
        Next

        For Each i As ListViewItem In ListView1.SelectedItems
            target = i.Text
            i.Checked = True
            Exit For
        Next

        If Not target.StringEmpty Then
            Label2.Text = $"Select [{GetAnnotation.name}]"

            If Not source_biodeep Then
                Call New Thread(Sub()
                                    Dim img = ImageFly.GetImage(target, size:="300,300", doBgTransparent:=False)

                                    Try
                                        Call Me.Invoke(Sub() PictureBox1.BackgroundImage = img)
                                    Catch ex As Exception
                                    End Try
                                End Sub) _
                     .Start()
            End If
        End If
    End Sub

    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
        If ListView1.SelectedIndices.Count = 0 Then
        Else
            setSelect()
            DialogResult = DialogResult.OK
            Close()
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If target.StringEmpty Then
            MessageBox.Show("No metabolite is selected!", "Select Metabolite", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            DialogResult = DialogResult.OK
        End If
    End Sub

    ''' <summary>
    ''' do biodeep search
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Strings.Trim(TextBox1.Text).StringEmpty Then
            Call MessageBox.Show("No query text input!", "BioDeep Query", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Call cids.Clear()
            Call ListView1.Items.Clear()

            Call ConnectToBioDeep.OpenAdvancedFunction(AddressOf doSearchBioDeep)
        End If
    End Sub

    Private Sub doSearchBioDeep()
        Call ProgressSpinner _
            .DoLoading(Sub()
                           Call Me.Invoke(
                               Sub()
                                   source_biodeep = True
                                   Call doSearchBioDeep(Strings.Trim(TextBox1.Text))
                               End Sub)
                       End Sub)
    End Sub

    Public ReadOnly Property IonMode As IonModes
        Get
            If CheckBox1.Checked Then
                Return IonModes.Positive
            Else
                Return IonModes.Negative
            End If
        End Get
    End Property

    Public Sub SetIonMassFilter()
        GroupBox3.Visible = True
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        CheckBox2.Checked = Not CheckBox1.Checked
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        CheckBox1.Checked = Not CheckBox2.Checked
    End Sub
End Class