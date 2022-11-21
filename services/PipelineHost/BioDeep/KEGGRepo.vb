#Region "Microsoft.VisualBasic::08566adaa29ccccb4611eb4bd31ad1a4, mzkit\src\mzkit\Task\BioDeep\KEGGRepo.vb"

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

'   Total Lines: 105
'    Code Lines: 85
' Comment Lines: 0
'   Blank Lines: 20
'     File Size: 4.07 KB


' Module KEGGRepo
' 
'     Function: RequestKEGGcompounds, RequestKEGGCompounds, RequestKEGGMaps, RequestKEGGReactions, RequestLipidMaps
'               unzip
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports BioDeep
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.MetaDNA
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.My
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Data.KEGG.Metabolism

Public Module KEGGRepo

    <Extension>
    Private Function unzip(file As Stream) As Stream
        Using zip As New ZipArchive(file, ZipArchiveMode.Read)
            Dim repoEntry As ZipArchiveEntry = zip.Entries.First

            Using repofile As Stream = repoEntry.Open
                Dim buffer As New MemoryStream

                Call repofile.CopyTo(buffer)
                Call buffer.Seek(0, SeekOrigin.Begin)

                Return buffer
            End Using
        End Using
    End Function

    Public Function RequestKEGGcompounds(println As Action(Of String)) As Compound()
        Const url As String = "http://query.biodeep.cn/kegg/repository/compounds"

        Call println("request KEGG compounds from BioDeep...")

        Return SingletonHolder(Of BioDeepSession).Instance _
            .RequestStream(url) _
            .unzip _
            .DoCall(AddressOf KEGGCompoundPack.ReadKeggDb)
    End Function

    Public Function RequestKEGGReactions(println As Action(Of String)) As ReactionClass()
        Const url As String = "http://query.biodeep.cn/kegg/repository/reactions"

        Call println("request KEGG reaction network from BioDeep...")

        Return SingletonHolder(Of BioDeepSession).Instance _
            .RequestStream(url) _
            .unzip _
            .DoCall(AddressOf ReactionClassPack.ReadKeggDb)
    End Function

    Public Function RequestKEGGCompounds() As Compound()
        Using zip As New ZipArchive(getMZKitPackage.Open(FileMode.Open, doClear:=False))
            Using pack = If(zip.GetEntry("data\KEGG_compounds.msgpack"), zip.GetEntry("data/KEGG_compounds.msgpack")).Open
                Return KEGGCompoundPack.ReadKeggDb(pack)
            End Using
        End Using
    End Function

    Public Function loadBackground(Optional ByRef maps As Map() = Nothing) As Background
        Dim background As Background
        maps = KEGGRepo.RequestKEGGMaps
        background = MSJointConnection.ImportsBackground(maps)
        Return background
    End Function

    Public Function RequestKEGGMaps() As Map()
        Using zip As New ZipArchive(getMZKitPackage.Open(FileMode.Open, doClear:=False))
            Using pack = If(zip.GetEntry("data\KEGG_maps.msgpack"), zip.GetEntry("data/KEGG_maps.msgpack")).Open
                Return KEGGMapPack.ReadKeggDb(pack)
            End Using
        End Using
    End Function

    Public Function RequestKeggReactionNetwork() As Dictionary(Of String, Reaction)
        Using zip As New ZipArchive(getMZKitPackage(pkg:="GCModeller").Open(FileMode.Open, doClear:=False))
            Using pack As Stream = If(zip.GetEntry("data\kegg\reactions.zip"), zip.GetEntry("data/kegg/reactions.zip")).Open
                Using innerZip As New ZipArchive(pack, ZipArchiveMode.Read)
                    Using innerPack As Stream = innerZip.GetEntry("reactions.msgpack").Open
                        Return KEGGReactionPack.ReadKeggDb(innerPack) _
                            .GroupBy(Function(r) r.ID) _
                            .ToDictionary(Function(r) r.Key,
                                          Function(r)
                                              Return r.First
                                          End Function)
                    End Using
                End Using
            End Using
        End Using
    End Function

    Private Function getMZKitPackage(Optional pkg As String = "mzkit") As String
        Dim filepath As String = pkg

        For Each dirLevel As String In {"", "../", "../../", "../../../", "../../../../"}
            filepath = $"{App.HOME}/{dirLevel}Rstudio/packages/{pkg}.zip"

            If filepath.FileExists Then
                Return filepath
            End If
        Next

        If Not filepath.FileExists Then
            Throw New FileNotFoundException(filepath)
        Else
            Return filepath
        End If
    End Function

    Public Function RequestMetabolights() As MetaboliteAnnotation()
        Using zip As New ZipArchive(getMZKitPackage.Open(FileMode.Open, doClear:=False))
            Using pack = If(zip.GetEntry("data\MetaboLights.csv"), zip.GetEntry("data/MetaboLights.csv")).Open
                Dim packData As DataFrame = DataFrame.Load(pack)
                Dim id As String() = packData("id")
                Dim name As String() = packData("name")
                Dim formula As String() = packData("formula")

                Return id _
                    .Select(Function(ref, i)
                                Return New MetaboliteAnnotation With {
                                    .UniqueId = ref,
                                    .CommonName = name(i),
                                    .Formula = formula(i),
                                    .ExactMass = FormulaScanner.EvaluateExactMass(.Formula)
                                }
                            End Function) _
                    .ToArray
            End Using
        End Using
    End Function

    Public Function RequestChebi() As MetaboliteAnnotation()
        Dim filepath As String = ""

        For Each dirLevel As String In {"", "../", "../../", "../../../", "../../../../"}
            filepath = $"{App.HOME}/{dirLevel}Rstudio/data/chebi_lite.obo"

            If filepath.FileExists Then
                Exit For
            End If

            filepath = $"{App.HOME}/{dirLevel}src/mzkit/rstudio/data/chebi_lite.obo"

            If filepath.FileExists Then
                Exit For
            End If
        Next

        If Not filepath.FileExists Then
            Return {}
        End If

        Dim obo As GO_OBO = GO_OBO.LoadDocument(filepath)
        Dim metabolites = obo.terms _
            .Where(Function(t) Not t.property_value.IsNullOrEmpty) _
            .Where(Function(t) t.property_value.Any(Function(p) p.StartsWith("formula"))) _
            .Select(Function(t)
                        Return New MetaboliteAnnotation With {
                            .CommonName = t.name,
                            .UniqueId = t.id,
                            .Formula = t.property_value _
                                .Where(Function(p) p.StartsWith("formula")) _
                                .First _
                                .Split(" "c)(1) _
                                .Trim(""""c),
                            .ExactMass = FormulaScanner.ScanFormula(.Formula).ExactMass
                        }
                    End Function) _
            .ToArray

        Return metabolites
    End Function

    Public Function RequestLipidMaps() As LipidMaps.MetaData()
        Using zip As New ZipArchive(getMZKitPackage.Open(FileMode.Open, doClear:=False))
            Using pack = If(zip.GetEntry("data\LIPIDMAPS.msgpack"), zip.GetEntry("data/LIPIDMAPS.msgpack")).Open
                Return LipidMaps.ReadRepository(pack)
            End Using
        End Using
    End Function
End Module
