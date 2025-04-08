require(MSImaging);

setwd(@dir);

let ions = read.csv("./map00020.csv", row.names = FALSE, check.names = FALSE);
let sampleinfo = read.csv("./sampleinfo.csv", row.names = FALSE, check.names = FALSE);

sampleinfo = data.frame(
    id = sampleinfo$ID,
    group = sampleinfo$sample_info,
    color = sampleinfo$color
);

# ions[,"mz"]=NULL;
ions[,"name"]=NULL;
ions[,"precursorType"]=NULL;
ions[,"kegg_id"]=NULL;

print(sampleinfo);

let rawdata = open.mzpack("./HK_vs_HKN.mzPack");
let tic = MSI_sampleTIC(rawdata, dims = NULL, filters = default_MSIfilter());

for(let met in as.list(ions,byrow=TRUE)) {
    let mz = met$mz;
    let title = met$key;

    met$mz=NULL;
    met$key = NULL;

    let sampleSet = as.data.frame(sampleinfo);

    print(title);
    # print(sampleSet );
    print(title in ["Isocitrate_[2M+H]+ [m/z 385.0652]" "Citrate_[2M+H]+ [m/z 385.0652]"]);

    if (title in ["Isocitrate_[2M+H]+ [m/z 385.0652]" "Citrate_[2M+H]+ [m/z 385.0652]"]) {
        let hk = sampleSet$group == "HK";
        let group = sampleSet$group;
        let color = sampleSet$color;
        let hk_color = unique(color[hk]);

        group[hk] = "HKN";
        group[!hk] = "HK";
        color[hk] = unique(color[!hk]);
        color[!hk] = hk_color;

        sampleSet[,"group"] = group;
        sampleSet[,"color"] = color;
        sampleSet = sampleSet[order(sampleSet$group), ];

        print(sampleSet);


MSI_ionStatPlot(mzpack = rawdata, mz = mz, met = met, sampleinfo = sampleSet , 
                                 savePng        = file.path("msi/map00020", `${title |> normalizeFileName(FALSE,".", maxchars=72)}.png` ), 
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

    } else {
        next;
    }

}