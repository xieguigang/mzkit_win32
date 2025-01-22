Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender

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
        End Set
    End Property

    Public Property data As PixelData()

    Public Property ions As MzAnnotation()
        Get
            Return _ions
        End Get
        Set(value As MzAnnotation())
            _ions = value

            If Not _ions.IsNullOrEmpty Then
                TextBox1.Text = _ions.Select(Function(a) a.productMz.ToString("F4")).JoinBy(", ")
                TextBox2.Text = If(_ions.Length = 1, "Single Ion Imaging", "Multiple Ions Rendering")
            End If
        End Set
    End Property

    Public Overrides Property text As String
        Get
            Return TextBox3.Text
        End Get
        Set(value As String)
            TextBox3.Text = value
        End Set
    End Property

End Class
