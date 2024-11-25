PROJECT_NAME = StarByte
SRC_DIR = src
OBJ_DIR = obj
CSC = mcs
OUT = out

SRC_FILES = $(wildcard $(SRC_DIR)/*.cs)

OUTPUT_EXE = $(OUT)/$(PROJECT_NAME).exe
OUTPUT_LINUX = $(OUT)/$(PROJECT_NAME)

all: $(OUTPUT_LINUX) #$(OUTPUT_EXE)

# $(OUTPUT_EXE): $(SRC_FILES)
# 	@echo "Kompiliere $(PROJECT_NAME) für Windows..."
# 	@mkdir -p $(OUT)
# 	$(CSC) -out:$(OUTPUT_EXE) $(SRC_FILES)
# 	@echo "Windows-kompatible .exe-Datei erstellt: $(OUTPUT_EXE)"

$(OUTPUT_LINUX): $(SRC_FILES)
	@echo "Kompiliere $(PROJECT_NAME) für Linux..."
	@mkdir -p $(OUT)
	$(CSC) -out:$(OUTPUT_LINUX) $(SRC_FILES)
	@chmod +x $(OUTPUT_LINUX)  # Macht die Linux-Datei ausführbar
	@echo "Linux-kompatible Datei erstellt: $(OUTPUT_LINUX)"