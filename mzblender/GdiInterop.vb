Imports System.Runtime.CompilerServices

#If NET48 Then
Imports Image = System.Drawing.Image
#Else
Imports Image = Microsoft.VisualBasic.Imaging.Image
#End If

Module GdiInterop

    <Extension>
    Public Function CTypeGdiImage(image As Image) As System.Drawing.Image
#If NET48 Then
        Return image
#Else
        Throw New NotImplementedException
#End If
    End Function
End Module
