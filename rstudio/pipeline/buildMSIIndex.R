imports "BackgroundTask" from "PipelineHost";

# title: Build indexed MSI cache
# author: xieguigang <xie.guigang@gcmodeller.org>

[@info "the file path of the imzML raw data file to create cache index, or a valid direcotry 
        path that contains multiple imzML rawdata files if running in batch mode."]
[@type "*.imzML"]
const imzML as string = ?"--imzML" || stop("no raw data file provided!");
[@info "running the conversion in batch mode? this parameter required of the input ``--imzML`` 
        source parameter should be a directory path, and the export ``--cache`` path also 
        could be a directory path if it has been specificed."]
const in_batch as boolean = ?"--batch" || FALSE;
[@info "intensity cutoff for removes noise data."]
const cutoff as double = ?"--into_cutoff" || 0.0;
[@info "make the spectrum in each spot centroid?"]
const make_centroid as boolean = ?"--centroid" || FALSE;

[@info "the file path of the MSI indexed cache file."]
[@type "filepath"]
const cache_handle as string = ?"--cache" || NULL;
const cache = 
{
    if (!in_batch) {
        (cache_handle || stop("a cache file path must be provided!"));
    } else {
        # data file will be export to the source directory
        # if this cache output parameter has not been specificed
        (cache_handle || imzML);
    }
};

sleep(1);

if (in_batch) {
    let rawfiles = list.files(imzML, pattern = ["*.imzML"]);

    print(`get ${length(rawfiles)} imzML source inputs:`);
    print(basename(rawfiles));

    for(file in rawfiles) {
        BackgroundTask::cache.MSI(
            file, `${cache}/${basename(file)}.mzPack`, 
            cutoff
        );
    }
} else {
    BackgroundTask::cache.MSI(imzML, cache, cutoff, 
        make_centroid = make_centroid );
}