----===== BioNovoGene MZKit® =====----

Data toolkits for processing NMR, MALDI MSI, LC-MS 
and GC-MS raw data, chemoinformatics data analysis 
and data visualization.

If you have already installed the mzkit application, 
please uninstall the previous version before you 
install the new version of the mzkit application.

mzkit home: https://mzkit.org/
mzkit on ELIXIR bio tools: https://bio.tools/mzkit
mzkit source code: https://github.com/xieguigang/mzkit
mzkit downloads: http://www.biodeep.cn/downloads?lang=en-US
mzkit on SciCrunch: RRID:SCR_023936

Contact The Author: gg.xie@bionovogene, 
                    xieguigang@metabolomics.ac.cn

------====== Runtime & System Requirements ======--------

mzkit_win32 application required of .NET Framework 4.8 runtime:
https://dotnet.microsoft.com/download/dotnet-framework/net48
Rstudio application required of .NET Core 8.0(windows-x64) runtime:
https://dotnet.microsoft.com/en-us/download/dotnet/8.0
Microsoft WebView2 runtime(windows-x64) is required for some interactive data visualization and analysis report generation:
https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section
Read GCxGC or GCMS netCDF rawdata file required of unidata netCDF-C library:
https://downloads.unidata.ucar.edu/netcdf-c/4.9.2/netCDF4.9.2-NC4-64.exe

---==== Copyright & LICENSE ====---

You can cite MZKit in your literature work: xieguigang/mzkit: BioNovoGene Mzkit. (2021) doi:10.5281/zenodo.4603277.

1. MZKit® is a registered trademark of BioNovoGene Corporation, protected by copyright law and international treaties.
2. RawFileReader reading tool. Copyright © 2016 by Thermo Fisher Scientific, Inc. All rights reserved.
3. Software license:

MIT License

Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

---==== Reference ====---

1.X. Shen, R. Wang, X. Xiong, Y. Yin, Y. Cai, Z. Ma, N. Liu, and Z.-J. Zhu* (Corresponding Author), Metabolic Reaction Network-based Recursive Metabolite Annotation for Untargeted Metabolomics, Nature Communications, 2019, 10: 1516.
2.Li S, Park Y, Duraisingham S, Strobel FH, Khan N, et al. (2013) Predicting Network Activity from High Throughput Metabolomics. PLOS Computational Biology 9(7): e1003123. https://doi.org/10.1371/journal.pcbi.1003123
3.Pang, Z., Chong, J., Zhou, G., Morais D., Chang, L., Barrette, M., Gauthier, C., Jacques, PE., Li, S., and Xia, J. (2021) MetaboAnalyst 5.0: narrowing the gap between raw spectra and functional insights Nucl. Acids Res. (doi: 10.1093/nar/gkab382)
4.Ogata, H., Goto, S., Sato, K., Fujibuchi, W., Bono, H., & Kanehisa, M. (1999). KEGG: Kyoto Encyclopedia of Genes and Genomes. Nucleic acids research, 27(1), 29–34. https://doi.org/10.1093/nar/27.1.29
5.Tsugawa, H., Cajka, T., Kind, T., Ma, Y., Higgins, B., Ikeda, K., Kanazawa, M., VanderGheynst, J., Fiehn, O., & Arita, M. (2015). MS-DIAL: data-independent MS/MS deconvolution for comprehensive metabolome analysis. Nature methods, 12(6), 523–526. https://doi.org/10.1038/nmeth.3393
6.Sud M, Fahy E, Cotter D, Brown A, Dennis EA, Glass CK, Merrill AH Jr, Murphy RC, Raetz CR, Russell DW, Subramaniam S., LMSD: LIPID MAPS® structure database Nucleic Acids Research, 35: p. D527-32., DOI: 10.1093/nar/gkl838 , PMID: 17098933
7.Fahy E, Sud M, Cotter D & Subramaniam S., LIPID MAPS® online tools for lipid research Nucleic Acids Research (2007), 35: p. W606-12., DOI: 10.1093/nar/gkm324 , PMID: 17584797
8.Wishart DS, Guo AC, Oler E, et al., HMDB 5.0: the Human Metabolome Database for 2022. Nucleic Acids Res. 2022. Jan 7;50(D1):D622–31. 34986597 
9.Mingxun Wang, Jeremy J. Carver, Vanessa V. Phelan, Laura M. Sanchez, Neha Garg, Yao Peng, Don Duy Nguyen et al. "Sharing and community curation of mass spectrometry data with Global Natural Products Social Molecular Networking." Nature biotechnology 34, no. 8 (2016): 828. PMID: 27504778
10.Li, Y., Kind, T., Folz, J. et al. Spectral entropy outperforms MS/MS dot product similarity for small-molecule compound identification. Nat Methods 18, 1524–1531 (2021). https://doi.org/10.1038/s41592-021-01331-z
11.Kind, T., Fiehn, O. Seven Golden Rules for heuristic filtering of molecular formulas obtained by accurate mass spectrometry. BMC Bioinformatics 8, 105 (2007). https://doi.org/10.1186/1471-2105-8-105