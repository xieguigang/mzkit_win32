Imports System.Runtime.CompilerServices

Public NotInheritable Class Pages

    Shared ReadOnly documentTypes As New Dictionary(Of String, Type)

    Private Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub SetDocument(name As String, type As Type)
        documentTypes(name) = type
    End Sub

    Public Shared Function OpenDocument(name As String) As DocumentWindow
        Return Workbench.ShowDocument(documentTypes(name))
    End Function
End Class
