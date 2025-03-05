namespace molmil {

    // ** camera object **
    export class glCamera {

        x;
        y;
        z;
        QPitch;
        QHeading;
        QView;
        QRoll;
        Qtmp;
        pitchAngle;
        vrXYZ;
        vrXYZupdated;
        headingAngle;
        rollAngle;

        constructor() {
            this.reset();
        }

        reset() {
            this.x = this.y = this.z = 0.0;
            this.QPitch = quat.create();
            this.QHeading = quat.create();
            this.QView = quat.create();
            this.QRoll = quat.create();
            this.Qtmp = quat.create();
            this.pitchAngle = this.headingAngle = this.rollAngle = 0.0;
            this.vrXYZ = [0.0, 0.0, 0.0];
            this.vrXYZupdated = false;
        }

        generateMatrix() {
            var matrix = mat4.create(); mat4.fromQuat(matrix, this.QView);
            matrix[12] = this.x;
            matrix[13] = this.y;
            matrix[14] = this.z;
            return matrix;
        }

        positionCamera() {
            quat.setAxisAngle(this.QPitch, [1, 0, 0], (this.pitchAngle / 180) * Math.PI);
            quat.setAxisAngle(this.QHeading, [0, 1, 0], (this.headingAngle / 180) * Math.PI);
            quat.setAxisAngle(this.QRoll, [0, 0, 1], (this.rollAngle / 180) * Math.PI);
            var q = quat.create();
            quat.multiply(q, this.QPitch, this.QHeading); quat.multiply(q, q, this.QRoll);
            quat.multiply(this.QView, q, this.QView);
            this.headingAngle = this.pitchAngle = this.rollAngle = 0.0;
        }

    }
}