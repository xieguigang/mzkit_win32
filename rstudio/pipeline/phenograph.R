require(GCModeller);
require(Erica);

imports "igraph" from "igraph";
imports "phenograph" from "Erica";
imports "geneExpression" from "phenotype_kit";

[@info "the matrix file path for run the phenograph cluster analysis."]
const input_data as string = ?"--raw"      || stop("missing of the raw data matrix file for run MSI data analysis!");
const read_bin as boolean = ?"--read_bin" || FALSE;
const knn as integer = as.integer(?"--knn" || 16);
[@info "the directory path for export the graph result"]
const savegraph as string = ?"--save"     || `${dirname(raw)}/${basename(raw)}_phenograph/`; 

const read_data = function() {
	if (!read_bin) {
		load.expr(input_data);
	} else {	
		input_data
		|> load.expr0(lazy = FALSE)
		;
	}
}

let pheno = read_data()
|> phenograph(k = knn, score = phenograph::score_metric("cosine"))
;

print(as.data.frame(V(pheno)));
print(as.data.frame(E(pheno)));

save.network(pheno, file = savegraph);