# -*- coding: utf-8 -*-
import codecs
import ColoredPrint
import Consts
import os
import Tools

coloredPrint = ColoredPrint.ColoredPrint()

def WriteDataManager(controllerFilesPath, targetFilePath):
    print('\n')
    coloredPrint.print_blue_text('***************************************')
    coloredPrint.print_green_text('******* Begin build DataManager *******')
    coloredPrint.print_blue_text('***************************************')
    controllerClasses = []
    controllerObjects = []
    for currentFolder, subfolders, files in os.walk(controllerFilesPath):
        for fileName in files:
            if not fileName.endswith('.cs'):
                continue
            fileName = Tools.GetFileNameByFilePath(fileName)
            controllerClasses.append(fileName)
            objectName = fileName[0].lower() + fileName[1:]
            if objectName.lower().endswith(Consts.CODE_CONTROLLER_NAME_SUFFIX.lower()):
                objectName = objectName[0:-len(Consts.CODE_CONTROLLER_NAME_SUFFIX)]
            controllerObjects.append(objectName)

    # 将两个列表打包成元组列表
    zipped_lists = list(zip(controllerClasses, controllerObjects))
    # 按照字符串首字母排序元组列表
    sorted_zipped_lists = sorted(zipped_lists, key=lambda x: x[0])
    # 解压缩排序后的元组列表
    controllerClasses, controllerObjects = zip(*sorted_zipped_lists)

    fileHandler = codecs.open(targetFilePath, 'w', Consts.CS_CODE_ENCODING)
    fileHandler.write('namespace ' + Consts.NAMESPACE + '\n')
    fileHandler.write('{\n')
    className = Tools.GetFileNameByFilePath(targetFilePath)
    fileHandler.write(Consts.TAB + 'public static class ' + className + '\n')
    fileHandler.write(Consts.TAB + '{\n')
    for index in range(len(controllerClasses)):
        fileHandler.write(Consts.TAB + Consts.TAB + 'public static ' + controllerClasses[index] + ' ' + controllerObjects[index] + ' = new ' + controllerClasses[index] + '();\n')

    fileHandler.write('\n')
    fileHandler.write(Consts.TAB + Consts.TAB + 'public static void Init(string dataPath)\n')
    fileHandler.write(Consts.TAB + Consts.TAB + '{\n')
    for index in range(len(controllerClasses)):
        fileHandler.write(Consts.TAB + Consts.TAB + Consts.TAB + controllerObjects[index] + '.LoadData(dataPath);\n')

    fileHandler.write(Consts.TAB + Consts.TAB + '}\n')
    fileHandler.write(Consts.TAB + '}\n')
    fileHandler.write('}')
    coloredPrint.print_green_text('<<<<<<< Complete build DataManager')
