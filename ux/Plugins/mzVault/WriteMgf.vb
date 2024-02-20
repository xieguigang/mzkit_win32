Imports System.IO
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.MassSpectrum.Assembly.ASCII.MGF
Imports SMRUCC.MassSpectrum.DATA.MetaLib.Models
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Ms1.PrecursorType
Imports SMRUCC.MassSpectrum.Math.Spectra

Public Class OTCMLClassify
    Public Property name As String
    Public Property enUS_name As String
    Public Property CAS As String
    Public Property formula As String
    Public Property mass As Double
    Public Property [class] As String
End Class

Module WriteMgf

    Public Function ParseName(name As String) As (enUS$, zhCN$)
        Dim enUs$ = name.Trim
        Dim zhCN$ = enUs.Split.Last

        If zhCN.IsPattern("[a-z]+") Then
            With enUs.Split
                zhCN = .Skip(.Length - 2).JoinBy(" ")
            End With
        End If

        enUs = enUs.Replace(zhCN, "").Trim

        Return (enUs, zhCN)
    End Function

    Sub Main()
        Dim pubchemData = "N:\software\中药库\PubChem\.cache\pugViews" _
            .ListFiles("*.Xml") _
            .Select(AddressOf LoadXml(Of PugViewRecord)) _
            .Select(AddressOf GetMetaInfo) _
            .ToArray
        Dim CASData = pubchemData _
            .Where(Function(m) Not m.xref.CAS.IsNullOrEmpty) _
            .Select(Function(m)
                        Return m.xref.CAS.Select(Function(id) (m, CAS:=id))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(m) m.CAS) _
            .ToDictionary(Function(id) id.Key,
                          Function(g)
                              Return g.Select(Function(t) t.m).GroupBy(Function(m) m.ID).Select(Function(gg) gg.First).OrderBy(Function(m) m.name.Length).First
                          End Function)
        Dim nameData = pubchemData _
            .Select(Function(m) m.synonym.Select(Function(name) (name, m)).JoinIterates({(m.name, m)})) _
            .IteratesALL _
            .GroupBy(Function(m) m.name.ToLower) _
            .ToDictionary(Function(m) m.Key,
                          Function(g)
                              Return g.First.m
                          End Function)
        Dim data = "N:\software\中药库\OTCML_db.Xml".LoadXml(Of FileRow())
        Dim info = "N:\software\中药库\meta.csv".LoadCsv(Of OTCMLClassify).ToDictionary(Function(m) m.CAS)

        Using pout As StreamWriter = "N:\software\中药库\OTCML+.mgf".OpenWriter,
              nout As StreamWriter = "N:\software\中药库\OTCML-.mgf".OpenWriter

            For Each spectrum As FileRow In data
                Dim ms2 As ms2() = spectrum.Fragments _
                    .Select(Function(f)
                                Return New ms2 With {
                                    .mz = f.ConfirmExtracted,
                                    .intensity = f.TargetRatio,
                                    .quantity = f.TargetRatio,
                                    .Annotation = f.IonCoelution
                                }
                            End Function) _
                    .OrderBy(Function(m) m.mz) _
                    .ToArray
                Dim nameParts = ParseName(spectrum.meta.CompoundName)
                Dim ion As New Ions With {
                    .PepMass = New NamedValue With {
                        .name = spectrum.meta.ExtractedMass,
                        .text = 100
                    },
                    .Charge = spectrum.meta.ChargeState,
                    .Database = "OTCML",
                    .Instrument = "QE",
                    .Locus = spectrum.meta.CAS,
                    .Rawfile = "OTCML.mgf",
                    .RtInSeconds = spectrum.meta.RT * 60,
                    .Peaks = ms2,
                    .Title = $"OTCML_{spectrum.meta.CAS}",
                    .Meta = New Dictionary(Of String, String) From {
                        {"CollisionEnergy", spectrum.meta.CollisionEnergy},
                        {"en-US", nameParts.enUS},
                        {"zh-CN", nameParts.zhCN}
                    }
                }

                Dim cas As MetaLib = Nothing

                If CASData.ContainsKey(spectrum.meta.CAS) Then
                    cas = CASData(spectrum.meta.CAS)
                ElseIf nameData.ContainsKey(nameParts.enUS.ToLower) Then
                    cas = nameData(nameParts.enUS.ToLower)
                End If

                If Not cas Is Nothing Then
                    ion.Meta("name") = cas.name
                    ion.Meta(NameOf(cas.xref.chebi)) = cas.xref.chebi
                    ion.Meta(NameOf(cas.xref.HMDB)) = cas.xref.HMDB
                    ion.Meta(NameOf(cas.xref.InChI)) = cas.xref.InChI
                    ion.Meta(NameOf(cas.xref.InChIkey)) = cas.xref.InChIkey
                    ion.Meta(NameOf(cas.xref.KEGG)) = cas.xref.KEGG
                    ion.Meta(NameOf(cas.xref.pubchem)) = cas.ID
                    ion.Meta(NameOf(cas.xref.SMILES)) = cas.xref.SMILES
                    ion.Meta(NameOf(cas.xref.Wikipedia)) = cas.xref.Wikipedia
                    ion.Meta(NameOf(cas.formula)) = cas.formula
                    ion.Meta(NameOf(cas.exact_mass)) = cas.exact_mass
                Else
                    Call spectrum.meta.CompoundName.Warning
                End If

                If info.ContainsKey(spectrum.meta.CAS) Then
                    ion.Meta("Class") = info(spectrum.meta.CAS).class
                End If

                Dim ex_mass = ion.Meta.TryGetValue("exact_mass", [default]:=ion.PepMass.name - 1)
                Dim precursor_type = PrecursorType.FindPrecursorType(ex_mass, ion.PepMass.name, ion.Charge, spectrum.meta.Polarity, Ms1.Tolerance.DeltaMass(0.5)).precursorType
                Dim libname = $"{ion.Title}_{spectrum.meta.CollisionEnergy}V_{precursor_type}"

                ion.Meta("precursor_type") = precursor_type
                ion.Meta("libname") = libname

                If spectrum.meta.Polarity = "+" Then
                    ion.Charge = Math.Abs(ion.Charge)

                    Call ion.WriteAsciiMgf(pout)
                Else
                    ion.Charge = -Math.Abs(ion.Charge)

                    Call ion.WriteAsciiMgf(nout)
                End If
            Next
        End Using

        Pause()
    End Sub
End Module
