namespace molmil {

    export class _shaderEngine {

        // ** initiates the shaders **
        public code: {} = {};

        recompile(renderer) {
            var global_defines = ""
            if (molmil.configBox.glsl_fog) global_defines += "#define ENABLE_FOG 1\n";
            if (renderer.settings.slab) global_defines += "#define ENABLE_SLAB 1\n"

            for (var s in renderer.shaders) renderer.shaders[s].compile(global_defines);
        }
    }

    export const shaderEngine = new _shaderEngine();

    // ** initializes a shader **
    export function setupShader(gl, name, program, src, type) {
        //var defines = "";
        //if (document.BrowserType.MSIE) defines += "# define MSIE";
        //src = defines+"\n"+src;
        var shader = gl.createShader(type);
        gl.shaderSource(shader, src);
        gl.compileShader(shader);
        if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) { console.log(name + ":\n" + gl.getShaderInfoLog(shader)); return null; }
        gl.attachShader(program, shader);
        return shader;
    }
}