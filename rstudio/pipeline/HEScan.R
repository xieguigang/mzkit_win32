require(mzkit);
require(graphics2D);
require(filter);
require(JSON);

imports "tissue" from "mzplot";

const map as string      = ?"--bitmap"   || stop("no image source is provided!");
const channels as string = ?"--channels" || stop("a set of color channels must be provided!");
const savejson as string = ?"--save"     || NULL;
const layers as string   = strsplit(channels, "[;,\/]") |> unlist();
const grid = map
|> readImage()
|> tissue::scan_tissue(colors = layers, tolerance = 30)
;

grid
|> json_encode()
|> writeLines(con = savejson)
;