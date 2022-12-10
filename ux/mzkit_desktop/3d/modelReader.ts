/**
 * Read of 3d model file blob
*/
class ModelReader {

    private pointCloud: pointCloud[] = [];
    private palette: string[] = [];

    public constructor(private bin: Uint8Array) {
        // npoints
        let view = new DataView(bin, 0, 8);
        let npoints = view.getInt32(0);
        // ncolors
        let ncolors = view.getInt32(4);
        // html color literal string array
        // is fixed size         
        // #rrggbb   
        let colorEnds = 8 + ncolors * 7;
        let stringBuf = bin.slice(8, colorEnds);
        let strings: string = String.fromCharCode.apply(null, stringBuf);

        for (let i = 0; i < ncolors; i++) {
            this.palette.push(strings.substring(i * 7, (i + 1) * 7));
        }

        view = new DataView(bin, colorEnds);

        for (let i = 0; i < npoints; i++) {
            let offset = i * (8 + 8 + 8 + 8 + 4);
            let x = view.getFloat64(offset);
            let y = view.getFloat64(offset + 8);
            let z = view.getFloat64(offset + 16);
            let data = view.getFloat64(offset + 24);
            let clr = view.getInt32(offset + 32);

            this.pointCloud.push(<pointCloud>{
                x: x, y: y, z: z,
                intensity: data,
                color: this.palette[clr]
            });
        }
    }
}

interface pointCloud {
    x: number;
    y: number;
    z: number;
    intensity: number;
    color: number | string;
}