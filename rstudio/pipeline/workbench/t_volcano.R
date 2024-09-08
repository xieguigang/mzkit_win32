require(ggplot);
require(GCModeller);

imports "stats" from "Rlapack";
#' the gene expression matrix data toolkit
imports "geneExpression" from "phenotype_kit";

# t-test and volcano plot of two sample group comparision
let rawdata = ?"--rawdata" || stop("matrix file for the data analysis and plot is required!");
let sampleinfo = ?"--sampleinfo" || stop("sample group information is required for defined the data comparison source.");
let trial = ?"--trial" || stop("group name of trial is required");
let control = ?"--control" || stop("group name of the control is required");
let output_dir = ?"--output_dir" || dirname(rawdata);
let log2fc as double = ?"--log2fc" || 1.0;
let pvalue as double = ?"--pvalue" || 0.05;

let matrix = rawdata
|> load.expr0(
    lazy = FALSE) 
|> as.data.frame()
;
let sample_info = read.csv(sampleinfo, row.names = 1, check.names = FALSE);
let trial_data = sample_info[sample_info$sample_info == trial,];
let control_data = sample_info[sample_info$sample_info == control,];

print(sample_info, max.print = 6);
# print(matrix, max.print = 6);

trial_data = t(matrix[, rownames(trial_data)]);
control_data = t(matrix[, rownames(control_data)]);

str(trial_data, max.print = 6);
str(control_data, max.print = 6);

let ttest = lapply(rownames(matrix), function(id) {
    let trial = trial_data[,id];
    let control = control_data[,id];
    let test = t.test(trial, control,var.equal=TRUE);
    let avg_trial = mean(trial);
    let avg_control = mean(control);
    let sd_trial = sd(trial);
    let sd_control = sd(control);
    let foldchange = avg_trial / avg_control;

    list(
        name = id,
        meanof_trial = avg_trial,
        meanof_control = avg_control,
        sd_trial = sd_trial,
        sd_control = sd_control,
        foldchange = foldchange,
        ttest = test
    );
});

ttest = data.frame(
    row.names = ttest@name,
    `meanof_${trial}` = ttest@meanof_trial,
    `sdof_${trial}` = ttest@sd_trial,
    `meanof_${control}` = ttest@meanof_control,
    `sdof_${control}` = ttest@sd_control,
    foldchange = ttest@foldchange,
    log2fc = log( ttest@foldchange, 2),
    pvalue = [ttest@ttest]::Pvalue,
    t = [ttest@ttest]::TestValue
);

colnames(ttest) = [
    `mean_${trial}` ,
    `sd_${trial}` ,
    `mean_${control}` ,
    `sd_${control}` ,
    "foldchange" ,
    "log2fc" ,
    "pvalue" ,
    "t"
];

let reg = rep("not sig", nrow(ttest));
let i = (ttest$log2fc > log2fc) && (ttest$pvalue < pvalue);

reg[i] = "up";
i = (ttest$log2fc < -log2fc) && (ttest$pvalue < pvalue);
reg[i] = "down";

ttest[, "reg_diff"] = reg;

print("get t-test result of the metabolites different expression:");
print(ttest,max.print =6);

write.csv(ttest, file = file.path(output_dir, "ttest_diffsig.csv"));

# plot volcano
bitmap(file = file.path(output_dir,"volcano.png"), size = [3800,3000]) {
    ttest[,"pvalue"] = -log10(ttest$pvalue);
    ttest[,"id"] =rownames(ttest);

    # create ggplot layers and tweaks via ggplot style options
	ggplot(ttest, aes(x = "log2fc", y = "pvalue"), padding = "padding:250px 500px 250px 300px;")
	   + geom_point(aes(color = "reg_diff"), color = "black", shape = "circle", size = 50,alpha = 0.7)
       + scale_colour_manual(values = list(
          up        = "#D22628",
          "not sig" = "black",
          down      = "#0091D5"
       ), alpha = 0.7)
       # + geom_text(aes(label = "id"), check_overlap = TRUE, size = 8)
       + geom_hline(yintercept = -log10(pvalue),      color = "red", line.width = 5, linetype = "dash")
       + geom_vline(xintercept =  log2fc, color = "red", line.width = 5, linetype = "dash")
       + geom_vline(xintercept = -log2fc, color = "red", line.width = 5, linetype = "dash")
       + labs(x = "log2(FoldChange)", y = "-log10(P.value)")
       + ggtitle(`Volcano Plot (${trial} vs ${control})`)
       + scale_x_continuous(labels = "F2")
       + scale_y_continuous(labels = "F2")
	   + theme(plot.title = element_text(family = "Cambria Math", size = 20)) 
	;
}