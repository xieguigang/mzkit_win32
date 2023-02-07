Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Math.SignalProcessing

Public Class RedisScan1 : Inherits MSScan
    Implements ITimeSignal, IRetentionTime

    Public Overrides Property rt As Double Implements ITimeSignal.time, IRetentionTime.rt
    Public Property TIC As Double Implements ITimeSignal.intensity
    Public Property BPC As Double

    ''' <summary>
    ''' the redis key to the ms2 object
    ''' </summary>
    ''' <returns></returns>
    Public Property products As String()

    ''' <summary>
    ''' other meta data about this MS1 scan, likes
    ''' the [x,y] coordinate data of MSI scan data.
    ''' </summary>
    ''' <returns></returns>
    Public Property meta As Dictionary(Of String, String)

    Public Shared Function FromData(data As ScanMS1, keys2 As IEnumerable(Of String)) As RedisScan1
        Return New RedisScan1 With {
            .BPC = data.BPC,
            .into = data.into,
            .meta = data.meta,
            .mz = data.mz,
            .products = keys2.ToArray,
            .rt = data.rt,
            .scan_id = data.scan_id,
            .TIC = data.TIC
        }
    End Function
End Class
