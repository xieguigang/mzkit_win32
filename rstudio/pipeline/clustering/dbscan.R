imports "clustering" from "MLkit"; 

const input_data = ?"--rawdata" || stop("a raw data matrix must be provided!");
const min_pts = ?"--min_pts" || 5;
const eps as double = ?"--eps" || 1;
const savefile = ?"--save" || `${dirname(input_data)}/${basename(input_data)}_dbscan.csv`;

const rawdata = read.csv(input_data, row.names = 1, check.names = FALSE);
const assign = dbscan(rawdata, eps, minPts = min_pts);
const clusters = ([assign]::cluster) |> as.data.frame();
const class = clusters$Cluster;

clusters[,"Cluster"] = NULL;
clusters[,"class"] = class;

print("get dbscan result:");
print(clusters, max.print = 13);

write.csv(clusters, file = savefile, row.names = TRUE);