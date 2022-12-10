/**
 * Read of 3d model file blob
*/
class ModelReader {

    private pointCloud: pointCloud[] = [];
    private palette: string[] = [];

    /**
     * @param bin the data should be in network byte order
    */
    public constructor(private bin: Uint8Array) {
        // npoints
        let view = new DataView(bin.buffer, 0, 8);
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

        view = new DataView(bin.buffer, colorEnds);

        for (let i = 0; i < npoints; i++) {
            let offset = i * (8 + 8 + 8 + 8 + 4);
            let x = view.getFloat64(offset) / 10;
            let y = view.getFloat64(offset + 8) / 10;
            let z = view.getFloat64(offset + 16) / 10;
            let data = view.getFloat64(offset + 24);
            let clr = view.getInt32(offset + 32);

            this.pointCloud.push(<pointCloud>{
                x: x, y: y, z: z,
                intensity: data,
                color: this.palette[clr]
            });
        }

        this.centroid();
        this.cubic_scale();
    }

    private cubic_scale() {
        const v = this.getVector3();
        const cubic = new data.NumericRange(-100, 100);
        const x = new data.NumericRange($ts(v.x).Min(), $ts(v.x).Max());
        const y = new data.NumericRange($ts(v.y).Min(), $ts(v.y).Max());
        const z = new data.NumericRange($ts(v.z).Min(), $ts(v.z).Max());
        const rad = 45 * Math.PI / 180;
        const cosa = Math.cos(rad);
        const sina = Math.sin(rad);

        let Xn = 0;
        let Zn = 0;
        let f = Math.pow(10, 15);

        for (let point of this.pointCloud) {
            point.x = x.ScaleMapping(point.x, cubic);
            point.y = y.ScaleMapping(point.y, cubic);
            point.z = z.ScaleMapping(point.z, cubic);

            Zn = point.z * cosa - point.x * sina;
            Xn = point.z * sina + point.x * cosa;

            point.x = Xn;
            point.z = Zn * f;
        }
    }

    private centroid() {
        const v = this.getVector3();
        const offset_x = $ts(v.x).Sum() / this.pointCloud.length;
        const offset_y = $ts(v.y).Sum() / this.pointCloud.length;
        const offset_z = $ts(v.z).Sum() / this.pointCloud.length;

        for (let point of this.pointCloud) {
            point.x = point.x - offset_x;
            point.y = point.y - offset_y;
            point.z = point.z - offset_z;
        }
    }

    private getVector3() {
        const x: number[] = [];
        const y: number[] = [];
        const z: number[] = [];

        for (let point of this.pointCloud) {
            x.push(point.x);
            y.push(point.y);
            z.push(point.z);
        }

        return {
            x: x,
            y: y,
            z: z
        };
    }

    public loadPointCloudModel(canvas: apps.three_app) {
        // 轴辅助 （每一个轴的长度）
        // 红色代表 X 轴. 绿色代表 Y 轴. 蓝色代表 Z 轴.
        var object = new THREE.AxesHelper(300);
        //创建THREE.PointCloud粒子的容器
        var geometry = new THREE.Geometry();
        //创建THREE.PointCloud纹理
        var material = new THREE.PointCloudMaterial(<any>{
            size: 0.5,
            vertexColors: true,
            color: 0xffffff
        });

        canvas.scene.add(object);

        //循环将粒子的颜色和位置添加到网格当中
        // for (var x = -5; x <= 5; x++) {
        //     for (var y = -5; y <= 5; y++) {
        //         var particle = new THREE.Vector3(x * 10, y * 10, 0);
        //         geometry.vertices.push(particle);
        //         geometry.colors.push(new THREE.Color(+three_app.randomColor()));
        //     }
        // }
        for (let point of this.pointCloud) {
            var particle = new THREE.Vector3(point.x, point.y, point.z);

            geometry.vertices.push(particle);
            geometry.colors.push(new THREE.Color(<string>point.color));
        }

        //实例化THREE.PointCloud
        canvas.scene.add(new THREE.PointCloud(geometry, material));
    }
}

interface pointCloud {
    x: number;
    y: number;
    z: number;
    intensity: number;
    color: number | string;
}