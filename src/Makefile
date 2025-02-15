.SUFFIXES:

DOTNET ?= dotnet

# Dotnet project directories to check for changes of
DNTC_TOOL_DIR := $(abspath ../dntc)
DNTC_TOOL_CSPROJ = $(DNTC_TOOL_DIR)/Dntc.Cli/Dntc.Cli.csproj
SRC_ROOT = $(abspath .)

CORE_DIR = $(SRC_ROOT)/DotnetSnes.Core
ARTIFACTS_ROOT = $(SRC_ROOT)/artifacts/bin/
DOTNETSNES_CORE_DLL = $(ARTIFACTS_ROOT)/DotnetSnes.Core/release/DotnetSnes.Core.dll

DOTNETSNES_CORE_CSPROJ = $(SRC_ROOT)/DotnetSnes.Core/DotnetSnes.Core.csproj
CURRENT_SAMPLE ?= 

SAMPLE_NS = DotnetSnes.Example.$(CURRENT_SAMPLE)
SAMPLE_ARTIFACTS_DIR = $(ARTIFACTS_ROOT)/$(SAMPLE_NS)/release
SAMPLE_DLL = $(SAMPLE_ARTIFACTS_DIR)/$(SAMPLE_NS).dll
SAMPLE_DIR = $(SRC_ROOT)/$(SAMPLE_NS)
SAMPLE_MANIFEST = $(SAMPLE_ARTIFACTS_DIR)/manifest.json
SAMPLE_C_FILE = $(SAMPLE_ARTIFACTS_DIR)/$(CURRENT_SAMPLE).c
SAMPLE_ROM_NAME = $(SAMPLE_ARTIFACTS_DIR)/$(CURRENT_SAMPLE).sfc
SAMPLE_BITMAP = $(SAMPLE_ARTIFACTS_DIR)/pvsneslibfont.pic
SAMPLE_PNG = $(SAMPLE_ARTIFACTS_DIR)/pvsneslibfont.png

# PVSNESLIB
PVSNESLIB_HOME ?= $(abspath ../pvsneslib)
include ${PVSNESLIB_HOME}/devkitsnes/snes_rules
PVSNESLIB_DEVKIT_TOOLS = $(PVSNESLIB_HOME)/devkitsnes/tools
GFXCONV ?= $(PVSNESLIB_DEVKIT_TOOLS)/gfx4nes
export ROMNAME := $(CURRENT_SAMPLE)

# Available samples that can be built
.phony: hello_world
hello_world:
	@$(MAKE) CURRENT_SAMPLE=HelloWorld build_sample

# Core building targets
.phony: build_sample
build_sample: $(PVSNESLIB_GFX4SNES_TOOL) $(SAMPLE_C_FILE) $(SAMPLE_BITMAP) $(ROMNAME).sfc
	$(if $(CURRENT_SAMPLE),,$(error "No sample name set. Invoke make with a valid sample name"))
	@echo Building $(CURRENT_SAMPLE)

ifdef CURRENT_SAMPLE # Targets that can't be included when we don't have a sample named yet

$(SAMPLE_C_FILE): $(SAMPLE_DLL)
	$(DOTNET) run --project $(DNTC_TOOL_CSPROJ) -- $(SAMPLE_MANIFEST)

$(SAMPLE_DLL): $(DOTNETSNES_CORE_DLL) $(shell find $(SAMPLE_DIR) $(CORE_DIR))
	$(DOTNET) build -c Release $(SAMPLE_DIR)/$(SAMPLE_NS).csproj

$(SAMPLE_BITMAP): $(SAMPLE_PNG) $(GFXCONV)
	@echo convert font with no tile reduction ... $(notdir $@)
	cd $(SAMPLE_ARTIFACTS_DIR) && $(GFXCONV) -s 8 -o 16 -u 16 -p -e 0 -i $(notdir $<)

endif

$(DOTNETSNES_CORE_DLL): $(shell find $(DNTC_TOOL_DIR))
	$(DOTNET) build -c Release $(DOTNETSNES_CORE_CSPROJ)

$(GFXCONV):
	$(MAKE) -C $(PVSNESLIB_HOME) PVSNESLIB_HOME=$(PVSNESLIB_HOME)

.phony: clean
clean:
	rm -rf $(ARTIFACTS_ROOT)
	$(DOTNET) clean -c Release $(DNTC_TOOL_CSPROJ)
	$(DOTNET) clean -c Debug $(DNTC_TOOL_CSPROJ)
	$(MAKE) -C $(PVSNESLIB_HOME) PVSNESLIB_HOME=$(PVSNESLIB_HOME) clean


