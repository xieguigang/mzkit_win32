Public Module AppEnvironment

    Public ReadOnly Property IsDevelopmentMode As Boolean

    Sub New()
        IsDevelopmentMode = getTestDevelopmentMode()
    End Sub

    Private Function getTestDevelopmentMode() As Boolean
        Static HOME As String = App.HOME.GetDirectoryFullPath.Replace("/"c, "\"c)

        If HOME = "E:\mzkit\dist\bin" OrElse HOME = "D:\mzkit\dist\bin" Then
            Dim drive As String = HOME.Split(":"c).First
            Dim githubFolder As String() = {
                "\mzkit\Rscript",
                "\mzkit\Sciex",
                "\mzkit\src",
                "\mzkit\ThermoFisher",
                "\mzkit\.github"
            }

            Return githubFolder.All(Function(ref) $"{drive}:{ref}".DirectoryExists)
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' get web view html source file folder
    ''' </summary>
    ''' <returns></returns>
    Public Function getWebViewFolder() As String
        If IsDevelopmentMode Then
            Return $"{App.HOME}/../../src/mzkit/webview/"
        Else
            Return $"{App.HOME}/WebView/"
        End If
    End Function

End Module
