imports "clustering" from "MLkit"; 

const input_data = ?"--rawdata" || stop("a raw data matrix must be provided!");
const k as integer = as.integer(?"--k" || 6);
const savefile = ?"--save" || `${dirname(input_data)}/${basename(input_data)}_kmeans.csv`;

const rawdata = read.csv(input_data, row.names = 1, check.names = FALSE);
const clusters = kmeans(rawdata, centers = k);

