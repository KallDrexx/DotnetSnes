.SUFFIXES:

DOTNET ?= dotnet
SAMPLE_NAME ?= HelloWorld
ARTIFACTS_ROOT ?= $(abspath bin/Release/net8.0)
SAMPLE_DLL ?= $(ARTIFACTS_ROOT)/DotnetSnes.Example.HelloWorld.dll
DNSNES_CORE_DIR ?= $(abspath ../DotnetSnes.Core)
DNTC_TOOL_CSPROJ ?= $(abspath ../../dntc/Dntc.Cli/Dntc.Cli.csproj)
PVSNESLIB_HOME ?= $(abspath ../../pvsneslib)

#------------------Shared Sections----------------------------
GENERATED_C_FILE ?= $(SAMPLE_NAME).c
MANIFEST_NAME ?= manifest.json
MANIFEST_PATH = $(ARTIFACTS_ROOT)/$(MANIFEST_NAME)
DNTC_CLI_DIR = $(dir $(DNTC_TOOL_CSPROJ))
PVSNESLIB_TOOL_FILE = $(PVSNESLIB_HOME)/tools/gfx4snes/gfx4snes

# These variables need to be exported for the sfc makefile execution
export DNSNES_CORE_DIR
export DNTC_TOOL_CSPROJ
export DNTC_CLI_DIR
export PVSNESLIB_HOME
export ARTIFACTS_ROOT

# Best test I could think of if pvsneslib needs to be built or not
$(PVSNESLIB_TOOL_FILE):
	$(MAKE) -C $(PVSNESLIB_HOME) PVSNESLIB_HOME=$(PVSNESLIB_HOME)

.phony: bitmaps all
all: $(PVSNESLIB_TOOL_FILE) $(GENERATED_C_FILE) $(ARTIFACTS_ROOT)/$(SAMPLE_NAME).sfc

$(GENERATED_C_FILE): $(SAMPLE_DLL)
	$(DOTNET) run --project $(DNTC_TOOL_CSPROJ) -- $(MANIFEST_PATH)

$(SAMPLE_DLL): $(shell find . $(DNSNES_CORE_DIR) $(DNTC_CLI_DIR) -type f -name "*.cs")
	$(DOTNET) build -c Release

# The actual rom needs to be built from inside the artifacts directory
$(ARTIFACTS_ROOT)/$(SAMPLE_NAME).sfc: bitmaps
	$(MAKE) -f $(abspath .)/Makefile -C $(ARTIFACTS_ROOT) $(SAMPLE_NAME).sfc ROMNAME=$(SAMPLE_NAME)

clean:
	rm -rf $(ARTIFACTS_ROOT)

#-----------------/Shared Sections----------------------------

# PVSNESLIB
include ${PVSNESLIB_HOME}/devkitsnes/snes_rules
PVSNESLIB_DEVKIT_TOOLS = $(PVSNESLIB_HOME)/devkitsnes/tools

bitmaps: $(ARTIFACTS_ROOT)/pvsneslibfont.pic
clean: cleanBuildRes cleanRom cleanGfx

$(ARTIFACTS_ROOT)/pvsneslibfont.pic: $(ARTIFACTS_ROOT)/pvsneslibfont.png
	@echo convert font with no tile reduction ... $(notdir $@)
	cd $(ARTIFACTS_ROOT) && \
		$(GFXCONV) -s 8 -o 16 -u 16 -p -e 0 -i $(notdir $<)


