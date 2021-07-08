import * as THREE from "three";

/*
 * A shader for the terrain of the map. It highlights the edges
 * (detected by color changes in nearby pixels) of the map texture.
 */
export const terrainShader: THREE.ShaderMaterialParameters = {
    fragmentShader: [
        "uniform sampler2D tColor;",
        "uniform float tPixelSize;",
        "varying vec2 vUv;",
        "void main() {",
        "vec4 texel = texture2D( tColor, vUv );",
        "float multi = 0.1;",
        "for(float x = -1.0; x <= 1.0; x++) {",
        "for (float y = -1.0; y <= 1.0; y++) {",
        "if (x == 0.0 && y == 0.0) continue;",
        "vec4 neighborcolor = texture2D(tColor, vUv + vec2(x/256.0, y/256.0) * tPixelSize);",
        "if (neighborcolor != texel) { multi = 1.0; }",
        "}",
        "}",
        "gl_FragColor = texel * multi;",
        "}"
    ].join("\n"),

    side: THREE.DoubleSide,

    uniforms: {
        tColor: { type: "t", value: 0 },
        tPixelSize: { type: "f", value: 0.3 },
    },

    vertexShader: [
        "varying vec2 vUv;",
        "void main() {",
        "vUv = vec2(uv.x, uv.y);",
        "gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);",
        "}"
    ].join("\n"),
};
