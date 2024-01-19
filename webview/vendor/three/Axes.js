import { SphereGeometry, CylinderGeometry, Group, Mesh, MeshStandardMaterial } from "three";

class Axes extends Group {
    constructor() {
        super();
        // 坐标轴：圆柱体和圆锥组成一条轴
        const cylinder = new CylinderGeometry(0.03, 0.03, 2, 16);
        const arrow = new CylinderGeometry(0, 0.06, 0.2);
        const sphere = new SphereGeometry(0.06);
        // 材质：RGB对应XYZ
        const red = new MeshStandardMaterial({ color: 0xff0000 });
        const green = new MeshStandardMaterial({ color: 0x00ff00 });
        const blue = new MeshStandardMaterial({ color: 0x0000ff });
        const gold = new MeshStandardMaterial({ color: "gold" });

        // y轴
        const axes_y = new Group();
        const y_line = new Mesh(cylinder, green);
        const y_arrow = new Mesh(arrow, green);
        y_arrow.position.y += 1.04;
        axes_y.add(y_line, y_arrow);
        axes_y.position.y += 1;

        // x轴
        const axes_x = new Group();    // group默认的中心点是(0,0,0)
        const x_line = new Mesh(cylinder, red);
        const x_arrow = new Mesh(arrow, red);
        // 这里只是箭头自身的平移，不会改变group的中心点，所以group的中心点还是(0,0,0)
        x_arrow.position.y += 1.04;
        axes_x.add(x_line, x_arrow);
        axes_x.position.x += 1;
        axes_x.rotation.z = -Math.PI / 2;

        // z轴
        const axes_z = new Group();
        const z_line = new Mesh(cylinder, blue);
        const z_arrow = new Mesh(arrow, blue);
        z_arrow.position.y += 1.04;
        axes_z.add(z_line, z_arrow);
        axes_z.position.z += 1;
        axes_z.rotation.x = Math.PI / 2;

        // 原点
        const origin = new Mesh(sphere, gold);

        this.add(
            axes_x,
            axes_y,
            axes_z,
            origin
        );
    }
}

export { Axes };