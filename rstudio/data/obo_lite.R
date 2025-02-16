require(GCModeller);

# toolkit script for make chebi_lite.obo from the full dataset

imports "OBO" from "annotationKit";

setwd(@dir);

obo = OBO::read.obo("./chebi.obo");
obo 
|> filter.is_obsolete() 
|> filter_properties(["monoisotopicmass","inchikey","inchi","smiles"])
|> write.obo( "./chebi_lite.obo",  excludes = ["synonym", "relationship","alt_id","xref","subset"])
;