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

let matrix = mat 
|> load.expr0(
    lazy = FALSE) 
|> geneExpression::tr()
|> as.data.frame()
;

#        injectionOrder     batch sample_name                    sample_info    color    shape
# ---------------------------------------------------------------------------------------------
# <mode>      <integer> <integer>    <string>                       <string> <string> <string>
# QC14                0         0      "QC14" "sample_group_170504955598698"  "black"       ""
# QC6                 0         0       "QC6" "sample_group_170504955598698"  "black"       ""
# QC3                 0         0       "QC3" "sample_group_170504955598698"  "black"       ""
# QC9                 0         0       "QC9" "sample_group_170504955598698"  "black"       ""
# QC16                0         0      "QC16" "sample_group_170504955598698"  "black"       ""
# QC11                0         0      "QC11" "sample_group_170504955598698"  "black"       ""
# QC10                0         0      "QC10" "sample_group_170504955598698"  "black"       ""
# QC4                 0         0       "QC4" "sample_group_170504955598698"  "black"       ""
# QC7                 0         0       "QC7" "sample_group_170504955598698"  "black"       ""
# QC5                 0         0       "QC5" "sample_group_170504955919797"  "black"       ""
# QC2                 0         0       "QC2" "sample_group_170504955919797"  "black"       ""
# QC15                0         0      "QC15" "sample_group_170504955919797"  "black"       ""
# QC13                0         0      "QC13" "sample_group_170504955919797"  "black"       ""
# QC12                0         0      "QC12" "sample_group_170504955919797"  "black"       ""
# QC1                 0         0       "QC1" "sample_group_170504955919797"  "black"       ""
# QC8                 0         0       "QC8" "sample_group_170504955919797"  "black"       ""
sampleinfo = read.csv(sampleinfo, row.names = 1);
sampleinfo = as.list(sampleinfo, byrow = TRUE);
sampleinfo = matrix 
|> rownames() 
|> sapply(id -> sampleinfo[[id]]$sample_info)
;

print(matrix 
|> rownames() );
str(sampleinfo);

let pls.da = plsda(x = matrix, y = sampleinfo,
                          ncomp = ncomp);

str(pls.da);

write.csv(pls.da$component, file = `${outputdir}/plsda/plsda_component.csv`);
write.csv(pls.da$scoreMN, file = `${outputdir}/plsda/plsda_scoreMN.csv`);
write.csv(pls.da$loadingMN, file = `${outputdir}/plsda/plsda_loadingMN.csv`);

let pls_score = pls.da$scoreMN;
let pls_loading = pls.da$loadingMN;

svg(file = `${outputdir}/plsda/plsda_loadingMN.svg`, width = 1920, height = 1600) {
    ggplot(pls_loading, aes(x="P1", y = "P2", color = "VIP"), padding = [200 400 200 250])
    + geom_point(
        size = 3, color = "viridis:turbo"
    )
    # + geom_text(size = 6)
    # + stat_ellipse()
    ;
}

pls_score[, "class_id"] = rownames(pls_score);

svg(file = `${outputdir}/plsda/plsda_scoreMN.svg`, width = 1920, height = 1600) {
    
    let score_figure = ggplot(pls_score, aes(x="T1", y = "T2", color = "class_id", label = rownames(matrix)), 
        padding = [200 400 200 250])
    + geom_point(
        size = 16
    )
    # + stat_ellipse()
    ;

    if (show_labels) {
        score_figure <- score_figure + geom_text(size = 6);
    }
    
    score_figure;
}