imports "BackgroundTask" from "PipelineHost";

# script for run deconvolution of the LC-MS rawdata files

const raw      as string  = ?"--raw"       || stop("a raw data file in mzpack format must be provided!");
const savepath as string  = ?"--save"      || stop("A file path of the table data output must be provided!");
const massDiff as double  = ?"--massdiff"  || 0.005;
[@info "the rt window size for the peaks output, min rt windows and max rt windows."]
const rt_win   as string  = ?"--rt_win"    || "3,15";
[@info "the threads number for run the parallel, default configuration is 8 cpu threads."]
const threads  as integer = ?"--n_threads" || 8;

if (dir.exists(raw)) {
    # processing of batch data
    # use the mzkit package api
    if (!require(mzkit)) {
        stop("mzkit package is not installed correctly, try to run windows batch script for repairs of the Rstudio environment.");
    }

    mzkit::run.Deconvolution(
        rawdata = raw, 
        outputdir = dirname(savepath), 
        mzdiff = 0.001, xic_mzdiff = massDiff,
        peak.width = as.integer(unlist(strsplit(rt_win, ","))),
        n_threads = threads,
        filename = file.info(savepath)$Name
    );
} else {
    raw 
    |> MS1deconv(massDiff)
    |> write.csv(file = savepath, row.names = TRUE)
    ;
}

