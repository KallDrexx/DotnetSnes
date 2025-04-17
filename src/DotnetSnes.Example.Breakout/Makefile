.SUFFIXES:

SNES_NAME ?= Breakout
SNES_DLL ?= $(ARTIFACTS_ROOT)/DotnetSnes.Example.Breakout.dll

export REPO_DEFAULT_RULES ?= $(abspath ../Makefile.defaults)

include $(REPO_DEFAULT_RULES)
include $(DNSNES_BASE_MAKEFILE)

# BEFORE including snes_rules :
# list in AUDIOFILES all your .it files in the right order. It will build to generate soundbank file
AUDIOFILES :=
# then define the path to generate soundbank data. The name can be different but do not forget to update your include in .c file !
export SOUNDBANK := soundbank

# PVSNESLIB
include ${PVSNESLIB_HOME}/devkitsnes/snes_rules
PVSNESLIB_DEVKIT_TOOLS = $(PVSNESLIB_HOME)/devkitsnes/tools

# To build musics, define SMCONVFLAGS with parameters you want
SMCONVFLAGS := -s -o $(SOUNDBANK) -V -b 5
musics: $(SOUNDBANK).obj

game_assets: musics


