# kDecals
### Projection Decals for Unity's Universal Render Pipeline.

![alt text](https://github.com/Kink3d/kDecals/wiki/Images/Home00.png?raw=true)
*An example of Decals in a scene.*

kDecals is a system for definition, placement and rendering of projection Decals in Unity's Universal Render Pipeline. It comes with Lit and Unlit Decal types by default and supports custom Decal shaders using a predefined Shader Library. kDecals supports Decal creation in Editor and runtime and supports pooling of runtime decals by default via [kPooling](https://github.com/Kink3d/kPooling).

Refer to the [Wiki](https://github.com/Kink3d/kDecals/wiki/Home) for more information.

## Instructions
- Open your project manifest file (`MyProject/Packages/manifest.json`).
- Add `"com.kink3d.decals": "https://github.com/Kink3d/kDecals.git"` to the `dependencies` list.
- kDecals has a dependency on [kPooling](https://github.com/Kink3d/kPooling). Add `"com.kink3d.pooling": "https://github.com/Kink3d/kPooling.git"` to the `dependencies` list.
- Open or focus on Unity Editor to resolve packages.

## Requirements
- Unity 2019.3.0f3 or higher.