require(mzkit);

imports "MRMLinear" from "mz_quantify";

let MRMfiles = ?"--files" || stop("the input MRM mzXML files must be provided!");
let ions = ?"--ions" || stop("the MRM ions for extract peaks data must be provided!");
let outputdir = ?"--outdir" || dirname(MRMfiles);
let args = MRM.arguments();

MRMfiles = readLines(MRMfiles);
MRMfiles = as.list(MRMfiles, names = basename(MRMfiles));
ions = read.ion_pairs(ions) |> isomerism.ion_pairs(tolerance = "ppm:20");

print("get MRM rawdata files:");
print(MRMfiles);

let xic = lapply(MRMfiles, path -> extract.ions(path, ionpairs = ions, tolerance = "da:0.01"));
let peaks = lapply(xic, ion -> as.data.frame(MRM.peakarea(ion, args)));

MRMfiles = bind_rows(peaks);

print(MRMfiles);

write.csv(MRMfiles, file = file.path(outputdir, "MRMIons.csv"));