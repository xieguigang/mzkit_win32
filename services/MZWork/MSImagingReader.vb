Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports HDF.PInvoke
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Language
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
        Dim mzArray = ReadData.Read_dataset(fileId, "/mzArray").GetSingles.ToArray
        Dim xLocation = ReadData.Read_dataset(fileId, "/xLocation").GetDoubles.ToArray
        Dim yLocation = ReadData.Read_dataset(fileId, "/yLocation").GetDoubles.ToArray
        Dim Data = ReadData.Read_dataset(fileId, "/Data").GetDoubles().ToArray
        Dim split_size As Integer = xLocation.Length
        Dim intos = Data.Split(split_size).ToArray

    End Function
End Module
