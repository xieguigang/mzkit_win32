Public Interface ITaskProgress

    Sub SetProgressMode()
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="p">[0,100]</param>
    Sub SetProgress(p As Integer)
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="p">[0,100]</param>
    ''' <param name="msg"></param>
    Sub SetProgress(p As Integer, msg As String)
    Sub SetTitle(title As String)
    Sub SetInfo(message As String)
    Sub TaskFinish()

End Interface