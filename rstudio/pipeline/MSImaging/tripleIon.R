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

    # rendering of rgb channels ion m/z
    ggplot(MSIheatmap(
        R = images[[1]], 
        G = images[[2]], 
        B = images[[3]]
    ), padding = "padding: 200px 600px 200px 250px;") 
       
	   + theme(panel.background = "black")
       + geom_msiheatmap()
	   + MSI_knnfill()
       # add ggplot charting elements
       + ggtitle(`MS-Imaging of ${paste(round(mzlist, 3), "+")}`)
       + labs(x = "Dimension(X)", y = "Dimension(Y)")
       + scale_x_continuous(labels = "F0")
       + scale_y_continuous(labels = "F0")
    ;
}