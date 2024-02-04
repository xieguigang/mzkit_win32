Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports CDF.PInvoke

Public Module GCMSReader

    Public Function LoadAllMemory(file As String) As GCMSnetCDF
        Dim nc As New DataReader(file)

        Return New GCMSnetCDF
    End Function
End Module
