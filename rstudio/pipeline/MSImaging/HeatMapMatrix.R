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

const images = lapply(mzSet, function(mz) {
	mz = as.numeric(mz);

    app::getMSIData(
        MSI_service = appPort, 
        mz          = mz, 
        mzdiff      = mzdiff
    );
});

bitmap(file = savefile, size = [3300, 2000]);



dev.off();