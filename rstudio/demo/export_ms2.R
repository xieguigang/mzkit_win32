require(mzkit);

imports "mzweb" from "mzkit";
imports "data" from "mzkit";

[@info "A directory path that contains multiple MS raw data file, example as mzXML, mzML"]
[@type "directory"]
const dir = ?"--dir" || stop("A source dir that contains the raw data file must be provided!");
const outfile = ?"--out" || `${dirname(dir)}/${basename(dir)}_peakMs2.csv`;
const raw = list.files(dir, pattern = ".*.mzX?ML", wildcard = FALSE);

let export_raw_ms2 = NULL;

print(basename(raw));

for(file in raw) {
    let rawdata = open.mzpack(file);
    let ms2 = mzweb::ms2_peaks(rawdata, centroid = TRUE, into.cutoff = 0.05);
    let mz = [ms2]::mz;
    let rt = [ms2]::rt;
    let into = [ms2]::intensity;
    let fragments = [ms2]::fragments;
    let matrix = sapply(ms2, function(m) {
        let mat = [m]::mzInto;
        let mz2 = round([mat]::mz, 4);
        let into2 = round([mat]::intensity * 100);
        let i = order(into2, decreasing = TRUE);

        paste(`${mz2}_${into2}`);
    });

    ms2 = data.frame(
        mz, rt, into, fragments, file = basename(file), ms2 = matrix, 
        row.names = make.ROI_names(
            list(mz, rt), name.chrs = TRUE
        )
    );

    # print(ms2);
    
    export_raw_ms2 = rbind(export_raw_ms2, ms2);

    # stop();

    invisible(NULL);
}

write.csv(export_raw_ms2, file = outfile, row.names = TRUE);