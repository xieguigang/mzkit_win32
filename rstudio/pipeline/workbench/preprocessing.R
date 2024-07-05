require(mzkit);

# tool script for make peaktable data pre-processing

let sampleinfo = ?"--sampleinfo" || stop("missing sampleinfo table file for run peaktable data pre-processing!");
let peaktable  = ?"--peaktable"  || stop("the raw matrix data file is missing!");
let norm_scale = ?"--scale_factor" || 1e8;
let impute_cutoff = ?"--missing" || 0.5;
let export_file = ?"--output" || file.path(dirname(peaktable), `${basename(peaktable)}_processed.csv`);
let input_file = peaktable;

sampleinfo <- read.csv(sampleinfo, row.names = NULL, check.names = FALSE);
# binary/csv/txt
peaktable  <- read.xcms_peaks(peaktable, tsv = file.ext(peaktable) != "csv");
peaktable  <- mzkit::preprocessing_expression(peaktable, 
    sampleinfo = sampleinfo, 
    factor = as.numeric(norm_scale), missing = impute_cutoff
);

if (file.ext(input_file) == "xcms") {
    # export binary peaktable 
    writeBin(peaktable, con = export_file);   
} else {
    # just export the normalized peaktable in csv file
    write.xcms_peaks(peaktable, file = export_file);
}

