Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON

Module ReadExports

    ' CompoundName	ExperimentType	Compound Type	ChemicalFormula	Category	CAS	Ionization	ResponseThreshold	Internal Standard	Internal Standard Concentration	PrecursorMass	ExtractedMass	Adduct	Polarity	ChargeState	RT	Window	CollisionEnergy	Lens	EnergyRamp	Confirm Precursor	Confirm Extracted	Confirm Energy	Target Ratio	Window Type	Ratio Window	Ion Coelution	Confirm Precursor	Confirm Extracted	Confirm Energy	Target Ratio	Window Type	Ratio Window	Ion Coelution	Confirm Precursor	Confirm Extracted	Confirm Energy	Target Ratio	Window Type	Ratio Window	Ion Coelution	Confirm Precursor	Confirm Extracted	Confirm Energy	Target Ratio	Window Type	Ratio Window	Ion Coelution	Confirm Precursor	Confirm Extracted	Confirm Energy	Target Ratio	Window Type	Ratio Window	Ion Coelution	Confirm Precursor	Confirm Extracted	Confirm Energy	Target Ratio	Window Type	Ratio Window	Ion Coelution	Confirm Precursor	Confirm Extracted	Confirm Energy	Target Ratio	Window Type	Ratio Window	Ion Coelution	Confirm Precursor	Confirm Extracted	Confirm Energy	Target Ratio	Window Type	Ratio Window	Ion Coelution	Confirm Precursor	Confirm Extracted	Confirm Energy	Target Ratio	Window Type	Ratio Window	Ion Coelution	Confirm Precursor	Confirm Extracted	Confirm Energy	Target Ratio	Window Type	Ratio Window	Ion Coelution	Fragment	Fragment	Fragment	Fragment	Fragment	Fragment	Fragment	Fragment	Fragment	Fragment
    Sub Main()
        Dim table = File.Load($"{App.HOME}/../../mzVault\OTCML.csv").Skip(2).DataFrame
        Dim rows = ParseFile(table, fragmentCount:=10).ToArray
        Dim exports = rows.GroupBy(Function(r) r.meta.CAS).ToArray

        Call (exports.Keys.AsList + rows.Select(Function(r) ParseName(r.meta.CompoundName).enUS).Distinct).GetJson(indent:=True).SaveTo("./cas.json")
        Call exports.Select(Function(g) g.Select(Function(m) m.meta)).IteratesALL.ToArray.SaveTo("./meta_OTCML.csv")
        Call exports.IteratesALL.ToArray.GetXml.SaveTo("./OTCML_db.Xml")

        Pause()
    End Sub

    Private Iterator Function ParseFile(table As File, fragmentCount%) As IEnumerable(Of FileRow)
        Dim headers = table.Headers

        For Each row As RowObject In table.Skip(1)
            Dim i = row.Pointer
            Dim meta As New MetaInfoTable With {
                .CompoundName = ++i,
                .ExperimentType = ++i,
                .CompoundType = ++i,
                .ChemicalFormula = ++i,
                .Category = ++i,
                .CAS = ++i,
                .Ionization = ++i,
                .ResponseThreshold = ++i,
                .InternalStandard = ++i,
                .InternalStandardConcentration = ++i,
                .PrecursorMass = ++i,
                .ExtractedMass = ++i,
                .Adduct = ++i,
                .Polarity = ++i,
                .ChargeState = ++i,
                .RT = ++i,
                .Window = ++i,
                .CollisionEnergy = ++i,
                .Lens = ++i,
                .EnergyRamp = ++i
            }

            Dim listOfFragments As New List(Of Fragment)

            For n As Integer = 1 To fragmentCount
                listOfFragments += New Fragment With {
                    .ConfirmPrecursor = ++i,
                    .ConfirmExtracted = ++i,
                    .ConfirmEnergy = ++i,
                    .TargetRatio = ++i,
                    .WindowType = ++i,
                    .RatioWindow = ++i,
                    .IonCoelution = ++i
                }
            Next

            Dim products As Double() = i.GetLeftsAll.AsDouble

            Yield New FileRow With {
                .meta = meta,
                .Fragments = listOfFragments,
                .productMz = products
            }
        Next
    End Function

End Module

Public Class FileRow
    Public Property meta As MetaInfoTable
    Public Property Fragments As Fragment()
    ''' <summary>
    ''' 校验用
    ''' </summary>
    ''' <returns></returns>
    Public Property productMz As Double()
End Class

Public Class Fragment
    Public Property ConfirmPrecursor As Double
    Public Property ConfirmExtracted As Double
    Public Property ConfirmEnergy As Double
    Public Property TargetRatio As Double
    Public Property WindowType As String
    Public Property RatioWindow As Double
    Public Property IonCoelution As Double

End Class

Public Class MetaInfoTable

    Public Property CompoundName As String
    Public Property ExperimentType As String
    Public Property CompoundType As String
    Public Property ChemicalFormula As String
    Public Property Category As String
    Public Property CAS As String
    Public Property Ionization As String
    Public Property ResponseThreshold As Double
    Public Property InternalStandard As String
    Public Property InternalStandardConcentration As Double
    Public Property PrecursorMass As String
    Public Property ExtractedMass As Double
    Public Property Adduct As String
    Public Property Polarity As String
    Public Property ChargeState As Double
    Public Property RT As Double
    Public Property Window As Double
    Public Property CollisionEnergy As Double
    Public Property Lens As String
    Public Property EnergyRamp As String

End Class