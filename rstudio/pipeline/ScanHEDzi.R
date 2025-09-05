require(Erica);

imports "singleCell" from "Erica";
imports "machineVision" from "signalKit";

let dzi = ?"--dzi" || stop("the dzi image metadata file is required!");
let dzi_data = read.dziImage(dzi);
let level = ?"--lv";
let dir = ?"--dir" || file.path(dirname(dzi), `${basename(dzi)}_files/${level}/`);
let outfile = ?"--out" || file.path(dirname(dzi), "cells.bson");

let cells = dzi_data |> scan.dzi_cells(level = level, dir = dir,
                                       ostu_factor = 0.7,
                                       noise = 0.25,
                                       moran_knn = 32,
                                       split_blocks = FALSE);

write.cells_bson(cells, file = outfile);