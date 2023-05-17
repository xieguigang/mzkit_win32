require(Erica);
require(mzkit);
require(JSON);

imports "mzweb" from "mzkit";
imports "BackgroundTask" from "PipelineHost";
imports "STImaging" from "PipelineHost";
imports "package_utils" from "devkit";
imports "STdata" from "Erica";

const spotsTable = ?"--spots" || stop("Missing spots list table file!");
const exprRaw = ?"--expr" || stop("The h5ad expression matrix file must be provided!");
const savefile = ?"--save" || `${dirname(exprRaw)}/${basename(exprRaw)}_import.mzPack`;
const human_genes = "data/HUMAN_geneExpression.json"
|> system.file(package = "Erica")
|> readText()
|> JSON::json_decode()
;

print("view of the human gene data annotation set:");
str(human_genes);
stop();

const spots = read.spatial_spots(spotsTable);
const matrix = read.ST_spacerangerH5Matrix(exprRaw);

ST_spaceranger.mzpack(spots, matrix)
|> write.mzPack(file = savefile)
;