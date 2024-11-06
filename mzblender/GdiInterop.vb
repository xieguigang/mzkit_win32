Imports System.Runtime.CompilerServices

#If NET48 Then
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
#Else
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
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

    <Extension>
    Public Function CTypeGdiImage(bitmap As Bitmap) As System.Drawing.Bitmap
#If NET48 Then
        Return bitmap
#Else
        Throw New NotImplementedException
#End If
    End Function
End Module
