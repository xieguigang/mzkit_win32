Public Class frmLogFile

    Protected Overrides Sub OpenContainingFolder()
        Call Process.Start("explorer.exe", App.ProductProgramData)
    End Sub

    Protected Overrides Sub CopyFullPath()
        Call Clipboard.Clear()
        Call Clipboard.SetText(App.ProductProgramData)
    End Sub

End Class