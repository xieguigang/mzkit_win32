Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm

Namespace MSdata

    Module Module1

        ''' <summary>
        ''' get XIC Chromatogram collection
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="scanId">the scan id of the target ms2 data</param>
        ''' <param name="ppm"></param>
        ''' <param name="relativeInto"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Friend Function getXICMatrix(raw As MZWork.Raw, scanId As String, ppm As Double, relativeInto As Boolean) As NamedCollection(Of ChromatogramTick)
            Dim ms2 As ScanMS2 = raw.FindMs2Scan(scanId)
            Dim name As String = raw.source.FileName
            Dim ms1 As ScanMS1 = raw.GetMs1Scans _
                .Where(Function(scan1)
                           Return scan1.products _
                               .SafeQuery _
                               .Any(Function(a) a.scan_id = scanId)
                       End Function) _
                .FirstOrDefault

            If ms2 Is Nothing OrElse ms2.parentMz = 0.0 Then
                Workbench.Warning("XIC plot is not avaliable for MS1 parent scan!")
                Return Nothing
            Else
                Workbench.StatusMessage(name)
            End If

            Dim parentMz As Double = ms1.mz _
                .Select(Function(mzi) (PPMmethod.PPM(mzi, ms2.parentMz), mzi)) _
                .OrderBy(Function(a) a.Item1) _
                .First _
                .mzi
            Dim mzdiff As Tolerance = PPMmethod.PPM(ppm)
            Dim XIC As ChromatogramTick() = raw _
                .GetMs1Scans _
                .Select(Function(a)
                            Return New ChromatogramTick With {
                                .Time = a.rt,
                                .Intensity = a.GetIntensity(ms2.parentMz, mzdiff)
                            }
                        End Function) _
                .ToArray

            name = $"XIC [m/z={parentMz.ToString("F4")}]"

            If Not relativeInto Then
                XIC = {
                    New ChromatogramTick With {.Time = raw.rtmin},
                    New ChromatogramTick With {.Time = raw.rtmax}
                }.JoinIterates(XIC) _
                 .OrderBy(Function(c) c.Time) _
                 .ToArray
            End If

            Dim plotTIC As New NamedCollection(Of ChromatogramTick) With {
                .name = name,
                .value = XIC,
                .description = ms2.parentMz & " " & raw.source.FileName
            }

            Return plotTIC
        End Function

    End Module
End Namespace