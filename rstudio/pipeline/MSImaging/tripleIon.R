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
const mzlist as double   = mz
|> strsplit(",", fixed = TRUE)
|> unlist()
|> as.numeric()
;

print("get a set of target RGB ions:");
print(mzlist);

const images  = lapply(mzlist, function(mz) {
	app::getMSIData(
        MSI_service = appPort, 
        mz          = mz, 
        mzdiff      = mzdiff
    ) 
    |> as.layer(context = mz)
    |> knnFill()
    ;
});

bitmap(file = savefile, size = [3300, 2000]) {
    
    # load mzpack/imzML raw data file
    # and config ggplot data source driver 
    # as MSImaging data reader
    ggplot(, padding = "padding: 200px 600px 200px 250px;") 
       # rendering of rgb channels ion m/z
       + geom_red(mz = images[[1]], tolerance = "da:0.3")
       + geom_green(mz = images[[2]], tolerance = "da:0.3")
       + geom_blue(mz = images[[3]], tolerance = "da:0.3")
	   + theme(panel.background = "black")
	   + MSI_knnfill()
       # add ggplot charting elements
       + ggtitle(`MS-Imaging of ${paste(mzlist, "+")}`)
       + labs(x = "Dimension(X)", y = "Dimension(Y)")
       + scale_x_continuous(labels = "F0")
       + scale_y_continuous(labels = "F0")
    ;
}