imports "BackgroundTask" from "PipelineHost";

# title: Build indexed MSI cache
# author: xieguigang <xie.guigang@gcmodeller.org>

[@info "the file path of the imzML raw data file to create cache index."]
[@type "*.imzML"]
const imzML as string = ?"--imzML" || stop("no raw data file provided!");
[@info "the file path of the MSI indexed cache file."]
[@type "filepath"]
const cache as string = ?"--cache" || stop("a cache file path must be provided!");
[@info "intensity cutoff for removes noise data."]
const cutoff as double = ?"--into_cutoff" || 0.0;

sleep(1);
BackgroundTask::cache.MSI(imzML, cache, cutoff);