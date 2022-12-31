Imports System.Runtime.InteropServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models

<ClassInterface(ClassInterfaceType.AutoDual)>
<ComVisible(True)>
Public Class PluginMgr

    Dim registry As RegistryFile

    Private Sub New()
    End Sub

    Public Sub SetStatus(id As String, status As String)
        Dim plugin As PluginMetadata = registry(id)

        If Not plugin Is Nothing Then
            plugin.status = status
        End If
    End Sub

    Public Function GetPlugins() As String
        Return registry.plugins.GetJson
    End Function

    Public Sub InstallLocal()
        Using file As New OpenFileDialog With {.Filter = "MZKit plugin(*.dll)|*.dll"}
            If file.ShowDialog = DialogResult.OK Then

            End If
        End Using
    End Sub

    Public Sub Save()
        Call registry.Save()
    End Sub

    Public Shared Function Load() As PluginMgr
        Dim registry = RegistryFile.defaultLocation.LoadXml(Of RegistryFile)(throwEx:=False)
        If registry Is Nothing Then
            registry = New RegistryFile With {.plugins = {}}
            registry.Save()
        End If
        Return New PluginMgr With {.registry = registry}
    End Function
End Class
