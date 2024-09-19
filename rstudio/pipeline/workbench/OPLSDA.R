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

let opls.da = opls(x = matrix, y = sampleinfo,
                          ncomp = ncomp);

str(opls.da);

write.csv(opls.da$component, file = `${outputdir}/opls/oplsda_component.csv`);
write.csv(opls.da$scoreMN, file = `${outputdir}/opls/oplsda_scoreMN.csv`);
write.csv(opls.da$loadingMN, file = `${outputdir}/opls/oplsda_loadingMN.csv`);


let opls_score = opls.da$scoreMN;
let opls_loading = opls.da$loadingMN;

svg(file = `${outputdir}/opls/oplsda_loadingMN.svg`, width = 1920, height = 1600) {
    ggplot(opls_loading, aes(x="P1", y = "P2", color = "VIP"), padding = [200 400 200 250])
    + stat_ellipse()
    + geom_point(
        size = 3, color = "viridis:turbo"
    )
    ;
}

opls_score[, "class_id"] = rownames(opls_score);

svg(file = `${outputdir}/opls/oplsda_scoreMN.svg`, width = 1920, height = 1600) {
    
    let score_figure = ggplot(opls_score, aes(x="T1", y = "T2", color = "class_id", label = rownames(matrix)), 
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