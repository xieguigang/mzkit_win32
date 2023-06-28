require(mzkit);

#' The chemical formulae toolkit
imports "formula" from "mzkit";

print(formula::eval(["CH3CH3", "H", "O3", "Na2CH2"]));