require(mzkit);

imports "MRMLinear" from "mz_quantify";

let MRMfiles = ?"--files" || stop("the input MRM mzXML files must be provided!");
let ions = ?"--ions" || stop("the MRM ions for extract peaks data must be provided!");
let outputdir = ?"--outdir" || dirname(MRMfiles);
let args = ?"--args" || stop("The peak finding arguments json must be provided!");

args = MRMLinear::from_arguments_json(readText(args));
MRMfiles = readLines(MRMfiles);
MRMfiles = as.list(MRMfiles, names = basename(MRMfiles));
ions = read.ion_pairs(ions) |> isomerism.ion_pairs(tolerance = "ppm:20");

print("get MRM rawdata files:");
str(MRMfiles);
print("view of the MRM arguments:");
str(JSON::json_deocde(readText(args)));

let xic = lapply(MRMfiles, path -> extract.ions(path, ionpairs = ions, tolerance = "da:0.01"));
let peaks = lapply(xic, ion -> MRM.peakarea(ion, args));
let peaktable = as.data.frame(unlist(peaks), peaktable = TRUE);
let peaktable_maxinto = as.data.frame(unlist(peaks), peaktable = TRUE, value = "maxinto");
let report = MRM_dataReport(xic, tpa = peaks);

MRMfiles = bind_rows(peaks |> lapply(i -> as.data.frame(i)));

print(MRMfiles, max.print = 6);
print("view your peaktable result:");
print(peaktable);

write.csv(MRMfiles, file = file.path(outputdir, "MRMIons.csv"), row.names = FALSE);
write.csv(t(peaktable), file = file.path(outputdir, "peaktable.csv"));
write.csv(t(peaktable_maxinto), file = file.path(outputdir, "intensity_table.csv"));

writeLines(report, con = file.path(outputdir, "report.html"));

