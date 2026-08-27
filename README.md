# Configurable More Employees

MelonLoader IL2CPP mod for Schedule I.

## Requirements

- Schedule I IL2CPP
- MelonLoader 0.7.x
- Optional: Mod Manager - Phone App 2.2.3+ / 2.2.4+, installed as `Mods\ModManager&PhoneApp.dll`

The mod uses standard MelonPreferences and works without Mod Manager - Phone App. Edit `UserData\MelonPreferences.cfg` directly when Mod Manager is not installed.

When Mod Manager - Phone App is installed, it can display and edit the same settings in-game. Configurable More Employees listens for Mod Manager save events through reflection, so changed values can be applied without restarting when possible. The phone app is a soft dependency and is not required at build time or runtime.

Employee capacity is only raised after the mod has enough idle points for the configured max. Missing idle points are generated at runtime from a cloned vanilla idle point. If a property cannot place enough idle points, that property is skipped and the rest continue.

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

When `DebugShowIdlePointMarkers` is enabled, the mod shows small in-world spheres at every current idle point. Green markers are vanilla idle points and cyan markers are generated idle points.

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

## Extension API

Other mods can register additional properties at runtime:

```csharp
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
