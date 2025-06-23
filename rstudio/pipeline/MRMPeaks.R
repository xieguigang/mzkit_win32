require(mzkit);

imports "MRMLinear" from "mz_quantify";

let MRMfiles = ?"--files" || stop("the input MRM mzXML files must be provided!");
let ions = ?"--ions" || stop("the MRM ions for extract peaks data must be provided!");
let outputdir = ?"--outdir" || dirname(MRMfiles);

MRMfiles = readLines(MRMfiles);
ions = read.ion_pairs(ions) |> isomerism.ion_pairs(tolerance = "ppm:20");

print("get MRM rawdata files:");
print(MRMfiles);

MRMfiles = lapply(MRMfiles, function(path) {
    let xic = extract.ions(path, ionpairs = ions, tolerance = "da:0.01");
});