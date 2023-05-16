require(mzkit);

imports "BackgroundTask" from "PipelineHost";
imports 'Linears' from 'mz_quantify';

const packfile    = ?"--linear" || stop("A linear data file is required!");
const export_html = ?"--export" || `${dirname(packfile)}/${basename(packfile)}_report.html`;
const linearPack  = 

report.dataset([linearPack]::linears, NULL, NULL, ionsRaw = ionsRaw)
|> html()
|> writeLines(con = export_html)
;