require(mzkit);

#' biodeep mzweb data viewer raw data file helper
imports "mzweb" from "mzkit";

let file = "C:\Program Files\BioNovoGene\mzkit_win32\demo\003_Ex2_Orbitrap_CID.mzXML";
let ms2_spectrum = open.mzpack(file) |> ms2_peaks(
    centroid = TRUE,
    norm = FALSE,
    filter.empty = TRUE,
    into.cutoff = 0);

print(ms2_spectrum);