#Region "Microsoft.VisualBasic::2f9698946a2ec7922e4ac983e84193e9, G:/mzkit/src/mzkit/services/BioDeep//Web/query.biodeep.cn/Query.vb"

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

    '   Total Lines: 84
    '    Code Lines: 53
    ' Comment Lines: 17
    '   Blank Lines: 14
    '     File Size: 3.25 KB


    '     Class Query
    ' 
    '         Properties: count, data, host, page, page_size
    '                     q
    ' 
    '         Function: mapping_terms, metabolite, metabolite_page, search
    ' 
    '     Class MetaboliteSearchHit
    ' 
    '         Properties: biodeep_id, exact_mass, formula, hits, iupac_name
    '                     matches, name, score, SMILES
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Flute.Http.AppEngine
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace query.biodeep.cn

    ''' <summary>
    ''' api handler for ``query.biodeep.cn``
    ''' </summary>
    Public Class Query

        Public Property q As String
        Public Property page As Integer
        Public Property page_size As Integer
        Public Property count As Integer
        Public Property data As MetaboliteSearchHit()

        ''' <summary>
        ''' the host name for the website, config this property for run debug
        ''' </summary>
        ''' <returns></returns>
        Public Shared Property host As String = "https://query.biodeep.cn"

        Public Shared Function search(q As String, Optional page As Integer = 1, Optional page_size As Integer = 100) As Query
            Dim url As String = $"{host}/search/?q={q.UrlEncode}&page={page}&page_size={page_size}"
            Dim json_str As String = url.GET
            Dim result = json_str.LoadJSON(Of JsonResponse(Of Query))(throwEx:=False)

            If result IsNot Nothing AndAlso result.code = 0 Then
                Return result.info
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' get metabolite information by given biodeep id
        ''' </summary>
        ''' <param name="biodeep_id"></param>
        ''' <returns></returns>
        Public Shared Function metabolite(biodeep_id As String) As metabolite
            Dim url As String = $"{host}/query/metabolite/?id={biodeep_id}"
            Dim json_str As String = url.GET
            Dim result = json_str.LoadJSON(Of JsonResponse(Of metabolite_result))(throwEx:=False)

            If result IsNot Nothing AndAlso result.code = 0 Then
                Return result.info.metabolite
            Else
                Return Nothing
            End If
        End Function

        Public Shared Function mapping_terms(mapping As IEnumerable(Of String)) As UInteger()
            Dim mapping_term As String = mapping.Select(Function(s) s.UrlEncode).JoinBy("+")
            Dim url As String = $"{host}/query/mapping?fetch_id=true&list={mapping_term}"
            Dim json As String = url.GET
            Dim biodeep_id = json.LoadJSON(Of JsonResponse(Of page_data(Of UInteger)))
            Return biodeep_id.info.data
        End Function

        ''' <summary>
        ''' get the url of page for display the metabolite information.
        ''' </summary>
        ''' <param name="biodeep_id"></param>
        ''' <returns></returns>
        Public Shared Function metabolite_page(biodeep_id As String) As String
            Return $"{host}/metabolite/{biodeep_id}"
        End Function

    End Class

    Public Class MetaboliteSearchHit

        Public Property biodeep_id As String
        Public Property name As String
        Public Property formula As String
        Public Property exact_mass As Double
        Public Property iupac_name As String
        Public Property score As Double
        Public Property matches As String
        Public Property hits As String
        Public Property SMILES As String

    End Class
End Namespace
