# cs2-vischeck-extractor

Reads Counter-Strike 2 map VPKs and writes the Default world-collision mesh the overlay traces.

A `.tri` file is a raw list of triangles. Each triangle is 36 bytes: nine little-endian floats, three vertices, xyz. The overlay drops non-finite and degenerate triangles.

## Layout

```
src/cli/       arguments and the run
src/maps/      Steam, the CS2 install, which maps
src/vpk/       VPK directory and world_physics.vmdl_c
src/physics/   PHYS block, Default collision, hulls, meshes
src/tri/       the 36-byte triangle file
src/vphys/     the physics dump
```

`src/Program.cs` only starts the run.

## Run

Requires the .NET 10 SDK.

```
dotnet run --project src -- --only de_mirage,de_dust2
dotnet run --project src -- --workshop --tri --vphys
```

No arguments extracts every official `de_`, `cs_`, and `ar_` map into `./out`. `--vphys` writes the physics dump beside the `.tri` files. `--cs2` overrides the install path. Vanity, preview, and lobby VPKs are skipped.

The mesh is the `default` collision group only. Windows, clips, ladders, and sky stay out.

## Release

A push to `main` replaces the `latest` release asset `cs2-vischeck-extractor.exe`. It is a Windows x64 self-contained binary.

## Why these pieces

ValveResourceFormat reads the physics aggregate directly. The VPK directory is read here, because ValvePak stops at the first preloaded entry and then `ar_pool_day` looks like it has no world physics. A nested `maps/*.vpk` is opened the same way.
