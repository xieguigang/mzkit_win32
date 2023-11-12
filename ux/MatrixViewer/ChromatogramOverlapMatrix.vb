Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports mzblender

Public Class ChromatogramOverlapMatrix : Inherits DataMatrix

    ReadOnly spatial3D As Boolean

    Public Sub New(name As String, matrix As NamedCollection(Of ChromatogramTick)(), spatial3D As Boolean)
        MyBase.New(name, matrix)

        Me.spatial3D = spatial3D
    End Sub

    Public Overrides ReadOnly Property UnderlyingType As Type

    Protected Overrides Sub CreateRows(table As DataTable)
        Throw New NotImplementedException()
    End Sub

    Public Overrides Function Plot(args As Task.PlotProperty, picBox As Size) As GraphicsData
        Dim blender As Blender
        Dim tic = GetMatrix(Of NamedCollection(Of ChromatogramTick))()

        If spatial3D Then
            blender = New XIC3DBlender(tic)
        Else
            blender = New ChromatogramBlender(tic)
        End If

        Return New ImageData(blender.Rendering(args, picBox))
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("retention time", GetType(Double))

        For Each line In GetMatrix(Of NamedCollection(Of ChromatogramTick))()
            Yield New NamedValue(Of Type)(line.name, GetType(Double))
        Next
    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Using text As New StreamWriter(s)

        End Using

        Return True
    End Function
End Class
