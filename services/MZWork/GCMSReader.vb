Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports CDF.PInvoke
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector

Public Module GCMSReader

    Public Function LoadAllMemory(file As String) As GCMSnetCDF
        Dim nc As New DataReader(file)
        Dim error_log As String = DirectCast(nc.GetData("error_log"), chars).ToArray.CharString
        Dim a_d_sampling_rate As doubles = nc.GetData("a_d_sampling_rate")
        Dim a_d_coaddition_factor As shorts = nc.GetData("a_d_coaddition_factor")
        Dim scan_acquisition_time As doubles = nc.GetData("scan_acquisition_time")
        Dim scan_duration As doubles = nc.GetData("scan_duration")
        Dim inter_scan_time As doubles = nc.GetData("inter_scan_time")
        Dim resolution As doubles = nc.GetData("resolution")
        Dim actual_scan_number As integers = nc.GetData("actual_scan_number")
        Dim total_intensity As doubles = nc.GetData("total_intensity")
        Dim mass_range_min As doubles = nc.GetData("mass_range_min")
        Dim mass_range_max As doubles = nc.GetData("mass_range_max")
        Dim time_range_min As doubles = nc.GetData("time_range_min")
        Dim time_range_max As doubles = nc.GetData("time_range_max")
        Dim scan_index As integers = nc.GetData("scan_index")
        Dim point_count As integers = nc.GetData("point_count")
        Dim flag_count As integers = nc.GetData("flag_count")
        Dim mass_values As shorts = nc.GetData("mass_values")
        Dim intensity_values As floats = nc.GetData("intensity_values")

        Return New GCMSnetCDF With {
            .actual_scan_number = actual_scan_number,
            .a_d_coaddition_factor = a_d_coaddition_factor,
            .a_d_sampling_rate = a_d_sampling_rate,
            .error_log = error_log,
            .flag_count = flag_count,
            .intensity_values = intensity_values,
            .inter_scan_time = inter_scan_time,
            .mass_range_max = mass_range_max,
            .mass_range_min = mass_range_min,
            .mass_values = mass_values,
            .point_count = point_count,
            .resolution = resolution,
            .scan_acquisition_time = scan_acquisition_time,
            .scan_duration = scan_duration,
            .scan_index = scan_index,
            .time_range_max = time_range_max,
            .time_range_min = time_range_min,
            .total_intensity = total_intensity
        }
    End Function
End Module
