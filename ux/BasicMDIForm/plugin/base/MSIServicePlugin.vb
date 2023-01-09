Public Interface MSIServicePlugin

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="filepath">
    ''' should be a file path of mzPack file, not working for other data format
    ''' </param>
    ''' <param name="message"></param>
    Sub LoadMSIRawDataFile(filepath As String, message As Action(Of String))

End Interface
