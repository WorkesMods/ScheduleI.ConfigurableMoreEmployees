# Configurable More Employees

MelonLoader IL2CPP mod for Schedule I.

## Requirements

- Schedule I IL2CPP
- MelonLoader 0.7.x
- Optional: Mod Manager - Phone App 2.2.3+ / 2.2.4+

The mod uses standard MelonPreferences and works without Mod Manager - Phone App. Edit `UserData\MelonPreferences.cfg` directly when Mod Manager is not installed.

When Mod Manager - Phone App is installed, it can display and edit the same settings in-game. Configurable More Employees listens for Mod Manager save events through reflection, so changed values can be applied without restarting when possible. The phone app is a soft dependency and is not required at build time or runtime.

Employee capacity is only raised after the mod has enough idle points for the configured max. Missing idle points are generated at runtime from a cloned vanilla idle point. If a property cannot place enough idle points, that property is skipped and the rest continue.

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
using UnityEngine;

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
        new IdlePointPlacementArea(
            startLocation => new[]
            {
                new Vector2(startLocation.x - 1f, startLocation.z - 1f),
                new Vector2(startLocation.x + 1f, startLocation.z - 1f),
                new Vector2(startLocation.x + 1f, startLocation.z + 1f),
                new Vector2(startLocation.x - 1f, startLocation.z + 1f)
            },
            new GridIdlePointPlacementStrategy(
                height: null,
                xCount: 2,
                zCount: 2,
                margin: 0.1f,
                yRotation: 0f,
                xDirection: GridAxisDirection.Positive,
                zDirection: GridAxisDirection.Positive,
                fillOrder: GridFillOrder.XMajor))
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

Placement strategies:

- `ExplicitIdlePointPlacementStrategy` uses exact world positions and does not need bounds.
- `GridIdlePointPlacementStrategy` fills an axis-aligned rectangle.
- `OrientedGridIdlePointPlacementStrategy` fills a four-point area whose axes do not need to align to world X/Z.

Use `height: null` when generated idle points should reuse the height of the cloned source idle point. Use a fixed height when a property needs points placed on a specific floor or platform.

Small `ExplicitIdlePointPlacementStrategy` example:

```csharp
new IdlePointPlacementArea(
    new ExplicitIdlePointPlacementStrategy(new[]
    {
        new IdlePointPlacement(new Vector3(10f, 0f, 20f), 180f),
        new IdlePointPlacement(new Vector3(11f, 0f, 20f), 180f)
    }))
```

Small `GridIdlePointPlacementStrategy` example:

```csharp
new IdlePointPlacementArea(
    startLocation => new[]
    {
        new Vector2(10f, 20f),
        new Vector2(14f, 20f),
        new Vector2(14f, 24f),
        new Vector2(10f, 24f)
    },
    new GridIdlePointPlacementStrategy(
        height: null,
        xCount: 2,
        zCount: 2,
        margin: 0.2f,
        yRotation: 180f,
        xDirection: GridAxisDirection.Positive,
        zDirection: GridAxisDirection.Positive,
        fillOrder: GridFillOrder.XMajor))
```

Small `OrientedGridIdlePointPlacementStrategy` example:

```csharp
new IdlePointPlacementArea(
    startLocation => new[]
    {
        new Vector2(10f, 20f),
        new Vector2(13f, 22f),
        new Vector2(11f, 25f),
        new Vector2(8f, 23f)
    },
    new OrientedGridIdlePointPlacementStrategy(
        height: 0f,
        columnCount: 2,
        rowCount: 2,
        margin: 0.2f,
        yRotation: 45f,
        columnDirection: OrientedGridColumnDirection.PointAToPointB,
        rowDirection: OrientedGridRowDirection.PointAToPointD,
        fillOrder: OrientedGridFillOrder.ColumnMajor))
```

## How It Works

Configurable More Employees does not apply employee capacity changes during initial game launch. At that point the game is still in menu/bootstrap flow, and the world/property objects either do not exist yet or are not the runtime instances that should be mutated.

The mod waits for a real world load, then binds once for that world instance:

- `LoadManager.StartGame`, `LoadLastSave`, `LoadAsClient`, and `LoadTutorialAsClient` mark the start of a world load.
- `LoadManager.ExitToMenu` clears world state because old Unity objects are no longer trustworthy.
- `LoadManager.set_IsGameLoaded(true)` marks useful world-load completion.
- `LoadManager.set_IsGameLoaded(false)` is ignored because it fires repeatedly during startup and menu transitions.
- `Property.Start` is counted as properties come alive.

The bind/apply step is allowed only after all required property definitions have reached `Property.Start`. Earlier versions used an observed vanilla property-start count of `13`, which was larger than the supported property list because the game also starts business/RV-like/non-target properties. The current approach tracks the required definitions themselves instead, so custom-property mods can participate in readiness by registering a required definition rather than depending on a hardcoded vanilla count.

Once binding is allowed, the mod finds the configured properties, locates their employee-capacity data, finds an existing idle point to use as a clone template, generates any missing idle points required for the configured caps, and only then raises capacity. If enough idle points cannot be generated for one property, that property is skipped while the rest continue.

This timing matters because the game loads saved employees and validates assignments while the world is loading. Applying too early can hit menu/bootstrap objects or incomplete property instances. Applying too late can let the game restore employees against vanilla capacities, causing extra employees to be lost or fail to restore correctly. The current timing is the tested compromise: wait for real world objects, ignore noisy unload signals, bind once per world instance, and reset cleanly when leaving or rejoining.

During development, a temporary `LoadTracePatches.cs` file hooked many load, property, employee, assignment, and Manny dialogue methods to print timing diagnostics. It confirmed the lifecycle above and was removed afterward because keeping it would add unnecessary Harmony patches, log noise, and maintenance risk.

Mod Manager - Phone App integration is reflection-based on purpose. The mod should compile and run as a single DLL with only MelonPreferences available, while still reacting to Mod Manager save events when that optional mod is installed.

Property metadata lives in `PropertyDefinition` and `PropertyDefinitionRegistry` so vanilla properties and externally registered properties follow the same path. This keeps preference creation, binding, idle point generation, Manny dialogue support, and future custom-property support from splitting into separate special cases.
