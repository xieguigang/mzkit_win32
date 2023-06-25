Imports System.Reflection.Emit
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports Mzkit_win32.BasicMDIForm

Namespace MSdata

    Module Module1

        <Extension>
        Public Function ClusterScan(raw As PeakMs2(), scan_id As String) As ScanMS1
            Dim rt As Double() = raw.Select(Function(p) p.rt).ToArray

            Return New ScanMS1 With {
                .into = raw.Select(Function(p) p.intensity).ToArray,
                .mz = raw.Select(Function(p) p.mz).ToArray,
                .rt = rt.Average,
                .scan_id = $"[MS1] tgroup={ .rt.ToString("F4")}; {scan_id}",
                .TIC = .into.Sum,
                .BPC = .into.Max,
                .meta = New Dictionary(Of String, String),
                .products = raw _
                    .Select(Function(r)
                                Return New ScanMS2 With {
                                    .activationMethod = [Enum].Parse(GetType(mzData.ActivationMethods), r.activation),
                                    .centroided = True,
                                    .charge = 0,
                                    .collisionEnergy = r.collisionEnergy,
                                    .intensity = r.intensity,
                                    .into = r.mzInto.Select(Function(i) i.intensity).ToArray,
                                    .mz = r.mzInto.Select(Function(i) i.mz).ToArray,
                                    .parentMz = r.mz,
                                    .polarity = 0,
                                    .rt = r.rt,
                                    .scan_id = r.lib_guid
                                }
                            End Function) _
                    .ToArray
            }
        End Function

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
            Dim pzero As Double = XIC.Count(Function(ti) ti.Intensity = 0.0) / XIC.Length

            If pzero > 0.5 Then
                ' needs data interplation
                Dim nonzero = XIC.Where(Function(ti) ti.Intensity > 0) _
                    .OrderBy(Function(ti) ti.Time) _
                    .ToArray
                Dim dt_avg As Double = nonzero _
                    .Skip(1) _
                    .Select(Function(ti, i) ti.Time - nonzero(i).Time) _
                    .TabulateMode

                If dt_avg >= 3 Then
                    Dim XIClist As New List(Of ChromatogramTick)

                    For i As Integer = 0 To nonzero.Length - 2
                        Dim nextT = nonzero(i + 1)
                        Dim mid As New ChromatogramTick With {
                            .Time = nonzero(i).Time + dt_avg,
                            .Intensity = (nonzero(i).Intensity + nextT.Intensity) / 2
                        }

                        XIClist.Add(nonzero(i))
                        XIClist.Add(mid)
                    Next

                    XIClist.Add(New ChromatogramTick With {.Time = nonzero.Last.Time + dt_avg, .Intensity = nonzero.Last.Intensity / 2})
                    XIC = XIClist.ToArray
                End If
            End If

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