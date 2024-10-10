Imports System.Management

Namespace nv

    Friend Class WQL

        ''' <summary>
        ''' Returns the most suitable graphics adapter in the system whiles also displaying to the user what graphics adapter they have.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function DetectGraphicsAdapter() As GraphicsAdapter
            Dim graphicsAdapter As GraphicsAdapter = GetGraphicsAdapter()

            Dim graphicsAdapterName = graphicsAdapter.Name
            Program.message(graphicsAdapterName)

            Return graphicsAdapter
        End Function

        ''' <summary>
        ''' Returns the most suitable graphics adapter present in the system.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetGraphicsAdapter() As GraphicsAdapter
            ' Gets every graphics adapter in use by the system.
            Dim searcher As ManagementObjectSearcher = New ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration")

            Dim graphicsAdapters As List(Of GraphicsAdapter) = New List(Of GraphicsAdapter)()

            ' Saves the name of every found graphics card in a list.
            For Each mo As ManagementObject In searcher.Get()
                For Each [property] In mo.Properties
                    If Equals([property].Name, "Description") Then graphicsAdapters.Add(New GraphicsAdapter([property].Value.ToString()))
                Next
            Next

            ' If no graphics adapters could be found...
            If graphicsAdapters.Count < 1 Then
                Program.message("For some unknown reason, your display adapter could not be found." & vbLf & vbLf & "Perhaps you have some security program blocking me access?" & vbLf & vbLf & "If you feel like this is a mistake, enable 'Force Mode'.")
                Return New GraphicsAdapter("N/A")
            End If

            ' For every graphics adapter, find its support level by looking up its name.
            For Each graphicsAdapter In graphicsAdapters

                If FullySupportedGraphicsAdapters().Any(Function(s) graphicsAdapter.Name.ToLower().Contains(s)) Then
                    graphicsAdapter.SupportLevel = SupportLevel.Full
                ElseIf PartiallySupportedGraphicsAdapters().Any(Function(s) graphicsAdapter.Name.ToLower().Contains(s)) Then
                    graphicsAdapter.SupportLevel = SupportLevel.Partial
                Else
                    graphicsAdapter.SupportLevel = SupportLevel.None
                End If
            Next

            ' Orders the graphics adapters by support level (first graphics adapter should be most suitable).
            graphicsAdapters.OrderBy(Function(o) o.SupportLevel).Reverse().ToList()

            ' Returns the most suitable graphics adapter.
            Return graphicsAdapters(0)
        End Function

        Private Shared Iterator Function FullySupportedGraphicsAdapters() As IEnumerable(Of String)
            Yield "rtx"
        End Function

        Private Shared Iterator Function PartiallySupportedGraphicsAdapters() As IEnumerable(Of String)
            Yield "1030"
            Yield "1040"
            Yield "1050"
            Yield "1060"
            Yield "1070"
            Yield "1080"
            Yield "titan"
            Yield "1640"
            Yield "1650"
            Yield "1660"
            Yield "1670"
            Yield "1680"
        End Function
    End Class
End Namespace
