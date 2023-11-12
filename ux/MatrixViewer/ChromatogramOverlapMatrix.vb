Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.SignalReader
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports mzblender

Public Class ChromatogramOverlapMatrix : Inherits DataMatrix

    ReadOnly spatial3D As Boolean

    Public Sub New(name As String, matrix As ChromatogramSerial(), spatial3D As Boolean)
        MyBase.New(name, matrix.FileAlignment(dt:=1).ToArray)

        Me.spatial3D = spatial3D
    End Sub

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(ChromatogramSerial)
        End Get
    End Property

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim lines = GetMatrix(Of ChromatogramSerial)()
        Dim ticks As Integer = lines(0).size
        Dim data As Object()
        Dim rt As Double() = lines(0).GetTime

        For i As Integer = 0 To ticks - 1
            data = New Object(lines.Length) {}
            data(0) = rt(i)

            For j As Integer = 0 To lines.Length - 1
                data(j + 1) = lines(j)(i).Intensity
            Next

            Call table.Rows.Add(data)
        Next
    End Sub

    Public Overrides Function Plot(args As Task.PlotProperty, picBox As Size) As GraphicsData
        Dim blender As Blender
        Dim tic = GetMatrix(Of ChromatogramSerial)()

        If spatial3D Then
            blender = New XIC3DBlender(tic.Select(Function(c) c.GetTuple))
        Else
            blender = New ChromatogramBlender(tic.Select(Function(c) c.GetTuple))
        End If

        Return New ImageData(blender.Rendering(args, picBox))
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("retention time", GetType(Double))

        For Each line In GetMatrix(Of ChromatogramSerial)()
            Yield New NamedValue(Of Type)(line.Name, GetType(Double))
        Next
    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Using text As New StreamWriter(s)
            Dim lines = GetMatrix(Of ChromatogramSerial)()
            Dim ticks As Integer = lines(0).size
            Dim data As String()
            Dim rt As Double() = lines(0).GetTime

            data = {"retention time"}.JoinIterates(From line As ChromatogramSerial In lines Select line.Name).ToArray

            Call text.WriteLine(New RowObject(data).AsLine)

            For i As Integer = 0 To ticks - 1
                data = New String(lines.Length) {}
                data(0) = rt(i)

                For j As Integer = 0 To lines.Length - 1
                    data(j + 1) = lines(j)(i).Intensity
                Next

                Call text.WriteLine(data.JoinBy(","))
            Next
        End Using

        Return True
    End Function
End Class
