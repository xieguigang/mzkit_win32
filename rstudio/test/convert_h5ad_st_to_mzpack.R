
imports "BackgroundTask" from "PipelineHost";
imports "STImaging" from "PipelineHost";
imports "package_utils" from "devkit";

package_utils::attach("D:\Erica");

imports "STdata" from "Erica";

require(mzkit);

imports "mzweb" from "mzkit";

spots = read.spatial_spots("C:\Users\lipidsearch\Desktop\tissue_positions_list.csv");
matrix = read.ST_spacerangerH5Matrix("C:\Users\lipidsearch\Desktop\raw_feature_bc_matrix.h5");

str(spots);
str(matrix);

ST_spaceranger.mzpack(spots, matrix)
|> write.mzPack(file = "C:\Users\lipidsearch\Desktop\raw_feature_bc_matrix.mzPack");