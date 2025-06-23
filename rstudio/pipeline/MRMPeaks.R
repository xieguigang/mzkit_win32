require(mzkit);

let MRMfiles = ?"--files" || stop("the input MRM mzXML files must be provided!");
let ions = ?"--ions" || stop("the MRM ions for extract peaks data must be provided!");

MRMfiles = readLines(MRMfiles);

print("get MRM rawdata files:");
print(MRMfiles);

