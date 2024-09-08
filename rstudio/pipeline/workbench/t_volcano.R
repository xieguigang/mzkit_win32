require(ggplot);
require(GCModeller);

# t-test and volcano plot of two sample group comparision
let rawdata = ?"--rawdata" || stop("matrix file for the data analysis and plot is required!");
let sampleinfo = ?"--sampleinfo" || stop("sample group information is required for defined the data comparison source.");
let trial = ?"--trial" || stop("group name of trial is required");
let control = ?"--control" || stop("group name of the control is required");
let output_dir = ?"--output_dir" || dirname(rawdata);

