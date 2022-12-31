Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Class RegistryFile : Inherits ListOf(Of PluginMetadata)

    <XmlElement>
    Public Property plugins As PluginMetadata()

    Friend Shared ReadOnly defaultLocation As String

    Default Friend ReadOnly Property GetElementById(id As String) As PluginMetadata
        Get
            Return plugins.KeyItem(id)
        End Get
    End Property

    Public Sub Save()
        Call Me.GetXml.SaveTo(defaultLocation)
    End Sub

    Protected Overrides Function getSize() As Integer
        Return plugins.TryCount
    End Function

    Protected Overrides Function getCollection() As IEnumerable(Of PluginMetadata)
        Return plugins
    End Function
End Class
