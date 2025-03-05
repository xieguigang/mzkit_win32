/*!
 * molmil.js
 *
 * Molmil molecular web viewer: https://github.com/gjbekker/molmil
 *
 * By Gert-Jan Bekker
 * License: LGPLv3
 *   See https://github.com/gjbekker/molmil/blob/master/LICENCE.md
 */
declare var molmil: any;
declare function findResidueRings(molObj: any): any;
declare function renderHbonds(pairs: any, soup: any, settings: any): any;
declare function renderPIinteractions(pairs: any, soup: any): any;
declare function renderSaltBridges(pairs: any, soup: any): any[];
declare function processExternalCommand(cmd: any, commandBuffer: any): void;
