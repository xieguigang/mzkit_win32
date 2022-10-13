require(mzkit);
require(ggplot);

imports "TissueMorphology" from "mzkit";

const tissue_file as string = ?"--tissue_map" || stop("A mzkit tissue map cdf file must be provided!");
const save_png as string    = ?"--save" || `${dirname(tissue_file)}/${basename(tissue_file)}-tissue_map.png`; 
const data = tissue_file 
|> loadTissue()
|> as.data.frame()
;
	
data[, "label"] = basename(data[, "label"]);
	
print("view tissue map matrix data:");
print(data, max.print = 13);
		
const labels = unique(data[, "label"]);
const colors = unique(data[, "color"]);
const colorMaps = lapply(1:length(labels), i -> colors[i], names = labels);

print("get unique color maps for each tissue region:");
str(colorMaps);
		
bitmap(file = save_png, size = [8600, 2100]) {
	# open graphics device and then do plot
	ggplot(data, aes(x = "x", y = "y", color = "label"), padding = "padding:300px 1600px 200px 250px;")
	+ geom_point(aes(color = "label"), shape = "rectangle", size = 30)
	+ scale_colour_manual(values = colorMaps)
	+ labs(x = "X", y = "Y")
	+ ggtitle(`Tissue Morphology - ${basename(tissue_file)}`)
	+ scale_x_continuous(labels = "F0")
	+ scale_y_continuous(labels = "F0")
	+ scale_y_reverse()
	;
}