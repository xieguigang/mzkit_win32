require(mzkit);

#' the R# math module
imports "math" from "mzkit";
#' m/z data operator module
imports "data" from "mzkit";
imports "visual" from "mzplot";

# define the spectrum from dataframe
let ms2_a = libraryMatrix(data.frame(mz = [100, 200, 301], intensity = [1, 0.9, 0.33]));
let ms2_b = libraryMatrix(data.frame(mz = [100, 203, 301], intensity = [0.5, 0.3, 1]));

# evaluate of the spectrum similarity scores
# cos score
str(as.list(math::cosine(ms2_a, ms2_b)));
# spectral_entropy similarity
print(math::spectral_entropy(ms2_a, ms2_b));

# plot specrum alignment
bitmap(file = "/spectrum-alignment.png") {
    mass_spectrum.plot(ms2_a, alignment = ms2_b);
}