imports "BackgroundTask" from "PipelineHost";

require(JSON);
require(graphics2D);

# title: export MSI peaktable
# author: xieguigang <xie.guigang@gcmodeller.org>
# description: export MSI peaktable for downstream data analysis

const raw      as string = ?"--raw"     || stop("a raw data file in mzpack format must be provided!");
const regions  as string = ?"--regions" || NULL;
const savepath as string = ?"--save"    || stop("A file path of the table data output must be provided!");
const mzdiff   as string = ?"--mzdiff"  || "da:0.005";
const intocutoff as double = ?"--into.cutoff" || 0.05;
const TrIQ as double = ?"--TrIQ" || 0.65;

#' get regions polygon data
#' 
const getRegions = function() {
	regions
	|> read.tissue_regions()
	;
}

if (is.null(regions)) {

	# D:\mzkit\dist\bin/Rstudio/bin/Rscript.exe "D:/mzkit/src/mzkit/rstudio/pipeline/MSI_peaktable.R" 
	# --raw "D:\mzkit\DATA\test\HR2MSI mouse urinary bladder S096 - Figure1.cdf" 
	# --save "C:\Users\lipidsearch\Desktop\matrix3.csv" 
	# --mzdiff "0.005"
	# --into.cutoff "0.05"
	# --TrIQ "0.8"
	# --SetDllDirectory D:/mzkit/dist/bin/Rstudio/host

	print("a rectangle list data in json format is not provided!");
	print("all of the spot ion features will be export!");
	
	using savefile as file(savepath, truncate = TRUE) {
		raw |> BackgroundTask::MSI_peaktable(
			NULL, savefile, 
			mzdiff = mzdiff,
			into.cutoff = intocutoff, 
			TrIQ = TrIQ
		);
	}
} else {
	using savefile as file(savepath, truncate = TRUE) {
		BackgroundTask::MSI_peaktable(raw, getRegions(), savefile);
	}
}


