imports "BackgroundTask" from "PipelineHost";

require(mzkit);
require(JSON);

# title: Build indexed MSI cache
# author: xieguigang <xie.guigang@gcmodeller.org>

[@info "the file path of the mzPack/mgf raw data file to create cache index."]
[@type "*.mzpack;*.mgf"]
const raw as string = ?"--raw" || stop("no raw data file provided!");
[@info "the directory path of the metaDNA output result files."]
[@type "directory"]
const outputdir as string = ?"--save" || stop("a directory path for save result files must be provided!");
const ssid as string = ?"--biodeep_ssid" || stop("a session id for biodeep is required!");
const ppm as double = ?"--ppm" || 20.0;
const dotcutoff as double = ?"--dotcutoff" || 0.5;
const rawdata = open.mzpack(raw);
const kegg_network = GCModeller::kegg_reactions();

sleep(1);

# BackgroundTask::biodeep.session(ssid);
let [output, infer] = rawdata |> BackgroundTask::metaDNA(
    ppm = ppm, 
    dotcutoff = dotcutoff,
    reactions = kegg_network
);

write.csv(output, file = file.path(outputdir, "metaDNA_annotation.csv"));  

infer 
|> JSON::json_encode()
|> writeLines(
    con = file.path(outputdir, "infer_network.json")
);