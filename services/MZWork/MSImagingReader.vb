Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports HDF.PInvoke
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports STRaid

Public Module MSImagingReader

    ''' <summary>
    ''' This method provides a unify method for read raw data as mzpack for ms-maging
    ''' </summary>
    ''' <param name="filepath"></param>
    ''' <returns></returns>
    Public Function UnifyReadAsMzPack(filepath As String) As [Variant](Of mzPack, ReadRawPack)
        If filepath.ExtensionSuffix("cdf") Then
            ' read multiple ion layers
            Using cdf As New netCDFReader(filepath)
                Return cdf.CreatePixelReader
            End Using
        End If
        If filepath.ExtensionSuffix("imzml") Then
            Dim mzPack = Converter.LoadimzML(filepath, AddressOf RunSlavePipeline.SendProgress)
            mzPack.source = filepath.FileName
            Return mzPack
        End If
        If filepath.ExtensionSuffix("h5") Then
            Return ReadmsiPLData(filepath)
        End If

        ' try to open all other kind of data files as mzpack
        Return mzPack.ReadAll(filepath.Open(FileMode.Open, doClear:=False, [readOnly]:=True), ignoreThumbnail:=True)
    End Function

    ''' <summary>
    ''' read msiPLData dataset as MSimaging mzpack data object
    ''' </summary>
    ''' <param name="h5"></param>
    ''' <returns></returns>
    Public Function ReadmsiPLData(h5 As String) As mzPack
        Dim fileId As Long = H5F.open(h5, H5F.ACC_RDONLY)
        Dim mzArray = ReadData.Read_dataset(fileId, "/mzArray") _
            .GetSingles _
            .Select(Function(s) CDbl(s)) _
            .ToArray
        Dim xLocation = ReadData.Read_dataset(fileId, "/xLocation").GetDoubles.ToArray
        Dim yLocation = ReadData.Read_dataset(fileId, "/yLocation").GetDoubles.ToArray
        Dim Data = ReadData.Read_dataset(fileId, "/Data").GetDoubles().ToArray
        Dim split_size As Integer = mzArray.Length
        Dim intos = Data.Split(split_size).ToArray
        Dim ms1 As New List(Of ScanMS1)

        For i As Integer = 0 To intos.Length - 1
            Dim v As Double() = intos(i)
            Dim x As Integer = xLocation(i)
            Dim y As Integer = yLocation(i)
            Dim ions As Integer = which(v.AsVector > 0).Count

            Call ms1.Add(New ScanMS1 With {
                .BPC = v.Max,
                .into = v,
                .meta = New Dictionary(Of String, String) From {
                    {"x", x}, {"y", y}
                },
                .mz = mzArray,
                .products = {},
                .rt = i,
                .TIC = v.Sum,
                .scan_id = $"[MS1][{i + 1}][{x},{y}] {ions} ions, BPC={ .BPC.ToString("G3")}, total_ions={ .TIC.ToString("G3")}, basepeak_m/z={mzArray(which.Max(v))}"
            })
        Next

        Return New mzPack With {
            .Application = FileApplicationClass.MSImaging,
            .metadata = New Dictionary(Of String, String),
            .source = h5.FileName,
            .MS = ms1.ToArray
        }
    End Function
End Module
