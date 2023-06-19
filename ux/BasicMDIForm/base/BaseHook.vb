Imports System.Runtime.CompilerServices

Module BaseHook

    Friend getColorSet As Func(Of String)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub HookPlotColorSet(del As Func(Of String))
        getColorSet = del
    End Sub

End Module
