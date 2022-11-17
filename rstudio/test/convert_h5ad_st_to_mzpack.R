imports "package_utils" from "devkit";

package_utils::attach("D:\Erica");


imports "STdata" from "Erica";
imports "BackgroundTask" from "PipelineHost";

require(mzkit);

imports "mzweb" from "mzkit";

spots = read.spatial_spots("F:\20221117-ST\tissue_positions_list.csv");
matrix = read.ST_spacerangerH5Matrix("F:\20221117-ST\raw_feature_bc_matrix.h5");

str(spots);
str(matrix);

ST_spaceranger.mzpack(spots, matrix)
|> write.mzPack(file = "F:\20221117-ST\tissue_positions_list.mzPack");