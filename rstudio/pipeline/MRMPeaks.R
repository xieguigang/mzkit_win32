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

MRMfiles = lapply(MRMfiles, function(path) {
    let xic = extract.ions(path, ionpairs = ions, tolerance = "da:0.01");
    let peaks = xic |> MRM.peakarea(args);

    peaks= as.data.frame(peaks);
    peaks[,"name"] = rownames(peaks);
    peaks[,"source"] = basename(path);
    peaks;
});

MRMfiles = bind_rows(MRMfiles);

let 

print(MRMfiles);

write.csv(MRMfiles, file = file.path(outputdir, "MRMIons.csv"));