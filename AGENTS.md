# cs2-vischeck-extractor

Windows command that turns a CS2 map VPK into the `.tri` mesh the overlay traces.

Behavior of the mesh is the file it writes. Law for the tree: `.cursor/rules/architecture.mdc`.

## Product

- Find CS2, then the official `de_` / `cs_` / `ar_` maps. Workshop only with `--workshop`.
- Read `world_physics.vmdl_c` from the map VPK. Skip sky.
- Keep the Default collision group.
- Write `{map}.tri`: 36 bytes per triangle, nine little-endian floats.

## Rules

1. Domain folders only: `cli`, `maps`, `vpk`, `physics`, `tri`.
2. One concern per file. Split before a file is hard to scan.
3. `Program.cs` starts the run and does nothing else.
4. Domain code returns data. `cli` prints.
5. No `utils`, `common`, `misc`, or `helpers`.
6. Do not add a second mesh format. This tool writes `.tri`.
7. Default collision only, until a change says otherwise.
