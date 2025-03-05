namespace molmil {

    export function invertColor(hexTripletColor) { return "#" + ("000000" + (0xFFFFFF ^ parseInt(hexTripletColor.substring(1), 16)).toString(16)).slice(-6); }
    export function componentToHex(c) { var hex = c.toString(16); return hex.length == 1 ? "0" + hex : hex; }
    export function rgb2hex(r, g, b) { return "#" + molmil.componentToHex(r) + molmil.componentToHex(g) + molmil.componentToHex(b); }
    export function hex2rgb(hex) { hex = (hex.charAt(0) == "#" ? hex.substr(1, 7) : hex); return [parseInt(hex.substring(0, 2), 16), parseInt(hex.substring(2, 4), 16), parseInt(hex.substring(4, 6), 16)]; }

}