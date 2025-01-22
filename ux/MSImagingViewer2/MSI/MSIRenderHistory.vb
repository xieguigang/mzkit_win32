Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Public Class MSIRenderHistory

    Public Property thumbnail As Image
        Get
            Return PictureBox1.BackgroundImage
        End Get
        Set(value As Image)
            PictureBox1.BackgroundImage = value
        End Set
    End Property

    Dim _rgb As RGBConfigs
    Dim _ions As MzAnnotation()

    Public Property rgb As RGBConfigs
        Get
            Return _rgb
        End Get
        Set(value As RGBConfigs)
            _rgb = value

            If Not _rgb Is Nothing Then
                TextBox1.Text = _rgb.ToString
                TextBox2.Text = "RGB Composition"
            End If

            If Not value Is Nothing Then
                _ions = Nothing
            End If

            If Text.StringEmpty(, True) AndAlso Not value Is Nothing Then
                Text = _rgb.ToString
            End If
        End Set
    End Property

    Public Property data As PixelData()
    Public Property mzdiff As Tolerance

    Public Property ions As MzAnnotation()
        Get
            Return _ions
        End Get
        Set(value As MzAnnotation())
            _ions = value

            If Not value.IsNullOrEmpty Then
                _rgb = Nothing
            End If

            If Not _ions.IsNullOrEmpty Then
                TextBox1.Text = _ions.Select(Function(a) a.productMz.ToString("F4")).JoinBy(", ")
                TextBox2.Text = If(_ions.Length = 1, "Single Ion Imaging", "Multiple Ions Rendering")
            End If

            If Text.StringEmpty(, True) AndAlso Not value.IsNullOrEmpty Then
                If value.Length = 1 Then
                    Text = If(value(0).annotation.StringEmpty(, True), value(0).ToString("F4"), value(0).annotation)
                Else
                    Text = value.Select(Function(i) i.productMz.ToString("F3")).JoinBy(",")
                End If
            End If
        End Set
    End Property

    ''' <summary>
    ''' Ion render title
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Property Text As String
        Get
            Return TextBox3.Text
        End Get
        Set(value As String)
            TextBox3.Text = value
            RaiseEvent TitleUpdated(Me, value)
        End Set
    End Property

    Public Event TitleUpdated(card As MSIRenderHistory, title As String)
    Public Event ExportMatrixCDF(card As MSIRenderHistory)

    Public Function GetTitle(mz As Double) As String
        If Not _ions.IsNullOrEmpty Then
            Return SelectTitle(_ions, mz)
        ElseIf Not _rgb Is Nothing Then
            Return SelectTitle(_rgb.AsEnumerable, mz)
        Else
            Return $"M/Z: {mz}"
        End If
    End Function

    Public Function HasData() As Boolean
        Return Not data.IsNullOrEmpty
    End Function

    Public Function TargetMz() As Double()
        If Not rgb Is Nothing Then
            Return rgb.AsEnumerable.Select(Function(a) a.productMz).ToArray
        ElseIf Not ions.IsNullOrEmpty Then
            Return ions.Select(Function(a) a.productMz).ToArray
        Else
            Return {}
        End If
    End Function

    Public Function IntensityData() As IEnumerable(Of Double)
        Return From i As PixelData In data.SafeQuery Select i.intensity
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function SelectTitle(ions As IEnumerable(Of MzAnnotation), mz As Double) As String
        Dim annodata = ions _
            .Where(Function(i) std.Abs(i.productMz - mz) < 0.3) _
            .OrderBy(Function(i) std.Abs(i.productMz - mz)) _
            .FirstOrDefault

        If annodata Is Nothing OrElse annodata.annotation.StringEmpty(, True) Then
            Return $"M/Z: {mz}"
        Else
            Return annodata.annotation
        End If
    End Function

    Private Sub ExportMatrixCDFToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportMatrixCDFToolStripMenuItem.Click
        RaiseEvent ExportMatrixCDF(Me)
    End Sub
End Class
