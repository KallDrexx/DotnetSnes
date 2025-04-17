.SUFFIXES:

SNES_NAME ?= Breakout
SNES_DLL ?= $(ARTIFACTS_ROOT)/DotnetSnes.Example.Breakout.dll

export REPO_DEFAULT_RULES ?= $(abspath ../Makefile.defaults)

include $(REPO_DEFAULT_RULES)
include $(DNSNES_BASE_MAKEFILE)

# PVSNESLIB
include ${PVSNESLIB_HOME}/devkitsnes/snes_rules
PVSNESLIB_DEVKIT_TOOLS = $(PVSNESLIB_HOME)/devkitsnes/tools

game_assets:


