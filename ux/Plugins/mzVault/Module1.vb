Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite
Imports Microsoft.VisualBasic.Data.IO.ManagedSqlite.Core
Imports Microsoft.VisualBasic.Linq

Module Module1

    Sub Main()
        Call traceFilder()

        Pause()
    End Sub

    Sub traceFilder()
        Dim db = Sqlite3Database.OpenFile("D:\smartnucl_integrative\biodeepDB\protocols\biodeepMSMS1\OTCML\数据库\OTCML for TraceFinder_v4.1.cdb")
        Dim CdbCompound As EntityObject() = db.GetTable(NameOf(CdbCompound)).ExportTable.SeqIterator.Select(Function(r) New EntityObject With {.ID = r.i, .Properties = r.value}).ToArray
        Dim SchemaVersion As EntityObject() = db.GetTable(NameOf(SchemaVersion)).ExportTable.SeqIterator.Select(Function(r) New EntityObject With {.ID = r.i, .Properties = r.value}).ToArray
        Dim CdbTransition As EntityObject() = db.GetTable(NameOf(CdbTransition)).ExportTable.SeqIterator.Select(Function(r) New EntityObject With {.ID = r.i, .Properties = r.value}).ToArray

        Pause()
    End Sub

    Sub OTCML()
        Dim db = Sqlite3Database.OpenFile("D:\smartnucl_integrative\biodeepDB\protocols\biodeepMSMS1\OTCML for mzVault.db")
        Dim spectrum = db.GetTable("SpectrumTable").ExportTable.SeqIterator.Select(Function(r) New EntityObject With {.ID = r.i, .Properties = r.value}).ToArray
        '  Dim compounds = db.GetTable("CompoundTable").ExportTable.SeqIterator.Select(Function(r) New EntityObject With {.ID = r.i, .Properties = r.value}).ToArray

        For Each spectra In spectrum

            Dim mz = Convert.FromBase64String(spectra("[blobMass]")).Split(8).Select(Function(bytes) BitConverter.ToDouble(bytes, 0)).ToArray
            Dim into = Convert.FromBase64String(spectra("[blobIntensity]")).Split(8).Select(Function(bytes) BitConverter.ToDouble(bytes, 0)).ToArray
            Dim flags = Convert.FromBase64String(spectra("[blobFlags]"))
            Dim toppeaks = Convert.FromBase64String(spectra("[blobTopPeaks]")).Split(8).Select(Function(bytes) BitConverter.ToDouble(bytes, 0)).ToArray

            Pause()
        Next


        'For Each table In db.GetTables
        '    Call Console.WriteLine(table.ParseSchema.GetJson)
        'Next

        'For Each c In compounds
        '    c("[Structure]") = c("[Structure]").LineTokens.JoinBy("\n")
        'Next

        ' Call compounds.SaveTo("./metadb.csv")
        Call spectrum.SaveTo("./spectrum.csv")
    End Sub
End Module
