Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports CDF.PInvoke

Public Module GCMSReader

    Public Function LoadAllMemory(file As String) As GCMSnetCDF
        Dim nc As New DataReader(file)
        Dim error_log = nc.GetData("error_log")
        Dim a_d_sampling_rate = nc.GetData("a_d_sampling_rate")
        Dim a_d_coaddition_factor = nc.GetData("a_d_coaddition_factor")
        Dim scan_acquisition_time = nc.GetData("scan_acquisition_time")
        Dim scan_duration = nc.GetData("scan_duration")
        Dim inter_scan_time = nc.GetData("inter_scan_time")
        Dim resolution = nc.GetData("resolution")
        Dim actual_scan_number = nc.GetData("actual_scan_number")
        Dim total_intensity = nc.GetData("total_intensity")
        Dim mass_range_min = nc.GetData("mass_range_min")
        Dim mass_range_max = nc.GetData("mass_range_max")
        Dim time_range_min = nc.GetData("time_range_min")
        Dim time_range_max = nc.GetData("time_range_max")
        Dim scan_index = nc.GetData("scan_index")
        Dim point_count = nc.GetData("point_count")
        Dim flag_count = nc.GetData("flag_count")
        Dim mass_values = nc.GetData("mass_values")
        Dim intensity_values = nc.GetData("intensity_values")


        Return New GCMSnetCDF
    End Function
End Module
