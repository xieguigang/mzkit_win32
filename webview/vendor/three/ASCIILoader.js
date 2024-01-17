import { FileLoader, Loader, Matrix4, Vector3 } from "three";
import * as fflate from "./fflate.module.js";
import { Volume } from "./Volume.js";

class ASCIILoader extends Loader {   

    constructor(manager) {
        super(manager);
    }

    /**
     * @param {string} url a url folder path for host the 3d model data
    */
    load(url, onLoad, onProgress, onError) {
        const scope = this;

        // load a data header and data bytes
        // the data header just contains the dimension size of [x,y,z]
        // the data bytes just contains the intensity value encoded in
        // bytes, each byte is a spot intensity value
        const header_url = `${url}/header.json`;
        const data_url = `${url}/data/`;

        $ts.getText(header_url, function (json) {
            const dims = JSON.parse(json);
            const loader = new FileLoader(scope.manager);

            loader.setPath(scope.path);
            loader.setResponseType("arraybuffer");
            loader.setRequestHeader(scope.requestHeader);
            loader.setWithCredentials(scope.withCredentials);
            loader.load(data_url, function (data) {
                try {
                    onLoad(scope.parse(data, dims));
                } catch (e) {
                    if (onError) {
                        onError(e);
                    } else {
                        console.error(e);
                    }

                    scope.manager.itemError(url);
                }
            },
                onProgress,
                onError
            );
        });
    }

    /**
     *
     * @param {boolean} segmentation is a option for user to choose
    */
    setSegmentation(segmentation) {
        this.segmentation = segmentation;
    }

    /**
     * construct a volume object, and return it to the 3d engine
     * 
     * @param {ArrayBuffer} data the model data in bytes
     * @param {number[]} dims the model dimension value, in axis order [x,y,z]
     * 
     * @returns {Volume} the 3d model data
    */
    parse(data, dims) {
        const volume = new Volume();
        const transitionMatrix = new Matrix4();

        // get the image dimensions
        volume.dimensions = dims;
        volume.xLength = volume.dimensions[0];
        volume.yLength = volume.dimensions[1];
        volume.zLength = volume.dimensions[2];
        volume.axisOrder = ["x", "y", "z"];
        // Create IJKtoRAS matrix
        volume.matrix = new Matrix4();
        volume.matrix.set(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

        transitionMatrix.set(-1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

        volume.inverseMatrix = new Matrix4();
        volume.inverseMatrix.copy(volume.matrix).invert();

        return volume;
    }
}

export { ASCIILoader };