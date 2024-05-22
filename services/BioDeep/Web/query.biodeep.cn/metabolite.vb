#Region "Microsoft.VisualBasic::5f8e36af08462d2aee4ad8acf4533308, mzkit\services\BioDeep\Web\query.biodeep.cn\metabolite.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 47
    '    Code Lines: 40 (85.11%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (14.89%)
    '     File Size: 1.48 KB


    '     Class metabolite
    ' 
    '         Properties: biodeep_id, description, exact_mass, formula, inchi
    '                     iupac_name, name, ncbi_taxonomy, reactions, secondary_id
    '                     smiles, synonyms, xrefs, zh_names
    ' 
    '     Class metabolite_result
    ' 
    '         Properties: id, metabolite
    ' 
    '     Class ncbi_taxonomy
    ' 
    '         Properties: doi, ncbi_taxid, tax_name
    ' 
    '     Class reaction
    ' 
    '         Properties: equation, name, reaction_id
    ' 
    '     Class xrefs
    ' 
    '         Properties: source, xref_id
    ' 
    '     Class inchi
    ' 
    '         Properties: inchi, inchikey
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace query.biodeep.cn

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
End Namespace
