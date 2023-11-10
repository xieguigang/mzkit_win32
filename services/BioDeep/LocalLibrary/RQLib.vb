Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports RQL

''' <summary>
''' Model for local library file for supports the metabolite 
''' annotation on ms1/ms2 level
''' </summary>
Public Class RQLib : Implements IDisposable

    ReadOnly query As Resource
    ReadOnly opts As New JSONSerializerOptions With {
        .indent = False,
        .maskReadonly = False,
        .enumToString = True,
        .unixTimestamp = True
    }

    ''' <summary>
    ''' mapping the unique id key to the resource map link
    ''' </summary>
    ReadOnly spectralMap As New Dictionary(Of String, String)

    Private disposedValue As Boolean

    Sub New(file As Stream)
        query = New Resource(New StreamPack(file))
        loadMap()
    End Sub

    Private Sub loadMap()
        Dim mapList As String = query.Archive.ReadText("/spectralMap.txt")
        Dim lines As String() = mapList.LineTokens
        Dim ref As NamedValue(Of String)

        ' guid: map
        For Each line As String In lines
            ref = line.GetTagValue(":", trim:=True)
            spectralMap.Add(ref.Name, ref.Value)
        Next
    End Sub

    Private Sub saveMap()
        Dim lines As String() = spectralMap.Select(Function(t) $"{t.Key}:{t.Value}").ToArray

        Call query.Archive.WriteText(lines, "/spectralMap.txt")
    End Sub

    Const class_metadata As String = "metadata"
    Const class_spectrum As String = "spectrum"

    Public Iterator Function ListMetabolites(Optional page As Integer = 1, Optional page_size As Integer = 100) As IEnumerable(Of MetaLib)
        Dim start As Integer = (page - 1) * page_size
        Dim pulls = query.Archive.GetFiles($"/pool/{class_metadata}/")
        Dim list = pulls.Skip(start).Take(page_size)
        Dim libfile As StreamPack = query.Archive
        Dim buf As MemoryStream

        For Each filepath As String In list
            buf = libfile.ReadBinary(filepath)

            If Not buf Is Nothing Then
                Yield ParseMetadata(buf.ToArray)
            End If
        Next
    End Function

    ''' <summary>
    ''' Add metabolite annotation metadata
    ''' </summary>
    ''' <param name="metabo"></param>
    ''' <returns></returns>
    Public Function AddAnnotation(metabo As MetaLib) As Boolean
        Dim packdata = BSON.GetBuffer(metabo.GetType.GetJsonElement(metabo, opts)).ToArray

        Call query.Add(metabo.ID, packdata, class_metadata)
        Call query.Add(metabo.name, packdata, class_metadata)
        Call query.Add(metabo.formula, packdata, class_metadata)
        Call query.Add(metabo.IUPACName, packdata, class_metadata)

        For Each name As String In metabo.synonym.SafeQuery
            Call query.Add(name, packdata, class_metadata)
        Next

        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mspeak"></param>
    ''' <param name="key">
    ''' the additional associated key of this spectrum data 
    ''' associates with the corresponding metabolite annotation 
    ''' data.
    ''' </param>
    ''' <returns></returns>
    Public Function AddSpectrum(mspeak As PeakMs2, key As String) As Boolean
        Dim buffer As New MemoryStream
        Dim bw As New BinaryDataWriter(buffer)
        Call Serialization.WriteBuffer(mspeak.Scan2, file:=bw)
        Call bw.Flush()
        Dim packdata = buffer.ToArray
        Dim mapKey As String = Resource.GetHashKey(packdata)

        Call query.Add(mspeak.lib_guid, packdata, class_spectrum)
        Call query.Add(mspeak.precursor_type, packdata, class_spectrum)
        Call query.Add(key, packdata, class_spectrum)

        spectralMap(key) = mapKey

        Return True
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function ParseMetadata(dat As Byte()) As MetaLib
        Return BSON _
            .Load(dat) _
            .CreateObject(Of MetaLib)(decodeMetachar:=False)
    End Function

    Public Function GetSpectrumByKey(id As String) As PeakMs2
        Dim map As String = spectralMap(id)
        Dim buffer = query.ReadBuffer(map, category:=class_spectrum)
        Dim ms2 As ScanMS2 = Serialization.ParseScan2(buffer)
        Dim spectral As PeakMs2 = mzPack.CastToPeakMs2(ms2, file:="spectral")

        Erase buffer

        Return spectral
    End Function

    ''' <summary>
    ''' just get metabolite annotation information
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns></returns>
    Public Iterator Function QueryMetabolites(name As String) As IEnumerable(Of MetaLib)
        Dim list As NumericTagged(Of String)() = query.Get(query:=name).ToArray
        Dim keys As String() = list _
            .Select(Function(m) m.value) _
            .Distinct _
            .ToArray
        Dim dat As Byte()

        For i As Integer = 0 To keys.Length - 1
            dat = query.ReadBuffer(
                map:=keys(i),
                category:=class_metadata
            )

            If Not dat.IsNullOrEmpty Then
                Yield ParseMetadata(dat)
            End If
        Next
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call saveMap()
                Call query.Dispose()
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
