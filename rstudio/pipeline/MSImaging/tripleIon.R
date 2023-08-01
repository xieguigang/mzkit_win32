require(MSImaging);
require(mzkit);
require(ggplot);
require(JSON);

imports "app" from "ServiceHub";
imports "MSI" from "mzkit";
imports "MsImaging" from "mzplot";

options(memory.load = "max");

const appPort as integer = ?"--app"     || stop("A MSimaging services hub app handle must be provided!");
const mz as string       = ?"--mzlist"  || stop("target ions list must be provided!");
const mzdiff as string   = ?"--mzdiff"  || "da:0.1";
const savefile as string = ?"--save"    || stop("A file path to save plot image must be specificed!");
const knnFill as integer = ?"--knnFill" || 3;
const overlap_totalIons as boolean = ?"--overlap-tic" || FALSE;
const mzlist as double   = mz
|> strsplit(",", fixed = TRUE)
|> unlist()
|> as.numeric()
;

print("get a set of target RGB ions:");
print(mzlist);

const dims = app::getMSIDimensions(MSI_service = appPort);
const images  = lapply(mzlist, function(mz) {
	app::getMSIData(
        MSI_service = appPort, 
        mz          = mz, 
        mzdiff      = mzdiff
    ) 
    |> as.layer(context = mz, strict = FALSE)
    |> knnFill()
    ;
});
const mz_keys = `m/z ${round( mzlist, 4)}`;
const kr = mz_keys[1];
const kg = mz_keys[2];
const kb = mz_keys[3];

names(images) = mz_keys;

print("view of the images data:");
str(images);

bitmap(file = savefile, size = [3300, 2000]) {
    
    # load mzpack/imzML raw data file
    # and config ggplot data source driver 
    # as MSImaging data reader

    # rendering of rgb channels ion m/z
    ggplot(MSIheatmap(
        R = images[[kr]], 
        G = images[[kg]], 
        B = images[[kb]],
        dims = dims
    ), padding = "padding: 200px 600px 200px 250px;") 
       
	   + theme(panel.background = "black")
       + geom_msiheatmap()
	   # + geom_MSIfilters(
            # denoise_scale() > TrIQ_scale(0.8) > knn_scale(knnFill, 0.5) > soften_scale()
        # )
       # add ggplot charting elements
       + ggtitle(`MS-Imaging of ${paste(round(mzlist, 3), "+")}`)
       + labs(x = "Dimension(X)", y = "Dimension(Y)")
       + scale_x_continuous(labels = "F0")
       + scale_y_continuous(labels = "F0")
       + theme(panel.grid = element_blank())
    ;
}