Imports Galaxy.Workbench.Actions

Public Class MetabonomicsAnalysisTool : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Do metabonomics analysis on the sample data."
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, table As DataTable)
        Call RibbonEvents.openLCMSWorkbench()
    End Sub
End Class
