Namespace ServiceHub.Manager

    Public Module Hub

        ''' <summary>
        ''' due to the reason of services name could be duplicated, so dictionary is not working as expected
        ''' </summary>
        Dim list As New List(Of Service)

        ''' <summary>
        ''' update system resource usage and then populate the services object
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function ServicesList() As IEnumerable(Of Service)
            For Each item In list
                Dim p As Process = Nothing

                Try
                    p = Process.GetProcessById(item.PID)
                Catch ex As Exception
                End Try

                If p Is Nothing Then
                    Yield New Service With {
                        .description = item.description,
                        .name = item.name,
                        .PID = item.PID,
                        .isAlive = False,
                        .port = item.port
                    }
                Else
                    Yield New Service With {
                        .description = item.description,
                        .name = item.name,
                        .PID = item.PID,
                        .port = item.port,
                        .isAlive = p.HasExited,
                        .cpu = p.TotalProcessorTime.TotalMilliseconds - item.cpu,
                        .mem = p.WorkingSet64
                    }

                    item.cpu = p.TotalProcessorTime.TotalMilliseconds
                End If
            Next
        End Function

        Public Sub Register(svr As Service)
            list.Add(svr)
        End Sub

        Public Sub RegisterSingle(svr As Service)
            For Each item In list.ToArray
                If item.name = svr.name Then
                    list.Remove(item)
                    Exit For
                End If
            Next

            Call list.Add(item:=svr)
        End Sub

        ''' <summary>
        ''' try to shutdown all registered service process
        ''' </summary>
        Public Sub Shutdown()
            For Each item As Service In list
                Try
                    Call Process.GetProcessById(item.PID).Kill()
                Catch ex As Exception
                    ' just ignores the process kill error
                End Try
            Next
        End Sub

    End Module
End Namespace