.SUFFIXES:

SNES_NAME ?= MoveObjects
SNES_DLL ?= $(ARTIFACTS_ROOT)/DotnetSnes.Example.Objects.MoveObjects.dll

export REPO_DEFAULT_RULES ?= $(abspath ../Makefile.defaults)

include $(REPO_DEFAULT_RULES)
include $(DNSNES_BASE_MAKEFILE)

# PVSNESLIB
include ${PVSNESLIB_HOME}/devkitsnes/snes_rules
PVSNESLIB_DEVKIT_TOOLS = $(PVSNESLIB_HOME)/devkitsnes/tools

game_assets: sprites.pic

sprites.pic: sprites.bmp
	@echo convert bitmap ... $(notdir $@)
	$(GFXCONV) -s 32 -o 16 -u 16 -p -t bmp -i $<
