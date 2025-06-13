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
    "http://purl.obolibrary.org/obo/chebi/mass"
])
|> set_remarks([
    "converts from the chebi.obo full dataset"
])
|> set_propertyValue(
    download_url = "https://ftp.ebi.ac.uk/pub/databases/chebi/ontology/chebi.obo",
    prefix_namespace = "http://purl.obolibrary.org/obo/chebi/"
)
|> set_namespace(dag = obo |> ontologyTree(), namespace = ["chemical entity" "role" "subatomic particle"])
|> write.obo( "./chebi_lite.obo",  excludes = ["relationship","alt_id","subset"], 
        strip_namespace_prefix = "http://purl.obolibrary.org/obo/chebi/",
        strip_property_unit=TRUE)
;