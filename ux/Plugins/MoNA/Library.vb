Imports Microsoft.VisualBasic.Serialization.JSON

Public Class Library

    Public Const librariesURL As String = "https://mona.fiehnlab.ucdavis.edu/rest/downloads/predefined"

    Public Property description As String
    Public Property label As String
    Public Property query As String
    Public Property queryCount As Integer
    Public Property jsonExport As Export
    Public Property mspExport As Export
    Public Property sdfExport As Export

    Public Shared Function LoadLibraries() As Library()
        Return librariesURL.GET.LoadJSON(Of Library())
    End Function

End Class

Public Class Export

    Public Property count As Integer
    Public Property [date] As Long
    Public Property exportFile As String
    Public Property format As String
    Public Property id As String
    Public Property label As String
    Public Property query As String
    Public Property queryFile As String
    Public Property size As Long

End Class