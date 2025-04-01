
import ColoredPrint
import Consts
import os
import shutil
import sys
import Tools
import WriteConfig
import WriteDataManager
import WriteDefine

coloredPrint = ColoredPrint.ColoredPrint()

def ResetDirectory():
    DeleteDirectory()
    CreateDirectory()

def DeleteDirectory():
    if os.path.exists(Consts.CSV_ROOT_PATH):
        shutil.rmtree(Consts.CSV_ROOT_PATH, True)
    if os.path.exists(Consts.CODE_ROOT_PATH):
        shutil.rmtree(Consts.CODE_ROOT_PATH, True)


def CreateDirectory():
    if not os.path.exists(Consts.CSV_ROOT_PATH):
        os.makedirs(Consts.CSV_ROOT_PATH)
    if not os.path.exists(Consts.CODE_ROOT_PATH):
        os.makedirs(Consts.CODE_ROOT_PATH)
    if not os.path.exists(Consts.CODE_ROOT_PATH + Consts.CODE_SUBFOLDER_MODEL):
        os.mkdir(Consts.CODE_ROOT_PATH + Consts.CODE_SUBFOLDER_MODEL)
    if not os.path.exists(Consts.CODE_ROOT_PATH + Consts.CODE_SUBFOLDER_CONTROLLER):
        os.mkdir(Consts.CODE_ROOT_PATH + Consts.CODE_SUBFOLDER_CONTROLLER)



def BuildFile(pFilePath, pBuildDataManager):
    if (not Tools.IsExcel(pFilePath)):
        coloredPrint.print_red_text("File is not excel! ----> " + pFilePath)
        return   
    
    tFileName = Tools.GetFileNameByFilePath(pFilePath)
    if(tFileName.lower().endswith(Consts.DEFINE_SUFFIX.lower())):
        WriteDefine.Write(pFilePath, tFileName)    
    else:    
        WriteConfig.Write(pFilePath, tFileName)
    if pBuildDataManager:
        WriteDataManager.WriteDataManager(Consts.CODE_ROOT_PATH + Consts.CODE_SUBFOLDER_CONTROLLER, Consts.CODE_ROOT_PATH + Consts.DATA_MANAGER_NAME + '.cs')


def BuildFolder(pFolderPath):
    for currentFolder, subfolders, files in os.walk(pFolderPath):
        if(currentFolder.startswith(Consts.IGNORE_PREFIX)):
            continue
        for file in files:
            if file.startswith(Consts.IGNORE_PREFIX):
                continue
            if (Tools.IsExcel(file)):
                BuildFile(currentFolder + Consts.FILE_PATH_SEPARATOR + file, False)
    
    WriteDataManager.WriteDataManager(Consts.CODE_ROOT_PATH + Consts.CODE_SUBFOLDER_CONTROLLER, Consts.CODE_ROOT_PATH + Consts.DATA_MANAGER_NAME + '.cs')

def BuildAllExcels():
    ResetDirectory()
    BuildFolder(Consts.EXCLE_ROOT_PATH)


def main():
    Consts.isWriteAllSheet = False
    if len(sys.argv) > 1:
        # 解析第一个参数并设置 Consts.isWriteAllSheet
        try:
            is_all_sheet_value = int(sys.argv[1])
            if is_all_sheet_value == 1:
                Consts.isWriteAllSheet = True
            elif is_all_sheet_value == 0:
                Consts.isWriteAllSheet = False
            else:
                coloredPrint.print_red_text("Invalid value for isWriteAllSheet. Expected 1 or 0.")
                return
        except ValueError:
            coloredPrint.print_red_text("Invalid value for isWriteAllSheet. Expected an integer.")
            return

    if len(sys.argv) == 2:
        BuildAllExcels()
    else:
        tInputParam = sys.argv[2] 
        tFileOrFolderName = os.path.basename(tInputParam)
        if tFileOrFolderName.startswith(Consts.IGNORE_PREFIX):
            coloredPrint.print_red_text('Ignored file or folder! ----> ' + tFileOrFolderName)
            return
        
        CreateDirectory()
        if os.path.isdir(tInputParam):
            BuildFolder(tInputParam)
        elif os.path.isfile(tInputParam):
            BuildFile(tInputParam, True)
        else:
            coloredPrint.print_red_text('Path is not exist! ' + tInputParam)

if __name__ == "__main__": 
    main()
