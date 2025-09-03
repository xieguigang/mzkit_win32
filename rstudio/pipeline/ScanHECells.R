require(Erica);

imports "singleCell" from "Erica";
imports "machineVision" from "signalKit";

let imagefile = ?"--img" || stop("A HE image file for make scan must be provided!");
let outfile = ?"--out" || file.path(dirname(imagefile), "cells.csv");
let snapshot = readImage(imagefile);
let bin = machineVision::ostu(snapshot, flip = FALSE,
                            factor = 1.125);
print(snapshot);

let cells = bin |> singleCell::HE_cells(is.binarized = TRUE,
                            flip = FALSE,
                            ostu.factor = 0.7,
                            offset = NULL,
                            noise = 0.25,
                            moran.knn = 32);

print(as.data.frame(cells));

write.csv(as.data.frame(cells), file = outfile);