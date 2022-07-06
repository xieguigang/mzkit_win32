imports "app" from "ServiceHub";

require(MSImaging);
require(mzkit);
require(ggplot);
require(JSON);

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

const size   = [2700, 2000];
const images = lapply(mzSet, function(ion) {
	let mz    = as.numeric(ion$mz);
    let layer = app::getMSIData(
        MSI_service = appPort, 
        mz          = mz, 
        mzdiff      = mzdiff
    ) 
    |> as.layer(context = mz)
    ;

    ion$layer = layer;
    ion;
});

bitmap(file = savefile, size = size);

images |> PlotMSIMatrixHeatmap(
    layout        = [3,3],
    colorSet      = "Jet",
    MSI_TrIQ      = 0.8,
    size          = size, 
    canvasPadding = [50, 300, 50, 50], 
    cellPadding   = [200, 100, 0, 100]
);

dev.off();