imports "BackgroundTask" from "PipelineHost";

require(JSON);
require(graphics2D);

# title: export MSI peaktable
# author: xieguigang <xie.guigang@gcmodeller.org>
# description: export MSI peaktable for downstream data analysis

[@info "the spatial rawdata file its file path, should be a 
        data file in mzpack file format."]
const raw as string = ?"--raw" || stop("a raw data file in mzpack format must be provided!");

[@info "the regions file, this parameter will enable the 
        script just export the spot samples inside the specific 
		sample regions."]
const regions  as string = ?"--regions" || NULL;

[@info "the matrix file save location"]
const savepath as string = ?"--save"    || `${dirname(raw)}/${basename(raw)}_sampledata.csv`;
const mzdiff   as string = ?"--mzdiff"  || "da:0.005";
const intocutoff as double = ?"--into.cutoff" || 0.05;
const TrIQ as double       = ?"--TrIQ"  || 1.0;

[@info "export the spatial expression matrix as the GCModeller 
        HTS expression matrix object? not a general csv ascii 
		text file."]
const save_bin as boolean  = ?"--bin"   || FALSE;

#' get regions polygon data
#' 
const getRegions = function() {
	regions
	|> read.tissue_regions()
	;
}

if (save_bin) {
	print("the spatial expression matrix will be saved as a GCModeller HTS matrix object!");
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


