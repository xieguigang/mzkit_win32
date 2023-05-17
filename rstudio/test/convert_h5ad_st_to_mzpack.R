require(Erica);
require(mzkit);
require(JSON);

imports "mzweb" from "mzkit";
imports "BackgroundTask" from "PipelineHost";
imports "STImaging" from "PipelineHost";
imports "package_utils" from "devkit";
imports "STdata" from "Erica";
imports "geneExpression" from "phenotype_kit";

const spotsTable = ?"--spots" || stop("Missing spots list table file!");
const exprRaw = ?"--expr" || stop("The h5ad expression matrix file must be provided!");
const namefile = ?"--targets";
const savefile = ?"--save" || `${dirname(exprRaw)}/${basename(exprRaw)}_import.mzPack`;
const human_genes = "data/HUMAN_geneExpression.json"
|> system.file(package = "Erica")
|> readText()
|> JSON::json_decode()
;
const target_nameset = {
    if (namefile == "") {
        NULL;
    } else {
        readLines(namefile);
    }
}

print("view of the human gene data annotation set:");
str(human_genes);

const spots = read.spatial_spots(spotsTable);
const matrix = read.ST_spacerangerH5Matrix(exprRaw);
const summary = geneExpression::dims(matrix);

print("view of the STdata summary:");
str(summary);

const geneIds = summary$sample_names;
const maps = lapply(human_genes, x -> x$"Gene Names (synonym)");
const anno_tags = map_geneNames(geneIds, maps, target_nameset);

print("mapping to gene names:");
print(anno_tags);

stop();

ST_spaceranger.mzpack(spots, matrix)
|> write.mzPack(file = savefile)
;