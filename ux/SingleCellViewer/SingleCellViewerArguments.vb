Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq

Public Class SingleCellViewerArguments

    Public ReadOnly Property cells As Integer
    Public ReadOnly Property clusters As Integer
    Public Property colorSet As CategoryPalettes = CategoryPalettes.Paper
    Public Property pointSize As Single = 10
    Public Property background As Color = Color.White

    Sub New(cells As IEnumerable(Of UMAPPoint))
        Dim groups = cells.GroupBy(Function(c) c.class).Select(Function(a) a.ToArray).ToArray

        Me.clusters = groups.Length
        Me.cells = groups.IteratesALL.Count
    End Sub

End Class
