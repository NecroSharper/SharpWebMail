CSC=mcs
# CSC=gmcs
CSCFLAGS+= /define:MONO /warn:4 /nologo /target:library

# Mono version (used only for subdirs)
MONO_VERION = 1.0
# MONO_VERION = 2.0

# Symbols that identify build target
CSCFLAGS+= /define:MONO_1_0 /define:API_1_1
# CSCFLAGS+= /define:MONO_2_0 /define:API_2_0

# Target subdir
TARGET_BUILD = debug
# TARGET_BUILD = release

# Debug compile options
CSCFLAGS+= /debug+ /debug:full /define:DEBUG

# Base dir of SharpWebMail source
BASE_SWM = .

# Base dir for references (log4net & FCKeditor assemblies should be there)
BASE_REF = $(BASE_SWM)/bin/mono/
BASE_REF_VERSION = $(BASE_SWM)/bin/mono/$(MONO_VERION)

ASP_NET = $(BASE_SWM)/asp.net

RESOURCES = $(BASE_SWM)/resources

BIN_SWM = $(BASE_SWM)/bin/mono/$(MONO_VERION)/$(TARGET_BUILD)

SOURCES_SWM = $(BASE_SWM)/src/*.cs \
              $(BASE_SWM)/src/config/*.cs \
              $(BASE_SWM)/src/net/*.cs \
              $(BASE_SWM)/src/tools/*.cs \
              $(BASE_SWM)/src/ui.pages/*.cs

REFERENCES_SWM= System.Web System.Data System.DirectoryServices $(BASE_REF)/log4net  $(BASE_REF)/FredCK.FCKeditorV2 $(BASE_REF)/SharpMimeTools $(BASE_REF)/OpenSmtp $(BASE_REF)/DotNetOpenMail
REFS_SWM= $(addsuffix .dll, $(addprefix /r:, $(REFERENCES_SWM)))  

all: SharpWebMail

SharpWebMail: swmdir $(BIN_SWM)/SharpWebMail.dll

swmdir:
	if [ ! -d $(BIN_SWM) ]; then \
                mkdir -p $(BIN_SWM); \
        fi; \

$(BIN_SWM)/SharpWebMail.dll: $(SOURCES_SWM)
	$(CSC) $(CSCFLAGS) $(REFS_SWM) /out:$@ $^

asp.net: all copy-asp.net-bin lang

copy-asp.net-bin:
	if [ ! -d $(ASP_NET)/bin/ ]; then \
		mkdir $(ASP_NET)/bin/; \
	fi; \
	cp -pf $(BIN_SWM)/SharpWebMail.dll $(ASP_NET)/bin/SharpWebMail.dll
	cp -pf $(BASE_REF)/*.dll $(ASP_NET)/bin/

clean-asp.net: clean
	rm -R $(ASP_NET)/bin/

lang:
	if [ ! -d $(ASP_NET)/bin/ ]; then \
		mkdir $(ASP_NET)/bin/; \
	fi; \
	for lang in `ls -1 $(RESOURCES)/*.resources|sed -e 's/.*SharpWebMail\.//' -e 's/\..*//'` ; do \
		if [ "$$lang" != "resources" -a "$$lang" != "sr-SP-Latn" -a "$$lang" != "zh-CHT" -a "$$lang" != "my" ]; then \
			if [ ! -d $(ASP_NET)/bin/$$lang/ ]; then \
				mkdir $(ASP_NET)/bin/$$lang/; \
			fi; \
			al /nologo /target:library /embed:$(RESOURCES)/SharpWebMail.$$lang.resources,SharpWebMail.$$lang.resources /culture:$$lang /out:$(ASP_NET)/bin/$$lang/SharpWebMail.resources.dll; \
		fi; \
	done; \

clean:
	rm -f $(BIN_SWM)/SharpWebMail.dll

distclean:
	rm -Rf $(ASP_NET)/bin/
	rm -Rf $(BASE_SWM)/bin/
