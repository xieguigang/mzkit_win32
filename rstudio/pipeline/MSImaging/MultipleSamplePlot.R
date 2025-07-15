require(MSImaging);
require(mzkit);
require(ggplot);
require(JSON);

imports "app" from "ServiceHub";
imports "MSI" from "mzkit";
imports "MsImaging" from "mzplot";
imports "mzDeco" from "mz_quantify";

options(memory.load = "max");

const appPort as integer  = ?"--app"    || stop("A MSimaging services hub app handle must be provided!");
const workdir as string   = ?"--tmpdir" || stop("A workdir for run data visual must be provided!");
const colorSet as string  = ?"--colors" || "jet"; 
const regions = file.path(workdir, "geometry.json") 
                |> readText() 
                |> JSON::json_decode()
                ;
let dim_x = as.integer([regions]::width);
let dim_y = as.integer([regions]::height);

let offsets = lapply(regions$regions, r -> mean(r$xpoints), names = regions$sample_tags);
let peak_ions = readBin(file.path(workdir,"peakset.xcms"), what = "peak_set");

print("view of the region geometry data:");
str(offsets);
print("msimaging dimension [x,y]:");
print([dim_x, dim_y]);

bitmap(file = file.path(workdir,"Rplot.png"), size = [dim_x * 5, dim_y * (length([peak_ions]::peaks) + 1)]);

colorSet |> colorMap.legend(
    [0, 100],
    titleFont = "font-style: strong; font-size: 16; font-family: Cambria;", 
    tickFont  = "font-style: normal; font-size: 16; font-family: Cambria;", 
    title     = "", 
    format    = "F0", 
    foreColor = "white"
)
|> plot()
;   

let yoffset = 20;

for(let ion in [peak_ions]::peaks) {
    let name = [ion]::ID;
    let mz   = [ion]::mz;
    let layer = app::getMSIData(
        MSI_service = appPort, 
        mz          = mz, 
        mzdiff      = "da:0.01"
    ) 
    |> as.layer(context = mz, strict = FALSE)
    ;

    [layer]::MSILayer |> rasterHeatmap(
        region       = rect(x = 200, y = yoffset, w = dim_x * 3, h = dim_y, float = FALSE), 
        gauss        = 0, 
        colorName    = colorSet, 
        rasterBitmap = TRUE,
        strict       = FALSE,
        dimSize      = [dim_x, dim_y]
    );

    yoffset= yoffset + dim_y + 10;
}

dev.off();