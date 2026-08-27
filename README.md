# Configurable More Employees

MelonLoader IL2CPP mod for Schedule I.

## Requirements

- Schedule I IL2CPP
- MelonLoader 0.7.x
- Mod Manager - Phone App 2.2.3+ / 2.2.4+, installed as `Mods\ModManager&PhoneApp.dll`

The mod uses standard MelonPreferences, so Mod Manager - Phone App can display and edit its settings in-game. It also listens for Mod Manager save events so changed values can be applied without restarting when possible.

Employee capacity is only raised after the mod has enough idle points for the configured max. Missing idle points are generated at runtime from a cloned vanilla idle point. If a property cannot place enough idle points, that property is skipped and the rest continue.

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
