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
const overlap_totalIons as boolean = ?"--overlap-tic" || FALSE;
const hostName as string = ?"--host" || "localhost";
const filter_file as string = ?"--filters" || ""; 

# config plot parameters
let plot_size    = ?"--size" || "3300,2000";
let plot_dpi     = ?"--dpi"  || 120;
let plot_padding = ?"--padding" || "padding: 200px 600px 200px 250px;";
let mzlist       = mz
|> base64_decode(asText.encoding="utf8")
|> JSON::json_decode()
;

print("inspect the configuration of target RGB ions ms-imaging:");
str(mzlist);

mzlist$mode <- NULL;

const dims   = app::getMSIDimensions(MSI_service = appPort);
const images = lapply(mzlist, function(mz) {
	app::getMSIData(
        MSI_service = appPort, 
        mz          = as.numeric(mz$"m/z"), 
        mzdiff      = mzdiff
    ) 
    |> as.layer(context = mz$annotation, strict = FALSE)
    # |> knnFill()
    ;
});
const totalIonLayer = {
    if (overlap_totalIons) {
        raster_blending(
            pixels = app::getTotalIons(appPort, host = hostName), 
            dims = dims,
            scale = "gray",
            levels = 255
        );
    } else {
        NULL;
    }
}
# const mz_keys = `m/z ${round( mzlist, 4)}`;
# const kr = mz_keys[1];
# const kg = mz_keys[2];
# const kb = mz_keys[3];

# names(images) = mz_keys;

print("view of the images data:");
str(images);

let filetype = file.ext(savefile);
let msi_filters = {
    if (file.exists(filter_file) && (length(readLines(filter_file)) > 0)) {
        geom_MSIfilters(file = filter_file);
    } else {
        # just use the default intensity filter
        geom_MSIfilters(
            TrIQ_scale(0.95)
        );
    }
}

#' load mzpack/imzML raw data file
#' and config ggplot data source driver 
#' as MSImaging data reader
let make_plot = function(mzlist) { 
    ggplot(
        # rendering of rgb channels ion m/z
        MSIheatmap(
            R = images$r, 
            G = images$g, 
            B = images$b,
            dims = dims
    ), padding = plot_padding) 
       
	   + theme(panel.background = "black")
       + geom_msiheatmap()
	   # + geom_MSIfilters(
            # denoise_scale() > TrIQ_scale(0.8) > knn_scale(knnFill, 0.5) > soften_scale()
        # )
       + msi_filters
       # add ggplot charting elements
       + ggtitle(`MS-Imaging of ${mzlist}`)
       + labs(x = "Dimension(X)", y = "Dimension(Y)")
       + scale_x_continuous(labels = "F0")
       + scale_y_continuous(labels = "F0")
       + theme(panel.grid = element_blank())
    ;
}

plot_size <- as.integer(unlist(strsplit(plot_size, ",")));
mzlist <- round(as.numeric(sapply(mzlist, i -> i$"m/z")), 3);
mzlist <- paste(mzlist, sep = "+", collapse= " ");

print(msi_filters);
print("plot size of the rgb ions heatmap:");
print(plot_size);
print(mzlist);

switch(filetype, default -> stop(`invalid file type of plot output: ${filetype}`)) {
    svg = {
        svg(file = savefile, size = plot_size, dpi = plot_dpi) {
            make_plot(mzlist);
        }
    },
    png = {
        bitmap(file = savefile, size = plot_size, dpi = plot_dpi) {
            make_plot(mzlist);
        }
    },
    pdf = {
        pdf(file = savefile, size = plot_size, dpi = plot_dpi) {
            make_plot(mzlist);
        }
    }
}