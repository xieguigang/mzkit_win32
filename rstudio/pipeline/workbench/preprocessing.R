require(mzkit);

# tool script for make peaktable data pre-processing

let sampleinfo = ?"--sampleinfo" || stop("missing sampleinfo table file for run peaktable data pre-processing!");
let peaktable  = ?"--peaktable"  || stop("the raw matrix data file is missing!");
let norm_scale = ?"--scale_factor" || 1e8;
let impute_cutoff = ?"--missing" || 0.5;
let export_file = ?"--output" || file.path(dirname(peaktable), `${basename(peaktable)}_processed.csv`);

sampleinfo <- read.csv(sampleinfo, row.names = NULL, check.names = FALSE);
peaktable  <- read.xcms_peaks(peaktable);
peaktable  <- mzkit::preprocessing_expression(peaktable, 
    sampleinfo = sampleinfo, 
    factor = as.numeric(norm_scale), missing = impute_cutoff
);

write.xcms_peaks(peaktable, file = export_file);
