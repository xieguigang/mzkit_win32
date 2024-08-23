imports "BackgroundTask" from "PipelineHost";

# script for run deconvolution of the LC-MS rawdata files

[@info "the rawdata files input, value could be:
   1. the file path of a single rawdata file, data format could be mzXML/mzML/mzPack;
   2. the directory path that contains multiple rawdata files, file data formats inside the directory could be mzXML/mzML/mzPack;
   3. a txt file that contains the filepath list of the rawdata files, each line should be a file path."]
let raw as string = ?"--raw" || stop("a raw data file in mzpack format must be provided!");

[@info "the csv file path of the peaktable outputs."]
const savepath as string  = ?"--save"      || stop("A file path of the table data output must be provided!");

[@info "the mass tolerance error for extract the XIC data."]
const massDiff as double  = ?"--massdiff"  || 0.005;

[@info "the rt window size for the peaks output, min rt windows and max rt windows."]
const rt_win   as string  = ?"--rt_win"    || "3,15";

[@info "the threads number for run the parallel, 
        default configuration use 8 cpu threads."]
const threads  as integer = ?"--n_threads" || 8;

let is_batch = function() {
    if (dir.exists(raw)) {
        TRUE;
    } else {
        file.ext(raw) == "txt";
    }
}

if (is_batch()) {
    # processing of batch data
    # use the mzkit package api
    if (!require(mzkit)) {
        stop("mzkit package is not installed correctly, try to run windows batch script for repairs of the Rstudio environment.");
    }
    if (!dir.exists(raw)) {
        raw <- readLines(raw);
    }

    mzkit::run.Deconvolution(
        rawdata = raw, 
        outputdir = dirname(savepath), 
        mzdiff = 0.001, xic_mzdiff = massDiff,
        peak.width = as.integer(unlist(strsplit(rt_win, ","))),
        n_threads = threads,
        filename = basename(savepath,
            withExtensionName = TRUE, 
            strict = FALSE)
    );
} else {
    raw 
    |> MS1deconv(massDiff)
    |> write.csv(file = savepath, row.names = TRUE)
    ;
}

