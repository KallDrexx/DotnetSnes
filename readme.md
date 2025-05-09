# DotnetSnes 

Allows using .net languages to create real working SNES roms!

<!-- TOC -->
* [DotnetSnes](#dotnetsnes-)
  * [How Does It Work?](#how-does-it-work)
  * [Getting Started](#getting-started)
    * [Dependencies needed to install on WSL Ubuntu for make](#dependencies-needed-to-install-on-wsl-ubuntu-for-make)
    * [Preparing the Repo and SDKs](#preparing-the-repo-and-sdks)
    * [Hello World Example](#hello-world-example)
    * [LikeMario Game Example](#likemario-game-example)
    * [Breakout Game Example](#breakout-game-example)
    * [Creating a New Project](#creating-a-new-project)
<!-- TOC -->

## How Does It Work?

This works by providing a .net library that abstracts functions and globals used in creating SNES games. Once a
project using the `DotnetSnes.Core` library is compiled, the resulting DLL is transpiled to C using the 
[Dotnet To C transpiler (dntc)](https://github.com/KallDrexx/dntc). The game's C code is then compiled against 
the [PvSnesLib SDK](https://github.com/alekmaul/pvsneslib) toolchain to create a working rom.

Due to the limitations of the SNES, it is not always possible to write idiomatic C# for a working rom. For example:
* Minimal `System` level type support
* No dynamic allocations are supported (thus no reference type support)
* Minimizing variables on the stack is important 
* Pointer/address tracking is common to save memory

Even with these limitations, it is still possible to create real SNES games, though there might be instances where
it may be necessary to go to a lower level than C# supports.

*Note:* The dntc transpiler has its own limitations on what MSIL operations it supports and doesn't yet support, so
it's possible to hit code that should work but the transpiler does not yet have support for.

## Getting Started

There are several things to be aware of before getting started

* The PVSnesLib SDK works best under Linux. If developing on windows, you'll want to do the final compilation of the
   SNES rom under WSL, but obviously you can use windows emulators to test the resulting rom.
* This repository should be cloned using `ssh` credentials. This is because all submodules (and submodules within
   submodules) are using `git@` addresses and I have not yet figured out a good way to allow them to be mixed. See the
   [Github SSH key docs](https://docs.github.com/en/authentication/connecting-to-github-with-ssh/generating-a-new-ssh-key-and-adding-it-to-the-ssh-agent)
   for help.
* Make sure to do a full recursive submodule update after cloning (e.g. `git submodule update --init --recursive`)

### Dependencies needed to install on WSL Ubuntu for make

* `export PVSNESLIB_HOME=(yourpath)/DotnetSnes/pvsneslib`
* `sudo apt-get update` (if needed)
* `sudo apt-get install cmake`
* `sudo apt-get install g++`
* `sudo apt-get install dotnet-sdk-8.0`

### Preparing the Repo and SDKs

1. Clone the DotnetSnes repository
2. Do a recursive submodule initialization on the clone to get dependent projects
   * `git submodule update --init --recursive`
3. `cd` into the `pvsneslib/` directory and run `make`
   * This will make sure the pvsneslib toolchain is now built
   
### Hello World Example

The Hello World is a basic example that shows a bare minimum SNES rom that prints text to the screen.

1. `cd` into [src/DotnetSnes.Example.HellowWorld/](src/DotnetSnes.Example.HelloWorld)
2. run `make`

You should see `Build finished successfully !` and a `HelloWorld.sfc` should now exist in your `bin/Release/net8.0`
folder. This is your SNES rom that you can load into an emulator, or save to flash cart.

### LikeMario Game Example

https://github.com/user-attachments/assets/b39b9c10-7562-445d-afaf-fa0f3fd6ce0c

This is a C# port of the 
[PVSnesLib LikeMario example](https://github.com/alekmaul/pvsneslib/tree/master/snes-examples/games/likemario), which
demonstrates how to manage objects, tmx tile maps, audio, and game pad input.

Important classes are:
* [Globals.cs](src/DotnetSnes.Example.LikeMario/Globals.cs) contains assembly labels for where content exists in the
   baked in rom.
* [Game.cs](src/DotnetSnes.Example.LikeMario/Game.cs) contains the logic for initializing the game
* [Mario.cs](src/DotnetSnes.Example.LikeMario/Mario.cs) contains the logic for showing and controlling the mario object

To build:
1. `cd` into [src/DotnetSnes.Example.LikeMario]
2. run 'make'

The `LikeMario.sfc` rom file should now exist in the `bin/Release/net8.0` folder.

### Breakout Game Example

https://github.com/user-attachments/assets/0c4ac3e7-3423-48b8-acda-2e8dee935c7a


This is a C# port of the
[PVSnesLib Brekaout example](https://github.com/alekmaul/pvsneslib/tree/master/snes-examples/games/breakout),
which demonstrates a complete game with more idiomatic C#.

Most game logic is in the [Game class](src/DotnetSnes.Example.Breakout/Game.cs).

To build:
1. `cd` into `src/DotnetSnes.Example.Breakout`
2. run `make`

The `Breakout.sfc` rom file should now exist in the `bin/Release/net8.0` folder.

### Creating a New Project

To start a brand-new project:

1. Create a new .net class library project
2. Reference the `DotnetSnes.Core` library
   * This provides core functionality needed by SNES roms
3. Reference the `Dntc.Attributes` library
   * This provides pretty important attributes thatare useful for the transpilation process
4. Create a `public static int Main()` function declaration to declare your rom's entry point
5. Add the `[CustomFunctionName("main")]` attribute to your `Main()` function, to force it to have `main` as the
   function name after transpiling to C. This allows the SDK to know it's the entry point.
6. Create a file named `Makefile` with the following contents:
```makefile
.SUFFIXES:

SNES_NAME ?= HelloWorld # Replace with the name you want your rom to have
SNES_DLL ?= $(ARTIFACTS_ROOT)/DotnetSnes.Example.HelloWorld.dll # Replace with the DLL your csproj will generate
ARTIFACTS_ROOT ?= $(abspath bin/Release/net8.0)
DNSNES_CORE_DIR ?= $(abspath ../DotnetSnes.Core) # Replace with the relative path to the DotnetSnes.Core project
DNTC_TOOL_CSPROJ ?= $(abspath ../../dntc/Dntc.Cli/Dntc.Cli.csproj) #
PVSNESLIB_HOME ?= $(abspath ../../pvsneslib) # Replace with the relative path to the pvsneslib folder from the repo
DNSNES_BASE_MAKEFILE ?= $(abspath ../Makefile.base) # Replace with the relative path to the DotnetSnes Makefile.base file

include $(DNSNES_BASE_MAKEFILE)

# PVSNESLIB
include ${PVSNESLIB_HOME}/devkitsnes/snes_rules
PVSNESLIB_DEVKIT_TOOLS = $(PVSNESLIB_HOME)/devkitsnes/tools
```
7. If any content/game assets are needed to be added to the ROM, include them in the directory and create `game_assets`
   rules for them at the bottom of the `Makefile`.  For example:
```makefile
game_assets: mario_sprite.pic tiles.pic map_1_1.m16 mariofont.pic musics mariojump.brr

mario_sprite.pic: mario_sprite.bmp
	@echo convert sprites ... $(notdir $@)
	$(GFXCONV) -s 16 -o 16 -u 16 -p -t bmp -i $<

tiles.pic: tiles.png
	@echo convert map tileset... $(notdir $@)
	$(GFXCONV) -s 8 -o 16 -u 16 -p -m -i $<

map_1_1.m16: map_1_1.tmj tiles.pic
	@echo convert map tiled ... $(notdir $@)
	$(TMXCONV) $< tiles.map

mariofont.pic: mariofont.bmp
	@echo convert font with no tile reduction ... $(notdir $@)
	$(GFXCONV) -s 8 -o 2 -u 16 -e 1 -p -t bmp -m -R -i $<
```
8. Create a `manifest.json` file in the same directory as your `csproj`. This instructs the transpiler on how to convert
   your project from a .net DLL to C.
```json
{
  "AssemblyDirectory": ".",
  "AssembliesToLoad": [
    "DotnetSnes.Core.dll",
    "DotnetSnes.Example.LikeMario.dll"
  ],
  "PluginAssembly": "DotnetSnes.Core.dll",
  "OutputDirectory": ".",
  "SingleGeneratedSourceFileName": "LikeMario.c",
  "MethodsToTranspile": [
    "System.Int32 DotnetSnes.Example.LikeMario.Game::Main()"
  ],
  "GlobalsToTranspile": []
}
```
9. Make the following updates to the `manifest.json` file:
   1. Change the second `AssembliesToLoad` value from `DotnetSnes.Example.LikeMario.dll` to the dll being generated
      by your project.
   2. Change `SingleGeneratedSourceFileName` from `LikeMario.c` to be `<RomName>.c`, where `<RomName>` is the same
      name specified in the `SNES_NAME` value in the `Makefile`
   3. Set `MethodsToTranspile` to point towards your `Main` function id. In most cases, this is just a matter of
      changing `DotnetSnes.Example.LikeMario.Game` to the full namespace + static class name that contains your
      `Main` function.
10. Make sure your .net project copies the `manifest.json` file and any other content files needed to the output 
   directory on build.
11. Add code to your `Main()` function
12. `cd` into your project directory and type `make`.

That should compile your project, and if successful will build the SNES rom in the directory specified by the 
`ARTIFACTS_ROOT` in your `Makefile`.

If your code references game content specified in assembly files, those locations in the assembly files are referenced
by labels. In C code these are referenced by defining an `extern char` for the label name. In .net projects, you can
reference these by annotating a field with a `[AssemblyLabel("labelName")]` attribute.
