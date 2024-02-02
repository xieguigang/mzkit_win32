require(MSImaging);
require(mzkit);
require(ggplot);
require(JSON);

imports "app" from "ServiceHub";
imports "MSI" from "mzkit";
imports "MsImaging" from "mzplot";

options(memory.load = "max");

const appPort as integer     = ?"--app"    || stop("A MSimaging services hub app handle must be provided!");
const mz as string           = ?"--mzlist" || stop("target ions list must be provided!");
const mzdiff as string       = ?"--mzdiff" || "da:0.1";
const savefile as string     = ?"--save"   || stop("A file path to save plot image must be specificed!");
const colorPalette as string = ?"--scaler" || "viridis:turbo";
const size_str as string     = ?"--size"   || "2800,2100";
const layout_str as string   = ?"--layout" || "3,3";

const mzSet = mz 
|> readText() 
|> json_decode()
|> lapply(function(ion) {
    ion$mz = round(as.numeric(ion$mz), 3);
    ion;
})
;

str(mzSet);

const padding = [50, 450, 50, 50];
const layout  = strsplit(layout_str, ",") |> unlist() |> as.integer();
const size    = strsplit(size_str, ",") |> unlist() |> as.integer();
const images  = lapply(mzSet, function(ion) {
	let mz    = as.numeric(ion$mz);
    let layer = app::getMSIData(
        MSI_service = appPort, 
        mz          = mz, 
        mzdiff      = mzdiff
    ) 
    |> as.layer(context = mz, strict = FALSE)
    ;

    ion$layer = layer;
    ion;
});

bitmap(
    file    = savefile,
    size    = size, 
    padding = `padding: ${padding[1]}px ${padding[2]}px ${padding[3]}px ${padding[4]}px;`, 
    fill    = "black"
);

#' images    
#' 
#' 1. type  precursor type information string
#' 2. title   the ion metabolite name
#' 3. layer   the MSI ion layer data
#' 4. mz      the target ion m/z value
#' 
images |> PlotMSIMatrixHeatmap(
    layout        = layout,
    colorSet      = colorPalette,
    MSI_TrIQ      = 0.85,
    size          = size, 
    canvasPadding = padding, 
    cellPadding   = [200, 100, 0, 100],
    strict        = FALSE,
    gaussian      = 0
);

dev.off();