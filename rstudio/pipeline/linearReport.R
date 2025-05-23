require(mzkit);

imports "BackgroundTask" from "PipelineHost";
imports 'Linears' from 'mz_quantify';

const packfile    = ?"--linear" || stop("A linear data file is required!");
const export_html = ?"--export" || `${dirname(packfile)}/${basename(packfile)}_report.html`;
const linearPack  = read.linearPack(packfile) |> linear.setErrPoints();
const standards   = [linearPack]::linears;
const ionsRaw     = linear.ions_raw(linearPack);

standards 
|> report.dataset(NULL, NULL, ionsRaw = ionsRaw)
|> html(reverse = TRUE)
|> writeLines(con = export_html)
;