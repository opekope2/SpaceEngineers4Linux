# SpaceEngineers4Linux

## What is this?

Experiments to run Space Engineers on .NET 6 on native Linux.

You are probably looking for the Wine/Windows branch, which enables Space Engineers via the Loader to be run on Wine and/or Windows. This is yet to be tested.

## How to set up

1. I recommend using [JetBrains Rider](https://www.jetbrains.com/rider/). Vscode works, too, but you don't have a decompiler and IL viewer. VS not tested.
2. Install .NET 6 (not .NET Core, not .NET 5, and not .NET 7)
3. Install the latest Mono
4. Install zenity
5. Open `RuntimePatches/RuntimePatches.csproj` and edit the path to the game assemblies (`/path/to/...`)
6. Copy `compatibilitytool.vdf` to steam compatibility tools folder (you can rename it) (`~/.steam/steam/compatibilitytools.d/` or something, you can search for that if doesn't work)
7. Open that file, and edit `install_path` to the root of this repo.
8. Download [Steam SDK](https://partner.steamgames.com/downloads/steamworks_sdk.zip) and extract `sdk/redistributable_bin/libsteam_api.so` into `bin/Debug/Native`
9. Download [DXVK Native](https://github.com/Joshua-Ashton/dxvk-native/releases) and extract all `.so` files from it into `bin/Debug/Native` (or `bin/Release/Native`. The native folder must be in the parent folder of the Loader binary, see `Loader/Program.cs`. Same applies for `libsteam_api.so`)
10. Open Steam > Library > Space Engineers > Properties > Compatibility and set it to `.NET 6` (or whatever it is in `compatibilitytool.vdf`)
11. Open project in your favorite IDE/text editor
12. Start Space Engineers
13. In your IDE/text editor, attach to the displayed process ID
14. Click OK. Now the game lauches.

## What is the furthest point the game runs?

I saw the splash screen at some point, but not anymore. Probably after skipping DXVK Native initialization (the game showed an error soon after).

```
   at SharpDX.DXGI.Adapter.GetDescription(AdapterDescription& descRef)
   at SharpDX.DXGI.Adapter.get_Description()
   at VRage.Platform.Windows.Render.MyPlatformRender.GetReadableAdapterDesc(Adapter adapter)
   at VRage.Platform.Windows.Render.MyPlatformRender.CreateAdaptersList()
   at VRage.Platform.Windows.Render.MyPlatformRender.GetAdaptersList()
   at VRage.Platform.Windows.Render.MyWindowsRender.GetRenderAdapterList()
   at VRageRender.MyRender11.GetAdaptersList()
   at VRageRender.MyDX11Render.get_IsSupported()
   at SpaceEngineers.MyProgram.InitializeRender()
   at SpaceEngineers.MyProgram.Main(String[] args)
   at Program.<Main>$(String[] args)
```

`GetDescription` cannot be decompiled, all decompilers I tried spit out weird stuff, so let's read MSIL.

```
    IL_000f: ldarg.0      // this
    IL_0010: ldfld        void* [SharpDX]SharpDX.CppObject::_nativePointer
    IL_0015: ldloca.s     ref
    IL_0017: conv.u
    IL_0018: ldarg.0      // this
    IL_0019: ldfld        void* [SharpDX]SharpDX.CppObject::_nativePointer
    IL_001e: ldind.i
    IL_001f: ldc.i4.8
    IL_0020: conv.i
    IL_0021: sizeof       void*
    IL_0027: mul
    IL_0028: add
    IL_0029: ldind.i
    IL_002a: calli        int32 (void*, void*)
    IL_002f: call         valuetype [SharpDX]SharpDX.Result [SharpDX]SharpDX.Result::op_Implicit(int32)
    IL_0034: stloc.1      // result
```

When I F11 into this, the code goes to `MonoMod.RuntimeDetour.Platforms.DetourRuntimeNET60Platform.CompileMethodHook` method.

After this, the code goes to `SharpDX.DXGI.AdapterDescription.__MarshalFrom`, and the stack is corrupted:

```
   at SharpDX.DXGI.AdapterDescription.__MarshalFrom(__Native& ref)
   at SharpDX.DXGI.Adapter.GetDescription(AdapterDescription& descRef)
```

This is the entire stack trace. `this` seems to be garbage (Could not dereference a value. A reference value was found to be bad during dereferencing. (0x80131305). The error code is CORDBG_E_BAD_REFERENCE_VALUE, or0x80131305) and shortly after, the game crashes.