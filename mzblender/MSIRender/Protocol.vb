Public Enum Protocol
    ''' <summary>
    ''' Open a new session and load new MSI pixel layer data
    ''' </summary>
    OpenSession
    SetFilters
    SetHEmap
    SetSampleTag
    SetIntensityRange
    GetTrIQIntensity
    ''' <summary>
    ''' Rendering a new MS-imaging layer based on new parameters
    ''' </summary>
    MSIRender
    ''' <summary>
    ''' Destroy the current MSI blender session
    ''' </summary>
    Destroy
    ''' <summary>
    ''' Shutdown the blender server
    ''' </summary>
    Shutdown
End Enum
