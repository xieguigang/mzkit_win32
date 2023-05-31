Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Language

Public Module MSImagingReader

    ''' <summary>
    ''' This method provides a unify method for read raw data as mzpack for ms-maging
    ''' </summary>
    ''' <param name="filepath"></param>
    ''' <returns></returns>
    Public Function UnifyReadAsMzPack(filepath As String) As [Variant](Of mzPack, ReadRawPack)
        If filepath.ExtensionSuffix("mzpack") Then
            Return mzPack.ReadAll(filepath.Open(FileMode.Open, doClear:=False, [readOnly]:=True), ignoreThumbnail:=True)
        End If
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
    End Function
End Module
