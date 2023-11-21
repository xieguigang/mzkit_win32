imports "clustering" from "MLkit"; 

const input_data = ?"--rawdata" || stop("a raw data matrix must be provided!");
const k as integer = as.integer(?"--k" || 6);
const bisectingKMeans as boolean = ?"--bisecting-kmeans";
const savefile = ?"--save" || `${dirname(input_data)}/${basename(input_data)}_kmeans.csv`;

const rawdata = read.csv(input_data, row.names = 1, check.names = FALSE);
const clusters = kmeans(rawdata, 
    centers = k, 
    bisecting = bisectingKMeans
) |> as.data.frame()
;
const class = clusters$Cluster;

clusters[,"Cluster"] = NULL;
clusters[,"class"] = class;

print("get k-means result:");
print(clusters, max.print = 13);

write.csv(clusters, file = savefile, row.names = TRUE);