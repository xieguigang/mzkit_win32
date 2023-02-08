Public Module TaskEngine

    Public ReadOnly Property hostDll As String = $"{App.HOME}/Rstudio/host/PipelineHost.dll".GetFullPath
    Public ReadOnly Property redisDll As String = $"{App.HOME}/Rstudio/host/MZWorkRedis.dll".GetFullPath

End Module
