# Configurable More Employees

MelonLoader IL2CPP mod for Schedule I.

## Requirements

- Schedule I IL2CPP
- MelonLoader 0.7.x
- Optional: Mod Manager - Phone App 2.2.3+ / 2.2.4+

The mod uses standard MelonPreferences and works without Mod Manager - Phone App. Edit `UserData\MelonPreferences.cfg` directly when Mod Manager is not installed.

When Mod Manager - Phone App is installed, it can display and edit the same settings in-game. Configurable More Employees listens for Mod Manager save events through reflection, so changed values can be applied without restarting when possible. The phone app is a soft dependency and is not required at build time or runtime.

Employee capacity is only raised after the mod has enough idle points for the configured max. Missing idle points are generated at runtime from a cloned vanilla idle point.

If a configured employee cap is higher than the number of supported idle point placements for that property, the value is clamped to the highest supported value and a warning is written to the MelonLoader log. The saved preference is not rewritten.

Supported properties:

- Storage Unit
- Sweatshop
- Bungalow
- Barn
- Docks Warehouse
- Manor
- Motel Room
- Sewer Office

Default max employee settings:

```text
StorageUnitMaxEmployees = 5
SweatshopMaxEmployees = 3
BungalowMaxEmployees = 7
BarnMaxEmployees = 13
ManorMaxEmployees = 17
DocksWarehouseMaxEmployees = 17
MotelMaxEmployees = 0
SewerMaxEmployees = 0
```

Existing preference keys are kept stable for config compatibility. Motel Room and Sewer Office default to `0`, so they must be enabled explicitly before Manny can offer them as employee destinations.

If you want to configure the mod before launching the game with it for the first time, paste a block like this into `UserData\MelonPreferences.cfg`. Values above the supported placement count are allowed; the mod will clamp them to the highest supported value and write a warning to the MelonLoader log.

```toml
["Configurable More Employees"]
# Maximum employees allowed at the Storage Unit.
StorageUnitMaxEmployees = 100
# Maximum employees allowed at the Sweatshop.
SweatshopMaxEmployees = 100
# Maximum employees allowed at the Bungalow.
BungalowMaxEmployees = 100
# Maximum employees allowed at the Barn.
BarnMaxEmployees = 15
# Maximum employees allowed at the Manor.
ManorMaxEmployees = 100
# Maximum employees allowed at the Docks Warehouse.
DocksWarehouseMaxEmployees = 100
# Maximum employees allowed at the Motel.
MotelMaxEmployees = 100
# Maximum employees allowed at the Sewer Office.
SewerMaxEmployees = 100

["Configurable More Employees Debug"]
# Shows visible in-world markers for vanilla and generated employee idle points.
DebugShowIdlePointMarkers = true
# Logs detailed placement decisions.
DebugVerboseLogging = true
```

Debug settings are kept in the `Configurable More Employees Debug` category:

```text
DebugShowIdlePointMarkers = false
DebugVerboseLogging = false
```

When `DebugShowIdlePointMarkers` is enabled, the mod shows small in-world spheres for idle point debugging. Green markers are vanilla idle points, cyan markers are generated idle points that are active for the current configured cap, and purple markers are supported generated placements that are not active because the configured cap is lower.

In UnityExplorer, the marker root is created under the property object as `ConfigurableMoreEmployees_DebugMarkers_{PropertyKey}`. Individual marker objects are named `{PropertyKey}_{Index}`. Moving these marker objects around in UnityExplorer is a useful way to discover where an `x,y,z` position lands in-game before turning that position into a placement rule (Useful if you are adding a custom property to this mod, more about that later).

## Build

Copy `.envExample`, paste it in the same folder, rename the copy to `.env`, then edit `GameDir` if your Schedule I install is not at the default Steam path.

From this folder:

```powershell
dotnet build -c Release
```

You can also override the game folder for a single build:

```powershell
dotnet build -c Release -p:GameDir="D:\SteamLibrary\steamapps\common\Schedule I"
```

The built DLL will be at:

```text
bin\Release\net6.0\ConfigurableMoreEmployees.dll
```

Copy that DLL into:

```text
<Schedule I>\Mods
```

The Release build also copies the DLL there automatically.

To verify without copying the DLL into the game folder:

```powershell
dotnet build -c Release --no-restore -p:GameDir="C:\Program Files (x86)\Steam\steamapps\common\Schedule I" -p:InstallAfterBuild=false
```

### Troubleshooting

If the build says `GameDir does not contain generated IL2CPP assemblies`, launch Schedule I with MelonLoader once, wait for the main menu, then close the game and build again. MelonLoader generates the required `MelonLoader\Il2CppAssemblies` files during that first launch.

## Extension API

The custom property integration API is intended to be stable, but it is currently untested with external mods. If you run into problems while integrating with it, please feel free to create an issue on GitHub with the property definition you tried and any relevant log output.

Other mods can register additional properties at runtime:

```csharp
using ConfigurableMoreEmployees;
using static ConfigurableMoreEmployees.GridAxisDirection;
using static ConfigurableMoreEmployees.GridFillOrder;
using static ConfigurableMoreEmployees.IdlePointPlacementDsl;

ConfigurableMoreEmployeesApi.RegisterProperty(new PropertyDefinition(
    key: "ExampleProperty",
    gameObjectName: "ExampleProperty",
    displayName: "Example Property",
    propertyCode: "exampleproperty",
    vanillaIdlePointCount: 0,
    defaultMaxEmployees: 0,
    addMannyDialogueChoice: true,
    placementAreas: new[]
    {
        Grid(
            Box(
                Point2(10f, 20f),
                Point2(14f, 24f)),
            height: null,
            xCount: 2,
            zCount: 2,
            yRotation: 0f,
            xDirection: Positive,
            zDirection: Positive,
            fillOrder: XMajor,
            margin: 0.1f)
    }));
```

Register custom properties before properties are bound during world loading. `key` and `gameObjectName` must be unique. If `preferenceKey` is omitted, the mod creates one from `key` with `MaxEmployees` appended.

Property definition fields:

- `key`: stable unique identifier used by this mod.
- `gameObjectName`: exact scene object name used to find the property.
- `displayName`: human-readable name used in logs and preference descriptions.
- `propertyCode`: game employee assignment code for the property.
- `vanillaIdlePointCount`: number of idle points the property normally has.
- `defaultMaxEmployees`: default config value for the property.
- `addMannyDialogueChoice`: whether Manny should offer this property when owned and enabled.
- `placementAreas`: rules for generated idle point positions.
- `preferenceKey`: optional explicit MelonPreferences key. Existing keys should not be renamed once released.
- `preferenceDescription`: optional config UI/help text.
- `required`: whether the property must be present before the mod binds and applies employee limits. Vanilla definitions are required. Custom definitions default to optional, but a custom-property mod can set this to `true` if its property should participate in the load-readiness gate.

Placement helpers:

- `Explicit(...)` uses exact world positions and does not need bounds.
- `Grid(Box(...), ...)` fills an axis-aligned rectangle created from two opposite corners.
- `OrientedGrid(OrientedBox(...), ...)` fills a three-point oriented area whose axes do not need to align to world X/Z. The fourth corner is derived internally.

Use `height: null` when generated idle points should reuse the height of the cloned source idle point. Use a fixed height when a property needs points placed on a specific floor or platform.

Coordinate notes:

- `Point(x, y, z, yRotation)` creates a 3D idle point. The fourth value is the Y-axis rotation in degrees, which controls which way the employee faces.
- `Point2(x, z)` creates a 2D X/Z coordinate for bounds.
- `Box(pointA, pointC)` creates a straight, axis-aligned rectangle from two opposite corners.
- `OrientedBox(pointA, pointB, pointC)` creates an angled rectangle from three corners. `pointA -> pointB` is one edge of the rectangle, `pointB -> pointC` is the adjacent edge, and the fourth corner is derived as `pointA + (pointC - pointB)`.
- For `OrientedGrid(...)`, the point order gives the derived bounds corners in `A, B, C, D` order. `columnDirection` chooses which bounds edge is the column axis, such as `PointAToPointB` or `PointBToPointA`. `rowDirection` chooses which adjacent edge is the row axis, such as `PointAToPointD` or `PointBToPointC`. So the same three oriented-box points can be filled in different directions by changing the column and row directions.

Small `Explicit(...)` example:

```csharp
Explicit(
    Point(10f, 0f, 20f, 180f),
    Point(11f, 0f, 20f, 180f))
```

Small `Grid(Box(...), ...)` example:

```csharp
Grid(
    Box(
        Point2(10f, 20f),
        Point2(14f, 24f)),
    height: null,
    xCount: 2,
    zCount: 2,
    yRotation: 180f,
    xDirection: Positive,
    zDirection: Positive,
    fillOrder: XMajor)
```

Small `OrientedGrid(OrientedBox(...), ...)` example:

```csharp
OrientedGrid(
    OrientedBox(
        pointA: Point2(10f, 20f),
        pointB: Point2(13f, 22f),
        pointC: Point2(11f, 25f)),
    height: 0f,
    columnCount: 2,
    rowCount: 2,
    yRotation: 45f,
    columnDirection: OrientedGridColumnDirection.PointAToPointB,
    rowDirection: OrientedGridRowDirection.PointAToPointD,
    fillOrder: OrientedGridFillOrder.ColumnMajor)
```

## How It Works

Configurable More Employees does not apply employee capacity changes during initial game launch. At that point the game is still in menu/bootstrap flow, and the world/property objects either do not exist yet or are not the runtime instances that should be mutated.

The mod waits for a real world load, then binds once for that world instance:

- `LoadManager.StartGame`, `LoadLastSave`, `LoadAsClient`, and `LoadTutorialAsClient` mark the start of a world load.
- `LoadManager.ExitToMenu` clears world state because old Unity objects are no longer trustworthy.
- `LoadManager.set_IsGameLoaded(true)` marks useful world-load completion.
- `LoadManager.set_IsGameLoaded(false)` is ignored because it fires repeatedly during startup and menu transitions.
- `Property.Start` is counted as properties come alive.

The bind/apply step is allowed only after all required property definitions have reached `Property.Start`. The current approach tracks the required definitions themselves, so custom-property mods can participate in readiness by registering a required definition rather than depending on a hardcoded vanilla count.

Once binding is allowed, the mod finds the configured properties, locates their employee-capacity data, finds an existing idle point to use as a clone template, generates any missing idle points required for the configured caps, and only then raises capacity. If a configured cap is higher than the available placement rules support, that property is clamped to the supported maximum.

This timing matters because the game loads saved employees and validates assignments while the world is loading. Applying too early can hit menu/bootstrap objects or incomplete property instances. Applying too late can let the game restore employees against vanilla capacities, causing extra employees to be lost or fail to restore correctly. The current timing is the tested compromise: wait for real world objects, ignore noisy unload signals, bind once per world instance, and reset cleanly when leaving or rejoining.

During development, a temporary `LoadTracePatches.cs` file hooked many load, property, employee, assignment, and Manny dialogue methods to print timing diagnostics. It confirmed the lifecycle above and was removed afterward because keeping it would add unnecessary Harmony patches, log noise, and maintenance risk.

Mod Manager - Phone App integration is reflection-based on purpose. The mod should compile and run as a single DLL with only MelonPreferences available, while still reacting to Mod Manager save events when that optional mod is installed.

Property metadata lives in `PropertyDefinition` and `PropertyDefinitionRegistry` so vanilla properties and externally registered properties follow the same path. This keeps preference creation, binding, idle point generation, Manny dialogue support, and future custom-property support from splitting into separate special cases.
