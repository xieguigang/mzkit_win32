require(GCModeller);
require(ggplot);

imports "stats" from "Rlapack";
#' the gene expression matrix data toolkit
imports "geneExpression" from "phenotype_kit";

let mat = ?"--matrix" || stop("a data matrix file must be provided!");
let sampleinfo = ?"--sampleinfo" || stop("the sample class label file must be provided!");
let ncomp as integer = ?"--ncomp" || 3;
let outputdir = ?"--outputdir" || dirname(mat);
let show_labels as boolean = ?"--show_labels" || TRUE;

let matrix = {
    if (file.ext(mat) == "csv") {
        mat |> load.expr();
    } else {
        mat |> load.expr0(lazy = FALSE) 
    }
};

matrix = matrix
|> geneExpression::tr()
|> as.data.frame()
;
let pca = prcomp(matrix, pc = ncomp);

print(pca);

write.csv(pca$score, file = `${outputdir}/pca/pca_score.csv`);
write.csv(pca$loading, file = `${outputdir}/pca/pca_loading.csv`);
writeLines(pca$contribution, con = `${outputdir}/pca/pca_contrib.txt`);

if (file.exists(sampleinfo)) {
    let pca_score = pca$score;
    let sample_info = read.csv(sampleinfo, row.names = 1, check.names = FALSE);
    let class_id = as.list(sample_info, byrow = TRUE) |> lapply(x -> x$sample_info);

    print(sample_info);
    str(class_id);

    # str(matrix);
    pca_score[, "class_id"] = sapply(rownames(pca_score), x -> class_id[[x]]);

    png(filename = `${outputdir}/pca/pca_score.png`, width = 1920, height = 1600) {
        let score_figure = ggplot(pca_score, aes(x="PC1", y = "PC2", color = "class_id", label = rownames(pca_score)))
        + stat_ellipse()
        + geom_point(
            size = 16
        )    
        ;

        if (show_labels) {
            score_figure <- score_figure + geom_text(size = 6);
        }

        score_figure;
    }

    png(filename = `${outputdir}/pca/pca_loading.png`, width = 1920, height = 1600) {
        ggplot(pca$loading, aes(x="PC1", y = "PC2", label = rownames(pca$loading)))
        + geom_point(
            size = 3, color = "skyblue"
        )
        # + geom_text(size = 6)
        # + stat_ellipse()
        ;
    }
}
