
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Serialization.JSON

<ClassInterface(ClassInterfaceType.AutoDual)>
<ComVisible(True)>
Public Class LCMSScatter

    ''' <summary>
    ''' the scatter raw data in current view range
    ''' </summary>
    Friend rawdata As Meta()

    Public Function GetLCMSScatter() As String
        Return rawdata.GetJson
    End Function

End Class
