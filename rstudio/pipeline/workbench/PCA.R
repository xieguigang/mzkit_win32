require(GCModeller);

#' the gene expression matrix data toolkit
imports "geneExpression" from "phenotype_kit";

let mat = ?"--matrix" || stop("a data matrix file must be provided!");
let ncomp as integer = ?"--ncomp" || 3;
let outputdir = ?"--outputdir" || dirname(mat);

let matrix = mat 
|> load.expr0(
    lazy = FALSE) 
|> geneExpression::tr()
|> as.data.frame()
;

# str(matrix);

let pca = prcomp(matrix, pc = ncomp);

print(pca);

write.csv(pca$score, file = `${outputdir}/pca/pca_score.csv`);
write.csv(pca$loading, file = `${outputdir}/pca/pca_loading.csv`);
writeLines(pca$contribution, con = `${outputdir}/pca/pca_contrib.txt`);