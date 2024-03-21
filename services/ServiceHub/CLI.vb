Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection

<RunDllEntryPoint>
<CLI>
Module CLI

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function

End Module
