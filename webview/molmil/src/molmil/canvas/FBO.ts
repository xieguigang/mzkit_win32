namespace molmil {

    export class FBO {


        // fbo

        constructor(gl, width, height) {
            this.width = width; this.height = height; this.gl = gl;
            this.textures = {}; // textureID, GLTextureID, colourNumber, internalFormat, format
            this.colourNumber = 0;
            this.fbo = null;
            this.depthBuffer = null;
            this.multisample = false;
        }

        addTexture(textureID, internalFormat, format) {
            if (textureID in this.textures) { this.gl.bindTexture(this.gl.TEXTURE_2D, this.textures[textureID][0]); }
            else {
                var texture = this.gl.createTexture();
                this.textures[textureID] = [texture, this.colourNumber++, internalFormat, format];
                this.gl.bindTexture(this.gl.TEXTURE_2D, texture);
                this.gl.texParameteri(this.gl.TEXTURE_2D, this.gl.TEXTURE_MAG_FILTER, this.gl.NEAREST);
                this.gl.texParameteri(this.gl.TEXTURE_2D, this.gl.TEXTURE_MIN_FILTER, this.gl.NEAREST);
                this.gl.texParameteri(this.gl.TEXTURE_2D, this.gl.TEXTURE_WRAP_S, this.gl.CLAMP_TO_EDGE);
                this.gl.texParameteri(this.gl.TEXTURE_2D, this.gl.TEXTURE_WRAP_T, this.gl.CLAMP_TO_EDGE);
            }
            this.gl.texImage2D(this.gl.TEXTURE_2D, 0, internalFormat, this.width, this.height, 0, format, this.gl.UNSIGNED_BYTE, null);
            this.gl.bindTexture(this.gl.TEXTURE_2D, null);
        };

        setup() {
            if (this.fbo != null) this.rebindTextures(true);
            else {
                if (this.multisample && this.multisample in this.textures && molmil.configBox.webGL2) {
                    var texture = this.textures[this.multisample];
                    this.fbo2 = this.gl.createFramebuffer();
                    this.colorRenderbuffer = this.gl.createRenderbuffer();
                    this.gl.bindRenderbuffer(this.gl.RENDERBUFFER, this.colorRenderbuffer);
                    this.gl.renderbufferStorageMultisample(this.gl.RENDERBUFFER, 4, this.gl.RGBA, this.width, this.height);

                    this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, this.fbo);
                    this.gl.framebufferRenderbuffer(this.gl.FRAMEBUFFER, this.gl.COLOR_ATTACHMENT0 + texture[1], this.gl.RENDERBUFFER, this.colorRenderbuffer);
                    this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, null);

                    this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, this.fbo2);
                    this.gl.framebufferTexture2D(this.gl.FRAMEBUFFER, this.gl.COLOR_ATTACHMENT0 + texture[1], this.gl.TEXTURE_2D, texture[0], 0);
                    this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, null);
                }
                else {
                    this.multisample = false;

                    this.fbo = this.gl.createFramebuffer();
                    this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, this.fbo);
                    this.rebindTextures(false);
                    this.depthBuffer = this.gl.createRenderbuffer();
                    this.gl.bindRenderbuffer(this.gl.RENDERBUFFER, this.depthBuffer);
                    this.gl.renderbufferStorage(this.gl.RENDERBUFFER, this.gl.DEPTH_COMPONENT16, this.width, this.height); // this.gl.DEPTH_COMPONENT16 --> GL_DEPTH_COMPONENT24
                    this.gl.framebufferRenderbuffer(this.gl.FRAMEBUFFER, this.gl.DEPTH_ATTACHMENT, this.gl.RENDERBUFFER, this.depthBuffer);
                    this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, null);
                }

            }
        };

        post() {
            if (this.multisample) {
                this.gl.bindFramebuffer(this.gl.READ_FRAMEBUFFER, this.fbo);
                this.gl.bindFramebuffer(this.gl.DRAW_FRAMEBUFFER, this.fbo2);
                this.gl.clearBufferfv(this.gl.COLOR, 0, [0.0, 0.0, 0.0, 1.0]);
                this.gl.blitFramebuffer(
                    0, 0, this.width, this.height,
                    0, 0, this.width, this.height,
                    this.gl.COLOR_BUFFER_BIT, this.gl.NEAREST
                );
            }
        };

        rebindTextures(unbind) {
            this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, this.fbo);
            for (var t in this.textures) this.gl.framebufferTexture2D(this.gl.FRAMEBUFFER, this.gl.COLOR_ATTACHMENT0 + this.textures[t][1], this.gl.TEXTURE_2D, this.textures[t][0], 0);
            this.gl.framebufferRenderbuffer(this.gl.FRAMEBUFFER, this.gl.DEPTH_ATTACHMENT, this.gl.RENDERBUFFER, this.depthBuffer);
            if (unbind) this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, null);

        };

        bindTextureToUniform(textureID, uniformLocation, bindLocation) {
            this.gl.activeTexture(this.gl.TEXTURE0 + bindLocation);
            this.gl.bindTexture(this.gl.TEXTURE_2D, this.textures[textureID][0]);
            this.gl.uniform1i(uniformLocation, bindLocation);
        };

        resize(width, height) {
            if (width == this.width && height == this.height) return;
            this.width = width; this.height = height;
            for (var t in this.textures) this.addTexture(t, this.textures[t][2], this.textures[t][3]);
            this.rebindTextures(false);
            this.gl.bindRenderbuffer(this.gl.RENDERBUFFER, this.depthBuffer);
            this.gl.renderbufferStorage(this.gl.RENDERBUFFER, this.gl.DEPTH_COMPONENT16, this.width, this.height);
            this.gl.framebufferRenderbuffer(this.gl.FRAMEBUFFER, this.gl.DEPTH_ATTACHMENT, this.gl.RENDERBUFFER, this.depthBuffer);
            this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, null);

            if (this.multisample) {
                this.gl.bindRenderbuffer(this.gl.RENDERBUFFER, this.colorRenderbuffer);
                this.gl.renderbufferStorageMultisample(this.gl.RENDERBUFFER, 4, this.gl.RGBA8, this.width, this.height);
                this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, this.fbo2);
                this.gl.framebufferRenderbuffer(this.gl.FRAMEBUFFER, this.gl.COLOR_ATTACHMENT0, this.gl.RENDERBUFFER, this.colorRenderbuffer);
                this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, null);
            }
        };

        bind() {
            this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, this.fbo);
            //if (this.multisample) this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, this.fbo2);
        };

        unbind() {
            this.gl.bindFramebuffer(this.gl.FRAMEBUFFER, null);
        };
    }
}