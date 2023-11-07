require(mzkit);
require(JSON);

# export the expression table for metabolomics analysis

const rawdata = ?"--mzpack" || stop("The MSI rawdata file must be provided!");
const tissue_cdf = ?"--tissue-cdf" || stop("The tissue segmentation map must be provided!");
const export_dir = ?"--export" || `${dirname(rawdata)}/${basename(rawdata)}_metabolon/`;
const nsamples = ?"--nsamples" || 32;
const coverage = ?"--coverage" || 0.3;

