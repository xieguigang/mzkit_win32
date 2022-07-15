require(MSImaging);
require(mzkit);
require(ggplot);
require(JSON);

imports "app" from "ServiceHub";
imports "MSI" from "mzkit";
imports "MsImaging" from "mzplot";

options(memory.load = "max");

const appPort as integer = ?"--app"    || stop("A MSimaging services hub app handle must be provided!");
const mz as string       = ?"--mzlist" || stop("target ions list must be provided!");
const mzdiff as string   = ?"--mzdiff" || "da:0.1";
const savefile as string = ?"--save"   || stop("A file path to save plot image must be specificed!");

const mzSet = mz 
|> readText() 
|> json_decode()
;

str(mzSet);

const padding = [50, 450, 50, 50];
const size    = [2800, 2100];
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
    padding = `padding: ${canvasPadding[1]}px ${canvasPadding[2]}px ${canvasPadding[3]}px ${canvasPadding[4]}px;`, 
    fill    = "black"
);

images |> PlotMSIMatrixHeatmap(
    layout        = [3,3],
    colorSet      = "rainbow", # "viridis:turbo",
    MSI_TrIQ      = 0.8,
    size          = size, 
    canvasPadding = padding, 
    cellPadding   = [200, 100, 0, 100],
    strict        = FALSE
);

dev.off();