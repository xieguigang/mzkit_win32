imports "BackgroundTask" from "PipelineHost";

const raw      as string = ?"--raw"      || stop("a raw data file in mzpack format must be provided!");
const savepath as string = ?"--save"     || stop("A file path of the table data output must be provided!");
const massDiff as double = ?"--massdiff" || 0.005;

raw 
|> MS1deconv(massDiff)
|> write.csv(file = savepath, row.names = TRUE)
;