.SUFFIXES:

SNES_NAME ?= HelloWorld
SNES_DLL ?= $(ARTIFACTS_ROOT)/DotnetSnes.Example.HelloWorld.dll

export REPO_DEFAULT_RULES ?= $(abspath ../Makefile.defaults)

include $(REPO_DEFAULT_RULES)
include $(DNSNES_BASE_MAKEFILE)

# PVSNESLIB
include ${PVSNESLIB_HOME}/devkitsnes/snes_rules
PVSNESLIB_DEVKIT_TOOLS = $(PVSNESLIB_HOME)/devkitsnes/tools

game_assets: pvsneslibfont.pic

pvsneslibfont.pic: pvsneslibfont.png
	@echo convert font with no tile reduction ... $(notdir $@)
	cd $(ARTIFACTS_ROOT) && \
		$(GFXCONV) -s 8 -o 16 -u 16 -p -e 0 -i $(notdir $<)


