require(GCModeller);

# toolkit script for make chebi_lite.obo from the full dataset

imports "OBO" from "annotationKit";

setwd(@dir);

obo = OBO::read.obo("./chebi.obo");
obo 
|> filter.is_obsolete() 
|> filter_properties([
    "http://purl.obolibrary.org/obo/chebi/monoisotopicmass",
    "http://purl.obolibrary.org/obo/chebi/inchikey",
    "http://purl.obolibrary.org/obo/chebi/inchi",
    "http://purl.obolibrary.org/obo/chebi/smiles"
])
|> write.obo( "./chebi_lite.obo",  excludes = ["synonym", "relationship","alt_id","xref","subset"], strip_namespace_prefix = "http://purl.obolibrary.org/obo/chebi/")
;