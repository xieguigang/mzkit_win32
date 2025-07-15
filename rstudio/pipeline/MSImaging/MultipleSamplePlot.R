require(MSImaging);
require(mzkit);
require(ggplot);
require(JSON);

imports "app" from "ServiceHub";
imports "MSI" from "mzkit";
imports "MsImaging" from "mzplot";
imports "mzDeco" from "mz_quantify";
imports "xcms" from "mz_quantify";
imports "geneExpression" from "phenotype_kit";
imports "sampleInfo" from "phenotype_kit";

options(memory.load = "max");

const appPort as integer  = ?"--app"    || stop("A MSimaging services hub app handle must be provided!");
const workdir as string   = ?"--tmpdir" || stop("A workdir for run data visual must be provided!");
const colorSet as string  = ?"--colors" || "jet"; 
const groups = read.sampleinfo(file.path(workdir,"sampleinfo.csv"));
const regions = file.path(workdir, "geometry.json") 
                |> readText() 
                |> JSON::json_decode()
                ;
let dim_x = as.integer([regions]::width);
let dim_y = as.integer([regions]::height);

let offsets = lapply(regions$regions, r -> mean(r$xpoints), names = regions$sample_tags);
let peak_ions = readBin(file.path(workdir,"peakset.xcms"), what = "peak_set");

print("view of the region geometry data:");
str(offsets);
print("msimaging dimension [x,y]:");
print([dim_x, dim_y]);

let Rplot_w = dim_x * 6;
let Rplot_y = dim_y * 2 * (length([peak_ions]::peaks) + 1);
let Rplot = function() {
    # plot the color scaler legend object
    colorSet |> colorMap.legend(
        [0, 100],
        titleFont = "font-style: strong; font-size: 24; font-family: Cambria;", 
        tickFont  = "font-style: normal; font-size: 24; font-family: Cambria;", 
        title     = "", 
        format    = "F0", 
        foreColor = "black"
    )
    |> plot()
    ;   

    let yoffset = 0.1 * Rplot_y;
    let msi_xoffset = Rplot_w - dim_x * 3.5;

    for(let tag in names(offsets)) {
        let x_pos = offsets[[tag]];
        x_pos = msi_xoffset + x_pos * 3;
        x_pos = x_pos - 50;

        text(x = x_pos, y = yoffset - 50, labels = tag, col = "black");
    }

    for(let ion in [peak_ions]::peaks) {
        let name = [ion]::ID;
        let mz   = [ion]::mz;
        let layer = app::getMSIData(
            MSI_service = appPort, 
            mz          = mz, 
            mzdiff      = "da:0.01"
        ) 
        |> as.layer(context = mz, strict = FALSE)
        ;

        print(name);

        [layer]::MSILayer |> rasterHeatmap(
            region       = rect(x = msi_xoffset, y = yoffset, w = dim_x * 3, h = dim_y * 2 , float = FALSE), 
            gauss        = 0, 
            colorName    = colorSet, 
            rasterBitmap = TRUE,
            strict       = FALSE,
            dimSize      = [dim_x, dim_y]
        );

        text(x = msi_xoffset + dim_x * 3 - 120, 
            y = yoffset + dim_y * 2 - 50, 
            labels = `m/z ${round(mz,4)}`,
            col = "white");

        let stat = ggplot(expression_df(ion, groups), aes(x = "group", y = "expr"), padding = [yoffset, msi_xoffset + dim_x + 50, Rplot_y - yoffset - dim_y * 2, 150 ])
        # Add horizontal line at base mean 
        # + geom_hline(yintercept = mean(myeloma$expr), linetype="dash", line.width = 2, color = "red")
        + geom_violin(width = 0.65, alpha = 0.85)
        + geom_jitter(width = 0.3, alpha = 1)	
        + ggtitle("")
        + ylab(name)
        + xlab("")
        + scale_y_continuous(labels = "F0")
        # + stat_compare_means(method = "anova", label.y = 1600) # Add global annova p-value 
        # + stat_compare_means(label = "p.signif", method = "t.test", ref.group = ".all.", hide.ns = TRUE)# Pairwise comparison against all
        + theme(
            axis.text.x = element_text(angle = 45), 
            axis.text.y = element_text(family = "Cambria Math", size = 36),
            plot.title = element_text(family = "Cambria Math", size = 16),
            panel.border = NULL,
            panel.grid.major = NULL,
            panel.grid.minor = NULL,
            panel.grid = NULL
        )
        ;

        plot(stat);

        yoffset= yoffset + dim_y * 2 + 10;
    }
}

svg(file = file.path(workdir,"Rplot.svg"), 
    size = [Rplot_w, Rplot_y], 
    fill = "white");
Rplot();
dev.off();

bitmap(file = file.path(workdir,"Rplot.png"), 
    size = [Rplot_w, Rplot_y], 
    fill = "white");
Rplot();
dev.off();

pdf(file = file.path(workdir,"Rplot.pdf"), 
    size = [Rplot_w, Rplot_y], 
    fill = "white");
Rplot();
dev.off();