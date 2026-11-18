# cs2-vischeck-extractor

Reads CS2 map VPKs and writes the world collision a vischeck trace uses.

ValvePak stops at the first preloaded directory entry, so `ar_pool_day` looks like it has no world physics. This walks the VPK itself, including a nested `maps/*.vpk`. Only the Default collision group is kept. Clips, ladders, sky, grenade clip, and ConditionallySolid stay out.

## .tri

A `.tri` file is a flat list of triangles, little endian.

```cpp
struct Triangle {
    float p1x, p1y, p1z;
    float p2x, p2y, p2z;
    float p3x, p3y, p3z;
}; // 36 bytes
```

When you load one, skip a triangle if a float is not finite or the three points are the same.

`.vphys` is the PHYS block as text. It is written next to the `.tri` when you ask for it. ValveResourceFormat reads the physics aggregate.

## Run

.NET 10 SDK. `start.bat` asks twice. Maps: 1 official, 2 official plus workshop. Files: 1 `.tri`, 2 `.vphys`, 3 both. Enter keeps 1. Everything lands in `out\`.

```
dotnet run --project src -- --only de_mirage
dotnet run --project src -- --workshop --tri --vphys
dotnet run --project src -- --cs2 "C:\path\to\Counter-Strike Global Offensive"
```

No arguments extracts every official `de_`, `cs_`, and `ar_` map. Vanity, preview, and lobby VPKs are skipped. The install is found through Steam.

## Release

A push to `main` replaces `cs2-vischeck-extractor.exe` on the `latest` release. Windows x64, self-contained.

## Layout

```
src/cli/       arguments and the run
src/maps/      Steam, the install, which maps
src/vpk/       VPK directory and world_physics.vmdl_c
src/physics/   PHYS block, Default collision, hulls, meshes
src/tri/       the 36-byte triangle file
src/vphys/     the physics dump
```

`src/Program.cs` only starts the run.
