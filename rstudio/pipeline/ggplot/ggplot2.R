require(ggplot);
require(JSON);

const rawdata as string = ?"--data" || stop("no raw data provided!");
const savefile as string = ?"--save" || stop("no output file!");
const title_str as string = ?"--title" || stop("no title!");
const plot_type as string = ?"--plot" || stop("should be one of the 'box', 'bar', 'violin'");

myeloma = rawdata
|> readText()
|> json_decode()
;

sample_color = lapply(myeloma, c -> c$color);
sample_id = names(myeloma);
group_id = [];
group_data = [];

for(name in sample_id) {
	part = myeloma[[name]];
	part = part$data;
	
	group_id = append(group_id, rep(name, length(part)));
	group_data = append(group_data, part);
}

myeloma = data.frame(
	sample_group = group_id,
	data = group_data
);

getGgplot = function() {
	if (plot_type == "box") {
		geom_boxplot(width = 0.65, alpha = 0.85, color = sample_color);
	} else if (plot_type == "bar") {
		geom_barplot(width = 0.65, alpha = 0.85, color = sample_color);
	} else {
		geom_violin(width = 0.65, alpha = 0.85, color = sample_color);
	}
}

plotGgplot = function() {
	ggplot(myeloma, aes(x = "sample_group", y = "data"))
	# Add horizontal line at base mean 
	+ geom_hline(yintercept = mean(group_data), linetype="dash", line.width = 6, color = "red")
	+ getGgplot()
	+ geom_jitter(width = 0.3, alpha = 1, color = sample_color)	
	+ ggtitle(title_str)
	+ ylab("intensity")
	+ xlab("")
	+ scale_y_continuous(labels = "G2")
	+ stat_compare_means(method = "anova", label.y = 1600) # Add global annova p-value 
    + stat_compare_means(label = "p.signif", method = "t.test", ref.group = ".all.", hide.ns = TRUE)# Pairwise comparison against all
	+ theme(
		axis.text.x = element_text(angle = 45),
		plot.title = element_text(family = "Cambria Math", size = 16),
		panel.border = element_rect(size = 10, linetype = "Solid")
	)
	;
}

bitmap(file = savefile, size = [1900, 1200]) {
	plotGgplot();
}