imports "umap" from "MLkit";
imports "clustering" from "MLkit";

const input_data  = ?"--input" || stop("A data matrix excel table file must be specific!");
const output_file = ?"--save"  || `${dirname(input_data)}/${basename(input_data)}_umap3d.csv`; 
const knn_size    = ?"--knn"   || 16;

let data   = read.csv(input_data, row.names = 1, check.names = FALSE);
let labels = NULL;

if ("class" in colnames(data)) {
	labels          = data$class;
	data[, "class"] = NULL;
}

let dim3 = data |> umap( 
	dimension         = 3, 
	spectral_cos      = TRUE,
	numberOfNeighbors = knn_size
);
let result         = as.data.frame(dim3$umap, labels = dim3$labels, dimension = ["x","y","z"]);
let assign_cluster = function(result) {
	let scans      = dbscan(result, eps = 1, minPts = 6);
	let cluster_id = [scans]::classLabels;
	
	print("create the cluster tags for the umap result:");
	print(cluster_id);
	
	return(cluster_id);
}

if (is.null(labels)) {
	# add class label via dbscan?
	result[, "class"] = assign_cluster(result);
} else {
	result[, "class"] = labels;
}

# 1. labels
# 2. x, y, z
# 3. class

write.csv(result, file = output_file, row.names = TRUE);