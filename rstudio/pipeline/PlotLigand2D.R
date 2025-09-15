require(GCModeller);
require(emily);

imports "proteinKit" from "seqtoolkit";

let input = ?"--pdb" || stop("the molecule docking result pdb file is required!");
let size = ?"--size" || "3600,2400";
let ligand_key = ?"--ligand" || stop("the ligand id is required for make 2d plot!");
let ligand_num = ?"--num" || stop("the ligand sequence number is required for make 2d plot!");
let dpi = ?"--ppi" || 120;
let outfile = ?"--out" || file.path(dirname(input), "Rplot.png");
let style = ?"--style" || stop("the file path to the plot style parameter json file is required!");
let pdb = read.pdb(input, safe = TRUE);
let ligand = pdb |> proteinKit::ligands(ligand_key, ligand_num);
let filetype = file.ext(outfile);
let args = style |> readText() |> parse_style();

size = as.integer(unlist( strsplit(size,",")));

let make2DRender = function() {
    pdb |> draw_ligand2D(
        ligand = ligand,
        style = args,
        size = size,
        dpi = dpi
    );
}

if (filetype == "png") {
    bitmap(file = outfile) {
        make2DRender();
    }
}
if (filetype == "svg") {
    svg(file = outfile) {
        make2DRender();
    }
}
if (filetype == "pdf") {
    pdf(file = outfile) {
        make2DRender();
    }
}