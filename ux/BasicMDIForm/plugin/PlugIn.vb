Imports System.Reflection

''' <summary>
''' the mzkit plugin model abstract
''' </summary>
Public MustInherit Class Plugin

    Public MustOverride ReadOnly Property Name As String
    Public MustOverride ReadOnly Property Link As String
    Public MustOverride ReadOnly Property Description As String

    ''' <summary>
    ''' run this plugin
    ''' </summary>
    Public MustOverride Sub Init(println As Action(Of String))

    Public Shared Sub LoadPlugins(dir As String, println As Action(Of String))
        Dim files As String() = dir.EnumerateFiles("*.dll").ToArray

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

                    Call plugin.Init(println)
                End If
            Next
        Next

        Call println("scan plugin job done!")
    End Sub
End Class