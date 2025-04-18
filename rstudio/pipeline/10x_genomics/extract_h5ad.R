require(Erica);
require(mzkit);
require(JSON);

imports "singleCell" from "Erica";
imports ["mzweb", "TissueMorphology"] from "mzkit";
imports "BackgroundTask" from "PipelineHost";
imports "STImaging" from "PipelineHost";

const h5ad_file as string  = ?"--h5ad" || stop("The 10x genomics h5ad rawdata file must be provided!");
const out_file as string   = ?"--save" || `${dirname(h5ad_file)}/${basename(h5ad_file)}.mzPack`;
const export_dir as string = dirname(out_file);
const raw = h5ad_file |> read.h5ad(loadExpr0 = TRUE);
const export_spatial = function(raw) {
    const spatial = raw |> spatialMap(useCellAnnotation = TRUE);

    print("peeks of the spatial data:");
    print(spatial, max.print = 6);    

    bitmap(file = `${export_dir}/spatial_3.png`) {
        plot(spatial[, "x"], spatial[, "y"],
            padding      = "padding:200px 500px 200px 250px;",
            class        = spatial[, "class"],
            title        = "Spatial 2D Tissue Map",
            x.lab        = "X",
            y.lab        = "Y",
            legend.block = 100,
            point.size   = 6,
            colorSet     = "paper",  
            grid.fill    = "transparent",
            size         = [2800, 3200],
            x.format     = "F0",
            y.format     = "F0",
            reverse      = TRUE
        );
    }

    write.csv(spatial, file = `${export_dir}/spatial.csv`, row.names = FALSE);
}

try({
    export_spatial(raw);
});

const [gene_exprs, tissue] = extract_h5ad(raw);

write.mzPack(gene_exprs, file = out_file);
TissueMorphology::writeCDF(tissue, file = `${export_dir}/${basename(out_file)}.cdf`);