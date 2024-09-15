imports "app" from "ServiceHub";

require(MSImaging);
require(mzkit);
require(ggplot);

options(memory.load = "max");

# request data from MSI service backend, and then do single ion ms-imaging plot

const appPort as integer = ?"--app"    || stop("A MSimaging services hub app handle must be provided!");
const mz as string       = ?"--mzlist" || stop("target ions list must be provided!");
const mzdiff as string   = ?"--mzdiff" || "da:0.1";
const savefile as string = ?"--save"   || stop("A file path to save plot image must be specificed!");
const title as string    = ?"--title"  || "";
const bg as string       = ?"--backcolor" || "black";
const overlap_totalIons as boolean = ?"--overlap-tic" || FALSE;
const filter_file as string = ?"--filters" || "";
const colorSet as string = ?"--colors" || "viridis:turbo";
const colorLevels as integer = ?"--levels" || 120;
const hostName as string = ?"--host" || "localhost";
const plot_size          = ?"--size" || "3300,2000";
const plot_dpi           = ?"--dpi"  || 120;
const plot_padding       = ?"--padding" || "padding: 200px 600px 200px 250px;";
const mzlist as double   = mz
|> strsplit(",", fixed = TRUE)
|> unlist()
|> as.numeric()
;
const pixelsData = app::getMSIData(
    MSI_service = appPort, 
    mz          = mzlist, 
    mzdiff      = mzdiff,
    host        = hostName
);
const dataPack = pixelPack(pixelsData, dims = app::getMSIDimensions(MSI_service = appPort));
const mz_tag as string = `m/z ${round(mzlist[1], 4)}`;
const totalIonLayer = {
    if (overlap_totalIons) {
        raster_blending(
            pixels = app::getTotalIons(appPort, host = hostName), 
            dims = as.object(dataPack)$GetDimensionSize(),
            scale = "gray",
            levels = 255
        );
    } else {
        NULL;
    }
}

print(`load ${length(pixelsData)} pixels data from given m/z:`);
print(mzlist);

let filetype = file.ext(savefile);
let msi_filters = {
    if (file.exists(filter_file)) {
        print("apply of the image filter from config file:");
        print(filter_file);
        
        geom_MSIfilters(file = filter_file);
    } else {
        geom_MSIfilters(
            denoise_scale() > TrIQ_scale(0.85) > knn_scale() > soften_scale()
        );
    }
}
let make_plot = function() {
    # load mzpack/imzML raw data file
    # and config ggplot data source driver 
    # as MSImaging data reader
    ggplot(dataPack, 
           mapping = aes(), 
           padding = plot_padding
    ) 
       # rendering of a single ion m/z
       # default color palette is Jet color set
       + geom_msimaging(
		    mz        = mzlist[1], 
			tolerance = mzdiff, 
			color     = colorSet,
            pixel_render = TRUE,
            raster = totalIonLayer,
            colorLevels = colorLevels
	   )
       + msi_filters
	   + geom_MSIbackground(bg)
       # add ggplot charting elements
       + ggtitle(`MSImaging of ${ifelse(title == "", mz_tag, title)}`)
       + labs(x = "Dimension(X)", y = "Dimension(Y)")
       + scale_x_continuous(labels = "F0")
       + scale_y_continuous(labels = "F0")
       + theme(panel.grid = element_blank())
    ;
}

if (filetype == "svg") {
    svg(file = savefile, size = as.integer(unlist(strsplit(plot_size, ","))), dpi = plot_dpi) {
        make_plot();
    }
} else {
    bitmap(file = savefile, size = as.integer(unlist(strsplit(plot_size, ","))), dpi = plot_dpi) {
        make_plot();
    }
}