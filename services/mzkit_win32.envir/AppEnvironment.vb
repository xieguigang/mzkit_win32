
Imports System.Runtime.InteropServices

Namespace Container

    ''' <summary>
    ''' A shared model for test application environment
    ''' </summary>
    Public Module AppEnvironment

        <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Function SetDllDirectory(lpPathName As String) As Boolean
        End Function

        <DllImport("kernel32", SetLastError:=True)>
        Public Function LoadLibrary(lpFileName As String) As IntPtr
        End Function

        ''' <summary>
        ''' mzkit_win32 is running in a source development environment
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsDevelopmentMode As Boolean

        Sub New()
            IsDevelopmentMode = getTestDevelopmentMode()
        End Sub

        Private Function getTestDevelopmentMode() As Boolean
            Static HOME As String = App.HOME.GetDirectoryFullPath.Replace("/"c, "\"c)

            For Each drive As String In New String() {"C", "D", "E", "F", "G", "H"}
                If HOME.StartsWith($"{drive}:\mzkit\dist\bin") Then
                    Dim githubFolder As String() = {
                        "\mzkit\Rscript",
                        "\mzkit\Sciex",
                        "\mzkit\src",
                        "\mzkit\ThermoFisher",
                        "\mzkit\.github"
                    }

                    Return githubFolder.All(Function(ref) $"{drive}:{ref}".DirectoryExists)
                End If
            Next

            Return False
        End Function

        Public Function get3DMALDIDemoFolder() As String
            If IsDevelopmentMode Then
                Return $"{App.HOME}/../../src/mzkit/extdata/3D-MALDI"
            Else
                Return $"{App.HOME}/demo/3D-MALDI/"
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

        Public Function GetNdpiTools() As String
            If AppEnvironment.IsDevelopmentMode Then
                Return $"{App.HOME}/../../src/mzkit/dist/ndpitools/".GetDirectoryFullPath
            Else
                Return $"{App.HOME}/tools/ndpitools/".GetDirectoryFullPath
            End If
        End Function

        Public Function getWkhtmltopdf() As String
            If AppEnvironment.IsDevelopmentMode Then
                Return $"{App.HOME}/../../src/mzkit/dist/wkhtmltopdf.exe".GetFullPath
            Else
                Return $"{App.HOME}/tools/wkhtmltopdf.exe".GetFullPath
            End If
        End Function

#Region "dzitools"

        ''' <summary>
        ''' this function just works for the dzitools
        ''' </summary>
        ''' <returns></returns>
        Public Function getOpenSlideLibDLL() As String
            Dim libdll As String

            If AppEnvironment.IsDevelopmentMode Then
                libdll = $"{App.HOME}/../../../../src/mzkit/dist/OpenSlide/openslide-win64/bin"
            Else
                libdll = $"{App.HOME}/../openslide-win64/bin/"
            End If

            libdll = libdll.GetDirectoryFullPath

            Return libdll
        End Function

        ''' <summary>
        ''' this function just works for the dzitools
        ''' </summary>
        ''' <returns></returns>
        Public Function getVIPS() As String
            Dim libdll As String

            If AppEnvironment.IsDevelopmentMode Then
                libdll = $"{App.HOME}/../../../../src/mzkit/services/dzitools/dist/vips/bin"
            Else
                libdll = $"{App.HOME}/../vips/bin/"
            End If

            libdll = libdll.GetDirectoryFullPath

            Return libdll
        End Function
#End Region
    End Module
End Namespace