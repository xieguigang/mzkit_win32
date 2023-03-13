imports "BackgroundTask" from "PipelineHost";

require(GCModeller);

# title: Pipeline for run ms1 ions peakset annotation based on Mummichog annotation algorithm
# author: xieguigang <xie.guigang@gcmodeller.org>

[@info "the file path of the mzPack/mgf raw data file to create cache index. And 
        also this argument file could be a set of the ms1 ions m/z values."]
[@type "*.mzpack;*.mgf;*.txt"]
const raw as string = ?"--raw" || stop("no raw data file provided!");
[@info "the directory path of the Mummichog output result files."]
[@type "directory"]
const outputdir as string = ?"--save" || stop("a directory path for save result files must be provided!");
const ssid as string = ?"--biodeep_ssid" || stop("a session id for biodeep is required!");
[@info "the input raw data is a set of ion peaks ms1 ion m/z values?"]
const peaksMz as boolean = ?"--mz-peaks" || FALSE;

sleep(1);
BackgroundTask::biodeep.session(ssid);

if (!peaksMz) {
    BackgroundTask::Mummichog(raw, outputdir);
} else {
    raw 
    |> readLines()
    |> as.numeric()
    |> BackgroundTask::Mummichog(outputdir)
    ;
}
