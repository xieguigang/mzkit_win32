Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Imaging.Math2D

Public Module MergeSlides

    Public Function JoinDataSet(file As IEnumerable(Of String), layout As String, fileNameAsSourceTag As Boolean) As mzPack
        Dim rawfiles As Dictionary(Of String, mzPack) = file _
            .ToDictionary(Function(path) path.BaseName,
                          Function(path)
                              Call RunSlavePipeline.SendMessage($"read {path}...")

                              Using buf As Stream = path.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                                  Dim data = mzPack.ReadAll(buf, ignoreThumbnail:=True, skipMsn:=True)
                                  If fileNameAsSourceTag Then
                                      data.source = path.BaseName
                                  End If
                                  Return data
                              End Using
                          End Function)

        If layout.StringEmpty Then
            ' merge data in linear
            Return rawfiles.Values.JoinMSISamples(println:=AddressOf RunSlavePipeline.SendMessage)
        Else
            Dim layoutData As String()() = layout _
                .SolveStream _
                .LineTokens _
                .Select(Function(line) line.Split(","c)) _
                .ToArray

            Return rawfiles.MergeDataWithLayout(layoutData)
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="layout">
    ''' the grid layout, value should be whitespace or the file base name
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function MergeDataWithLayout(raw As Dictionary(Of String, mzPack), layout As String()()) As mzPack
        Dim polygons = raw.ToDictionary(
            Function(m) m.Key,
            Function(m)
                Return New Polygon2D(m.Value.MS.Select(Function(s1) s1.GetMSIPixel))
            End Function)
        Dim averageWidth = Aggregate p As Polygon2D In polygons.Values Into Average(p.GetRectangle.Width)
        Dim averageHeight = Aggregate p As Polygon2D In polygons.Values Into Average(p.GetRectangle.Height)
        Dim union As New List(Of ScanMS1)
        Dim padding As New Size(30, 30)
        Dim top As Integer = padding.Height
        Dim left As Integer = padding.Width
        Dim relativePos As Boolean = True
        Dim norm As Boolean = True
        Dim println As Action(Of String) = AddressOf RunSlavePipeline.SendMessage
        Dim scan_x As Integer
        Dim scan_y As Integer

        For Each row As String() In layout
            Dim maxHeight As Double = averageHeight

            For Each col As String In row
                If col.IsPattern("\s+") Then
                    ' is a blank space, just offset the box
                    left += padding.Width * 2 + averageWidth
                Else
                    Dim sample As mzPack = raw(col)
                    Dim sample_shape = polygons(col)
                    Dim rect As RectangleF = sample_shape.GetRectangle

                    union.JoinOneSample(
                        shape:=sample_shape,
                        sample:=sample,
                        left:=left,
                        top:=top,
                        relativePos:=relativePos,
                        norm:=norm,
                        println:=println
                    )
                    left += padding.Width * 2 + rect.Width

                    If rect.Height > maxHeight Then
                        maxHeight = rect.Height
                    End If
                End If
            Next

            If left > scan_x Then
                scan_x = left
            End If

            ' offset the top
            top += padding.Height + maxHeight
            ' reset left offset
            left = padding.Width

            If top > scan_y Then
                scan_y = top
            End If
        Next

        Return New mzPack With {
            .MS = union.ToArray,
            .Application = FileApplicationClass.MSImaging,
            .source = raw.Keys.JoinBy("+"),
            .metadata = New Dictionary(Of String, String) From {
                {"scan_x", scan_x + padding.Width},
                {"scan_y", scan_y + padding.Height}
            }
        }
    End Function
End Module