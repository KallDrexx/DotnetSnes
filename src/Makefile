.SUFFIXES:

DOTNET ?= dotnet

# Dotnet project directories to check for changes of
DNTC_TOOL_DIR := $(abspath ../dntc)
SRC_ROOT = $(abspath .)

DNTC_TOOL_CSPROJ = $(DNTC_TOOL_DIR)/Dntc.Cli/Dntc.Cli.csproj

CORE_DIR = $(SRC_ROOT)/DotnetSnes.Core
ARTIFACTS_ROOT = $(SRC_ROOT)/artifacts/bin/
DOTNETSNES_CORE_DLL = $(ARTIFACTS_ROOT)/DotnetSnes.Core/release/DotnetSnes.Core.dll

DOTNETSNES_CORE_CSPROJ = $(SRC_ROOT)/DotnetSnes.Core/DotnetSnes.Core.csproj
CURRENT_SAMPLE ?= 

SAMPLE_NS = DotnetSnes.Example.$(CURRENT_SAMPLE)
SAMPLE_DLL = $(ARTIFACTS_ROOT)/$(SAMPLE_NS)/release/$(SAMPLE_NS).dll
SAMPLE_DIR = $(SRC_ROOT)/$(SAMPLE_NS)

# Available samples that can be built
.phony: hello_world
hello_world:
	@$(MAKE) CURRENT_SAMPLE=HelloWorld build_sample

# Core building targets
.phony: build_sample
build_sample: $(SAMPLE_DLL) 
	$(if $(CURRENT_SAMPLE),,$(error "No sample name set. Invoke make with a valid sample name"))
	@echo Building $(CURRENT_SAMPLE)

ifdef CURRENT_SAMPLE
$(SAMPLE_DLL): $(DOTNETSNES_CORE_DLL) $(shell find $(SAMPLE_DIR))
	$(DOTNET) build -c Release $(SAMPLE_DIR)/$(SAMPLE_NS).csproj
endif

$(DOTNETSNES_CORE_DLL): $(shell find $(DNTC_TOOL_DIR))
	$(DOTNET) build -c Release $(DOTNETSNES_CORE_CSPROJ)

.phony: clean
clean:
	$(DOTNET) clean -c Release $(SRC_ROOT)/*.sln
	$(DOTNET) clean -c Release $(DNTC_TOOL_CSPROJ)


