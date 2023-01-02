Imports System.Reflection
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' the mzkit plugin model abstract
''' </summary>
Public MustInherit Class Plugin

    Public MustOverride ReadOnly Property guid As Guid
    Public MustOverride ReadOnly Property Name As String
    Public MustOverride ReadOnly Property Link As String
    Public MustOverride ReadOnly Property Description As String

    ''' <summary>
    ''' run this plugin
    ''' </summary>
    Public MustOverride Function Init(println As Action(Of String)) As Boolean
    Public MustOverride Sub Exec()

    Public Function GetMetadata() As PluginMetadata
        Dim asm As AssemblyInfo = Me.GetType.Assembly.FromAssembly

        Return New PluginMetadata With {
            .author = asm.AssemblyCompany,
            .desc = Description,
            .id = guid.ToString,
            .name = Name,
            .url = Link,
            .ver = asm.AssemblyVersion
        }
    End Function

    Public Shared Sub LoadPlugins(println As Action(Of String))
        Dim files As String() = $"{App.HOME}/plugins".EnumerateFiles("*.dll").JoinIterates($"{App.LocalData}/plugins/".EnumerateFiles("*.dll")).ToArray
        Dim loaded As Index(Of String) = New String() {}
        Dim registry As RegistryFile = RegistryFile.LoadRegistry
        Dim hashlist = registry.plugins.ToDictionary(Function(p) p.id)

        Call println($"load plugins: get {files.Length} dll modules")

        For Each path As String In files
            Dim asm As Assembly = Assembly.LoadFile(path.GetFullPath)

            If Not MZKitPlugin.GetFlag(asm) Then
                Continue For
            Else
                Call println($"scan plugins from '{path.FileName}'!")
            End If

            Dim types As Type() = asm.GetExportedTypes

            For Each type As Type In types
                If type.IsInheritsFrom(GetType(Plugin)) Then
                    Dim obj As Object = Activator.CreateInstance(type)
                    Dim plugin As Plugin = DirectCast(obj, Plugin)
                    Dim pluginData As PluginMetadata = plugin.GetMetadata

                    If pluginData.id Like loaded Then
                        Continue For
                    Else
                        loaded.Add(pluginData.id)

                        If Not hashlist.ContainsKey(pluginData.id) Then
                            Call hashlist.Add(pluginData.id, pluginData)
                        Else
                            pluginData.status = hashlist(pluginData.id).status
                            hashlist(pluginData.id) = pluginData
                        End If
                    End If

                    If registry.IsDisabled(pluginData.id) Then
                        Continue For
                    End If

                    Try
                        If Not plugin.Init(println) Then
                            pluginData.status = "incompatible"
                        End If
                    Catch ex As Exception
                        Call println($"Load plugin error: {ex.Message}")
                        Call App.LogException(ex)
                    End Try
                End If
            Next
        Next

        registry.plugins = hashlist.Values.ToArray

        Call registry.Save()
        Call println("scan plugin job done!")
    End Sub
End Class