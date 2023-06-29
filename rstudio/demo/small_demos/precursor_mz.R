require(mzkit);

imports "math" from "mzkit";

let exact_mass = 131.0946;

# evaluate mz from exact mass
exact_mass
|> math::mz(mode = "+")
|> as.data.frame()
|> print()
;