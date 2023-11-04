imports "umap" from "MLkit";
imports "clustering" from "MLkit";

require(GCModeller);

[@info "A matrix file input, could be a general csv 
        ascii text file or the GCModeller HTS expression 
		matrix file."]
const input_data  = ?"--input" || stop("A data matrix excel table file must be specific!");
const output_file = ?"--save"  || `${dirname(input_data)}/${basename(input_data)}_umap3d.csv`; 
const knn_size    = ?"--knn"   || 16;
const knniter     = ?"--knniter" || 64;
const localconnectivity = ?"--localconnectivity" || 1.0;
const bandwidth = ?"--bandwidth" || 1.0;
const learningrate = ?"--learningrate" || 1.0;
const spectral_cos as boolean = ?"--spectral_cos" || FALSE;
[@info "read the GCModeller HTS expression matrix binary file?"]
const read_bin as boolean = ?"--read_bin" || FALSE;

let data   = {
	if (!read_bin) {
		read.csv(input_data, row.names = 1, check.names = FALSE);
	} else {
		imports "geneExpression" from "phenotype_kit";

		input_data
		|> load.expr0(lazy = FALSE)
		|> as.data.frame()
		;
	}
}
let labels = NULL;

if ("class" in colnames(data)) {
	labels          = data$class;
	data[, "class"] = NULL;
}
if ("Cluster" in colnames(data)) {
	labels            = data$Cluster;
	data[, "Cluster"] = NULL;
}

let dim3 = data |> umap( 
	dimension         = 10, 
	spectral_cos      = spectral_cos,
	numberOfNeighbors = knn_size,
	localConnectivity = localconnectivity,
	KnnIter   = knniter ,
    bandwidth  = bandwidth,
    learningRate  = learningrate
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
	# result[, "class"] = assign_cluster(result);
} else {
	result[, "class"] = labels;
}

# 1. labels
# 2. x, y, z
# 3. class

write.csv(result, file = output_file, row.names = TRUE);