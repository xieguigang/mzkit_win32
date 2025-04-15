require(MSImaging);

let map_ions = ?"--ions" || stop("A dataframe that contains the ions for make charting plot is not provided!");
let sampleinfo = ?"--sampleinfo" || stop("A dataframe that contains the sample group information must be provided!");
let rawdata = ?"--rawdata" || stop("A file path to the MS-Imaging rawdata file in mzPack file format must be provided!");
let outputdir = ?"--outputdir" || dirname(map_ions);
let ions = read.csv(map_ions, row.names = FALSE, check.names = FALSE);

rawdata = open.mzpack(rawdata);
sampleinfo = read.csv(sampleinfo, row.names = FALSE, check.names = FALSE);
sampleinfo = data.frame(
    id = sampleinfo$ID,
    group = sampleinfo$sample_info,
    color = sampleinfo$color
);

ions[,"name"]=NULL;
ions[,"precursorType"]=NULL;
ions[,"kegg_id"]=NULL;

print(sampleinfo);

let tic = MSI_sampleTIC(rawdata, dims = NULL, filters = default_MSIfilter());

for(let met in as.list(ions,byrow=TRUE)) {
    let mz = met$mz;
    let title = met$key;

    met$mz=NULL;
    met$key = NULL;

    let sampleSet = as.data.frame(sampleinfo);

    print(title);

    rawdata |> MSI_ionStatPlot(
        mz = mz, met = met, sampleinfo = sampleSet , 
        savePng        = file.path(outputdir, `${title |> normalizeFileName(FALSE,".", maxchars=72)}.png` ), 
        ionName        = NULL,
        size           = [1800, 1200], 
        colorMap       = NULL, 
        MSI_colorset   = "viridis:turbo",
        ggStatPlot     = function(colorMap) {
            geom_violin(color = colorMap,
                width = 0.8,
                alpha = 0.8
            );
        }, 
        font_family    = "Cambria",
        padding_top    = 80,
        padding_right  = 80,
        padding_bottom = 200,
        padding_left   = 80,
        interval       = 300,
        combine_layout = [5, 5], 
        jitter_size    = 4.5, 
        title_fontsize = 100, 
        TrIQ           = 0.8,
        backcolor      = "black", 
        regions        = NULL, 
        swap           = TRUE,
        show_legend    = FALSE,
        show_grid      = FALSE,
        show_stats     = FALSE,
        show_axis.msi  = FALSE,
        show_title     = FALSE,
        intensity_format = "F1",
        intensity_axis   = FALSE,
        plot_daErr     = 0.01,
        tic_outline    = tic);
}