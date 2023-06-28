require(mzkit);

#' m/z data operator module
imports "data" from "mzkit";
imports "visual" from "mzplot";

# construct a specrum object from a dataframe
let spectrum = libraryMatrix(data.frame(
    mz = [100, 200, 301], 
    intensity = [1, 0.9, 0.33]
));

bitmap(file = "/spectrum.png") {
    plot(spectrum);
}

