CSC=mcs
CSCFLAGS+= /define:MONO /warn:4 /nologo /target:library

# Mono version (used only for subdirs)
MONO_VERION = 1.0

# Target subdir
TARGET_BUILD = debug

CSCFLAGS+= /debug+ /debug:full /define:DEBUG

# Base dir of SharpWebMail source
BASE_SWM = .

# Base dir of SharpMimeTools source
BASE_SMT = ../SharpMimeTools

# Base dir for references (log4net & FCKeditor assemblies should be there)
BASE_REF = ../mono/$(MONO_VERION)

ASP_NET = $(BASE_SWM)/asp.net

RESOURCES = $(BASE_SWM)/controls/res

BIN_SWM = $(BASE_SWM)/controls/bin/mono/$(MONO_VERION)/$(TARGET_BUILD)
BIN_SMT = $(BASE_SMT)/bin/mono/$(MONO_VERION)/$(TARGET_BUILD)

SOURCES_SMT = $(BASE_SMT)/src/*.cs

SOURCES_SWM = $(BASE_SWM)/controls/src/UI/*.cs \
		$(BASE_SWM)/controls/src/AssemblyInfo.cs \
		$(BASE_SWM)/controls/src/CTNInbox.cs \
		$(BASE_SWM)/controls/src/CTNSimplePOP3Client.cs \
		$(BASE_SWM)/controls/src/CTNSimpleTCPClient.cs \
		$(BASE_SWM)/controls/src/basicsanitizer.cs \
		$(BASE_SWM)/controls/src/emailserver.cs \
		$(BASE_SWM)/controls/src/serverselector.cs \
		$(BASE_SWM)/controls/src/emailclientfactory.cs \
		$(BASE_SWM)/controls/src/iemailclient.cs \
		$(BASE_SWM)/controls/src/sharpwebmailconfighandler.cs \
		$(BASE_SWM)/controls/src/simpleemailclient.cs \
		$(BASE_SWM)/controls/src/simpleimapclient.cs 


REFERENCES_SMT= System.Web $(BASE_REF)/log4net
REFS_SMT= $(addsuffix .dll, $(addprefix /r:, $(REFERENCES_SMT)))

REFERENCES_SWM= System.Web System.Data System.DirectoryServices $(BASE_REF)/log4net  $(BASE_REF)/FredCK.FCKeditorV2 $(BIN_SMT)/SharpMimeTools
REFS_SWM= $(addsuffix .dll, $(addprefix /r:, $(REFERENCES_SWM)))  

all: SharpMimeTools SharpWebMail

SharpMimeTools: smtdir $(BIN_SMT)/SharpMimeTools.dll

smtdir:
	if [ ! -d $(BIN_SMT) ]; then \
                mkdir -p $(BIN_SMT); \
        fi; \

$(BIN_SMT)/SharpMimeTools.dll: $(SOURCES_SMT)
	$(CSC) $(CSCFLAGS) $(REFS_SMT)  /out:$@ $^

SharpWebMail: SharpMimeTools swmdir $(BIN_SWM)/SharpWebMail.dll

swmdir:
	if [ ! -d $(BIN_SWM) ]; then \
                mkdir -p $(BIN_SWM); \
        fi; \

$(BIN_SWM)/SharpWebMail.dll: $(SOURCES_SWM)
	$(CSC) $(CSCFLAGS) $(REFS_SWM) /resource:$(RESOURCES)/SharpWebMail.resources,SharpWebMail.resources /out:$@ $^

asp.net: all copy-asp.net-bin lang

copy-asp.net-bin:
	if [ ! -d $(ASP_NET)/bin/ ]; then \
		mkdir $(ASP_NET)/bin/; \
	fi; \
	cp -f $(BIN_SWM)/SharpWebMail.dll $(ASP_NET)/bin/SharpWebMail.dll
	cp -f $(BIN_SMT)/SharpMimeTools.dll $(ASP_NET)/bin/SharpMimeTools.dll
	cp -f $(BASE_REF)/*.dll $(ASP_NET)/bin/

clean-asp.net: clean
	rm -R $(ASP_NET)/bin/

lang:
	if [ ! -d $(ASP_NET)/bin/ ]; then \
		mkdir $(ASP_NET)/bin/; \
	fi; \
	for lang in `ls -1 $(RESOURCES)/*.resources|sed -e 's/.*SharpWebMail\.//' -e 's/\..*//'` ; do \
		if [ "$$lang" != "resources" -a "$$lang" != "sr-SP-Latn" ]; then \
			if [ ! -d $(ASP_NET)/bin/$$lang/ ]; then \
				mkdir $(ASP_NET)/bin/$$lang/; \
			fi; \
			al /nologo /target:library /embed:$(RESOURCES)/SharpWebMail.$$lang.resources,SharpWebMail.$$lang.resources /culture:$$lang /out:$(ASP_NET)/bin/$$lang/SharpWebMail.resources.dll; \
		fi; \
	done; \
	if [ ! -d $(ASP_NET)/bin/$$lang/ ]; then \
		mkdir $(ASP_NET)/bin/en/; \
	fi; \
	al /nologo /target:library /embed:$(RESOURCES)/SharpWebMail.resources,SharpWebMail.en.resources /culture:en /out:$(ASP_NET)/bin/en/SharpWebMail.resources.dll; \

clean:
	rm -f $(BIN_SMT)/SharpMimeTools.dll $(BIN_SWM)/SharpWebMail.dll

distclean:
	rm -Rf $(ASP_NET)/bin/
	rm -Rf $(BASE_SMT)/bin/
	rm -Rf $(BASE_SWM)/controls/bin/
