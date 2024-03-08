Public Class metabolite

    Public Property biodeep_id As String
    Public Property name As String
    Public Property formula As String
    Public Property exact_mass As Double
    Public Property iupac_name As String
    Public Property description As String
    Public Property smiles As String()
    Public Property inchi As inchi()
    Public Property xrefs As xrefs()
    Public Property reactions As reaction()
    Public Property zh_names As String()
    Public Property synonyms As String()
    Public Property secondary_id As String()
    Public Property ncbi_taxonomy As ncbi_taxonomy()
End Class

Public Class metabolite_result
    Public Property id As String
    Public Property metabolite As metabolite
End Class

Public Class ncbi_taxonomy
    Public Property ncbi_taxid As String
    Public Property tax_name As String
    Public Property doi As String
End Class

Public Class reaction
    Public Property name As String
    Public Property equation As String
    Public Property reaction_id As String
End Class

Public Class xrefs
    Public Property source As String
    Public Property xref_id As String
End Class

Public Class inchi
    Public Property inchi As String
    Public Property inchikey As String
End Class