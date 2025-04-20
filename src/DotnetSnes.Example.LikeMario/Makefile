.SUFFIXES:

SNES_NAME ?= LikeMario
SNES_DLL ?= $(ARTIFACTS_ROOT)/DotnetSnes.Example.LikeMario.dll

export REPO_DEFAULT_RULES ?= $(abspath ../Makefile.defaults)

include $(REPO_DEFAULT_RULES)
include $(DNSNES_BASE_MAKEFILE)

# BEFORE including snes_rules :
# list in AUDIOFILES all your .it files in the right order. It will build to generate soundbank file
AUDIOFILES := overworld.it
# then define the path to generate soundbank data. The name can be different but do not forget to update your include in .c file !
export SOUNDBANK := soundbank

# PVSNESLIB
include ${PVSNESLIB_HOME}/devkitsnes/snes_rules
PVSNESLIB_DEVKIT_TOOLS = $(PVSNESLIB_HOME)/devkitsnes/tools

# To build musics, define SMCONVFLAGS with parameters you want
SMCONVFLAGS := -s -o $(SOUNDBANK) -V -b 5
musics: $(SOUNDBANK).obj

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

