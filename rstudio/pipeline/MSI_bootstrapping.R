require(mzkit);
require(JSON);

# export the expression table for metabolomics analysis
imports ["MSI", "TissueMorphology"] from "mzkit";
imports "MsImaging" from "mzplot";

const rawdata = ?"--mzpack" || stop("The MSI rawdata file must be provided!");
const tissue_cdf = ?"--tissue-cdf" || stop("The tissue segmentation map must be provided!");
const export_dir = ?"--export" || `${dirname(rawdata)}/${basename(rawdata)}_metabolon/`;
const nsamples = ?"--nsamples" || 32;
const coverage = ?"--coverage" || 0.3;
const ionset = ?"--ionset" || NULL;
const mzdiff = ?"--mzdiff" || 0.01;

const tissue_data = loadTissue(tissue_cdf);
const msi_raw = open.mzpack(rawdata) |> MsImaging::viewer();
const ions = {
    if (nchar(ionset) > 0) {
        ionset 
        |> readText()
        |> JSON::json_decode()
        ;
    } else {
        # get ions from rawdata
        let set = open.mzpack(rawdata) 
        |> MSI::ionStat(da = mzdiff)
        |> as.data.frame()
        ;
        let mz = set$mz;
        let mzkey = `MSI_${round(mz, 4)}`;

        as.list(mz, names = mzkey);
    }
}

const layers = MSIlayer(msi_raw, mz = unlist(ions), 
    tolerance = `da:${mzdiff}`, 
    split = TRUE);

names(layers) = names(ions);

const labels = [tissue_data]::label;
const expr = lapply(layers, function(b) {
    MSI::sample_bootstraping(b, tissue_data, 
        n = nsamples, coverage = coverage);
});

let mat = data.frame();

for(label in labels) {
    let block = data.frame();

    for (name in names(expr)) {
        matrix = expr[[name]];
        block = cbind(block, matrix[[label]]);
    }

    colnames(block) = names(expr);
    rownames(block) = `${label}_${1:nsamples}`;

    mat = cbind(mat, t(block));

    invisible(NULL);
}

write.csv(mat, file = `${export_dir}/expr.csv`, row.names = TRUE);