require(GCModeller);
require(ggplot);

imports "stats" from "Rlapack";
#' the gene expression matrix data toolkit
imports "geneExpression" from "phenotype_kit";

let mat = ?"--matrix" || stop("a data matrix file must be provided!");
let sampleinfo = ?"--sampleinfo" || stop("the sample class label file must be provided!");
let ncomp as integer = ?"--ncomp" || 3;
let outputdir = ?"--outputdir" || dirname(mat);

let matrix = mat 
|> load.expr0(
    lazy = FALSE) 
|> geneExpression::tr()
|> as.data.frame()
;
let sample_info = read.csv(sampleinfo, row.names = 1, check.names = FALSE);
let class_id = as.list(sample_info, byrow = TRUE) |> lapply(x -> x$sample_info);

print(sample_info);
str(class_id);

# str(matrix);

let pca = prcomp(matrix, pc = ncomp);

print(pca);

write.csv(pca$score, file = `${outputdir}/pca/pca_score.csv`);
write.csv(pca$loading, file = `${outputdir}/pca/pca_loading.csv`);
writeLines(pca$contribution, con = `${outputdir}/pca/pca_contrib.txt`);

let pca_score = pca$score;

pca_score[, "class_id"] = sapply(rownames(pca_score), x -> class_id[[x]]);

svg(file = `${outputdir}/pca/pca_score.svg`) {
    ggplot(pca_score, aes(x="PC1", y = "PC2", color = "class_id", label = rownames(pca_score)))
    + geom_point(
        size = 16
    )
    + geom_text(size = 6)
    # + stat_ellipse()
    ;
}

svg(file = `${outputdir}/pca/pca_loading.svg`) {
    ggplot(pca$loading, aes(x="PC1", y = "PC2", label = rownames(pca$loading)))
    + geom_point(
        size = 9, color = "blue"
    )
    # + geom_text(size = 6)
    # + stat_ellipse()
    ;
}