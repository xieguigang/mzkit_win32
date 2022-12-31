Imports System.Runtime.InteropServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models

<ClassInterface(ClassInterfaceType.AutoDual)>
<ComVisible(True)>
Public Class PluginMgr : Inherits ListOf(Of PluginMetadata)

    <XmlElement>
    Public Property plugins As PluginMetadata()

    Shared ReadOnly defaultLocation As String

    Public Sub SetStatus(id As String, status As String)
        Dim plugin As PluginMetadata = plugins _
            .Where(Function(a) a.id = id) _
            .FirstOrDefault

        If Not plugin Is Nothing Then
            plugin.status = status
        End If
    End Sub

    Public Function GetPlugins() As String
        Return plugins.GetJson
    End Function

    Public Sub InstallLocal()
        Using file As New OpenFileDialog With {.Filter = "MZKit plugin(*.dll)|*.dll"}
            If file.ShowDialog = DialogResult.OK Then

            End If
        End Using
    End Sub

    Public Sub Save()
        Call Me.GetXml.SaveTo(defaultLocation)
    End Sub

    Protected Overrides Function getSize() As Integer
        Return plugins.TryCount
    End Function

    Protected Overrides Function getCollection() As IEnumerable(Of PluginMetadata)
        Return plugins
    End Function

    Public Shared Function Load() As PluginMgr
        Dim registry = defaultLocation.LoadXml(Of PluginMgr)(throwEx:=False)
        If registry Is Nothing Then
            registry = New PluginMgr With {.plugins = {}}
            registry.Save()
        End If
        Return registry
    End Function
End Class
