Imports System.Runtime.CompilerServices

Module BaseHook

    Friend getColorSet As Func(Of String)
    Friend showProperties As Action(Of Object)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub HookPlotColorSet(del As Func(Of String))
        getColorSet = del
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub HookShowProperties(displ As Action(Of Object))
        showProperties = displ
    End Sub

End Module
