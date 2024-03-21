Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM

<CLI>
Module CLI

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function

End Module
