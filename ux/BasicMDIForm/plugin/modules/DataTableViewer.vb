Imports System.Runtime.CompilerServices
Imports Galaxy.Workbench

Public Module DataTableViewer

    Private openTable As Func(Of IDataTableViewer)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub HookTableViewer(open As Func(Of IDataTableViewer))
        openTable = open
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub OpenInTableViewer(matrix As DataGridView)
        Call openTable().LoadTable(AddressOf New TableValueCopy(matrix).LoadTable)
    End Sub
End Module



