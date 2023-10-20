imports "app" from "ServiceHub";

require(MSImaging);
require(mzkit);
require(ggplot);

options(memory.load = "max");
options(strict = FALSE);

const rawdata as string   = ?"--data"   || stop("no raw data provided!");
const plot_type as string = ?"--plot"   || stop("should be one of the 'box', 'bar', 'violin'");
const appPort as integer  = ?"--app"    || stop("A MSimaging services hub app handle must be provided!");
const mz as string        = ?"--mzlist" || stop("target ions list must be provided!");
const mzdiff as string    = ?"--mzdiff" || "da:0.1";
const savefile as string  = ?"--save"   || stop("A file path to save plot image must be specificed!");
const title as string     = ?"--title"  || NULL;
const bg as string        = ?"--backcolor" || "black";
const colorSet as string  = ?"--colors" || "viridis:turbo";
const show_tissue as boolean = ?"--show-tissue" || FALSE;
const mzlist as double    = mz
|> strsplit(",", fixed = TRUE)
|> unlist()
|> as.numeric()
;
const pixelsData = app::getMSIData(
    MSI_service = appPort, 
    mz          = mzlist, 
    mzdiff      = mzdiff
);
const mz_tag as string = `m/z ${round(mzlist[1], 4)}`;
const mzpack = pixelPack(pixelsData, dims = app::getMSIDimensions(MSI_service = appPort));

# region_label -> [color, data, x, y]
let myeloma = rawdata
|> readText()
|> json_decode()
;

const intensity_data = list();
const sampleinfo = list();
const sample_color = lapply(myeloma, c -> c$color);

let x = [];
let y = [];
let colors = [];

str(myeloma);

for(name in names(myeloma)) {
    let part = myeloma[[name]];
    let color = part$color;
    let group_id = `${name}_${1:length(part$data)}`;
    let nsize = length(part$x);

    x = append(x, part$x);
    y = append(y, part$y);
    colors = append(colors, rep(color, nsize));
	part = part$data;
	sampleinfo[[name]] = {
        group: name,
        id: group_id,
        color: color
    };
    
    for(i in 1:length(group_id)) {
        intensity_data[[(group_id[i])]] = part[i];
    }
}

print(`load ${length(pixelsData)} pixels data from given m/z:`);
print(mzlist);

const getGgplot = function() {
	if (plot_type == "box") {
		geom_boxplot(width = 0.65, alpha = 0.85, color = sample_color);
	} else if (plot_type == "bar") {
		geom_barplot(width = 0.65, alpha = 0.85, color = sample_color);
	} else {
		geom_violin(width = 0.65, alpha = 0.85, color = sample_color);
	}
}
const tissue_regions = {
    if (show_tissue) {
        data.frame(x,y,colors);
    } else {
        NULL;
    }
}

MSImaging::MSI_ionStatPlot(
    mzpack         = mzpack, 
    mz             = mzlist, 
    met            = intensity_data, 
    sampleinfo     = sampleinfo, 
    savePng        = savefile, 
    ionName        = title,
    size           = [2400, 1000], 
    colorMap       = NULL, 
    MSI_colorset   = colorSet,
    ggStatPlot     = getGgplot, 
    padding_top    = 150,
    padding_right  = 200,
    padding_bottom = 150,
    padding_left   = 150,
    interval       = 50,
    combine_layout = [4, 5], 
    jitter_size    = 8, 
    TrIQ           = 0.65,
    backcolor      = bg,
    regions        = tissue_regions
)
;