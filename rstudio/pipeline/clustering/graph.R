imports "clustering" from "MLkit"; 

const input_data = ?"--rawdata" || stop("a raw data matrix must be provided!");
const cutoff as double = as.numeric(?"--cutoff" || 0.8);
const savefile = ?"--save" || `${dirname(input_data)}/${basename(input_data)}_graph.csv`;

const rawdata = read.csv(input_data, row.names = 1, check.names = FALSE);
const clusters = btree(rawdata, equals = cutoff,
                            gt = cutoff / 2) |> as.data.frame();
const class = clusters$Cluster;
const uniq_class = unique(class);

let i = 1;

for(tag in uniq_class) {
    class[tag == class] = i;
    i = i + 1;
}

clusters[,"Cluster"] = NULL;
clusters[,"class"] = class;

print("get graph clustering result:");
print(clusters, max.print = 13);

write.csv(clusters, file = savefile, row.names = TRUE);