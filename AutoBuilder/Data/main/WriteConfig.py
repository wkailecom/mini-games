# -*- coding: utf-8 -*-
import codecs
import ColoredPrint
import Consts
import Tools

coloredPrint = ColoredPrint.ColoredPrint()

def GetHeaderDatas(excelData, pConvertConstLengthArrayTypeToOriginType):
    headerDatas = [0] * Consts.HEADERS_COUNT
    headerDatas[Consts.ANNOTATION] = excelData.row_values(Consts.ANNOTATION)
    headerDatas[Consts.CLIENT_NAME] = excelData.row_values(Consts.CLIENT_NAME)
    headerDatas[Consts.DATA_TYPE] = Tools.ParseDataTypes(excelData.row_values(Consts.DATA_TYPE), pConvertConstLengthArrayTypeToOriginType)
    headerDatas[Consts.INDEX_IDENTIFY] = excelData.row_values(Consts.INDEX_IDENTIFY)
    headerDatas[Consts.CONST_IDENTIFY] = excelData.row_values(Consts.CONST_IDENTIFY)
    return headerDatas


def WriteCSVFile(excelData, csvFileName):
    csvFileHandle = codecs.open(Consts.CSV_ROOT_PATH + csvFileName + '.csv', 'w', Consts.CSV_ENCODING)
    headerDatas = GetHeaderDatas(excelData, False)
    names = headerDatas[Consts.CLIENT_NAME]
    dataTypes = headerDatas[Consts.DATA_TYPE]
    for rowIndex in range(excelData.nrows):
        if rowIndex < Consts.HEADERS_COUNT:
            continue
        rowData = excelData.row_values(rowIndex)
        for colIndex in range(len(names)):
            if names[colIndex] == '':
                continue
            dataType = dataTypes[colIndex]
            if dataType == '':
                raise Exception
            if colIndex > 0:
                csvFileHandle.write(Consts.TEXT_DATA_SEPARATOR)
            csvCellData = Tools.GetCsvCellDataByExcelCellData(rowData[colIndex], dataType)
            csvFileHandle.write(csvCellData)

        if rowIndex != excelData.nrows - 1:
            csvFileHandle.write('\n')

    csvFileHandle.close()

def WriteCSVFileAllSheet(excelAllData, csvFileName):
    csvFileHandle = codecs.open(Consts.CSV_ROOT_PATH + csvFileName + '.csv', 'w', Consts.CSV_ENCODING)
    
    for sheetIndex, sheetData in enumerate(excelAllData):
        # 获取表头数据（假设第一个Sheet包含表头）
        if sheetIndex == 0:
           headerDatas = GetHeaderDatas(sheetData, False)
           names = headerDatas[Consts.CLIENT_NAME]
           dataTypes = headerDatas[Consts.DATA_TYPE]

        for rowIndex in range(sheetData.nrows):
            if rowIndex < Consts.HEADERS_COUNT:
                continue
            rowData = sheetData.row_values(rowIndex)
            for colIndex in range(len(names)):
                if names[colIndex] == '':
                    continue
                dataType = dataTypes[colIndex]
                if dataType == '':
                    raise Exception
                if colIndex > 0:
                    csvFileHandle.write(Consts.TEXT_DATA_SEPARATOR)
                csvCellData = Tools.GetCsvCellDataByExcelCellData(rowData[colIndex], dataType)
                csvFileHandle.write(csvCellData)

            # 添加换行符（如果不是当前Sheet的最后一行）
            if rowIndex != sheetData.nrows - 1 or sheetIndex != len(excelAllData) - 1:
                csvFileHandle.write('\n')

    csvFileHandle.close()

def WriteConstCodes(csvFileName, codeName, valueType, columnType, columnIndex):

    if valueType != Consts.DATA_TYPE_INT:
        if valueType != Consts.DATA_TYPE_STRING:
            coloredPrint.print_red_text('valueType must be string or int')
            raise Exception
        if columnType != Consts.DATA_TYPE_STRING:
            coloredPrint.print_red_text('Const names type must be string')
            raise Exception
        
    csvFileHandle = codecs.open(Consts.CSV_ROOT_PATH + csvFileName + '.csv', 'r', Consts.CSV_ENCODING)
    csvData = csvFileHandle.readlines()
    csvFileHandle.close()
    codeName = Tools.UpperFirstLetter(codeName)
    fileHandlerConst = codecs.open(Consts.CODE_ROOT_PATH + Consts.CODE_SUBFOLDER_MODEL + codeName + '.cs', 'w', Consts.CS_CODE_ENCODING)
    fileHandlerConst.write('namespace ' + Consts.NAMESPACE + '\n')
    fileHandlerConst.write('{\n')
    if valueType == Consts.DATA_TYPE_INT:
        fileHandlerConst.write(Consts.TAB + 'public enum ' + codeName + '\n')
    else:
        fileHandlerConst.write(Consts.TAB + 'public class ' + codeName + '\n')
    fileHandlerConst.write(Consts.TAB + '{\n')
    if valueType == Consts.DATA_TYPE_INT:
        fileHandlerConst.write(Consts.TAB + Consts.TAB + 'Invalid = -1,\n')
    for rowData in csvData:
        parsedRowData = rowData.replace('\r', '').replace('\n', '').split(Consts.TEXT_DATA_SEPARATOR)
        constName = parsedRowData[columnIndex]
        constName = Tools.GetFileNameByFilePath(constName)
        constName = Tools.UpperFirstLetter(constName)
        constValue = parsedRowData[0]
        if valueType == Consts.DATA_TYPE_INT:
            fileHandlerConst.write(Consts.TAB + Consts.TAB + constName + ' = ' + constValue + ',\n')
        else:
            fileHandlerConst.write(Consts.TAB + Consts.TAB + 'public const ' + valueType + ' ' + constName + ' = "' + constValue + '";\n')

    fileHandlerConst.write(Consts.TAB + '}\n')
    fileHandlerConst.write('}')
    fileHandlerConst.close()
    

def WriteCodes(excelData, fileName):
    headerDatas = GetHeaderDatas(excelData, True)
    annotations = headerDatas[Consts.ANNOTATION]
    names = headerDatas[Consts.CLIENT_NAME]
    types = headerDatas[Consts.DATA_TYPE]
    indexes = headerDatas[Consts.INDEX_IDENTIFY]
    consts = headerDatas[Consts.CONST_IDENTIFY]
    for index in range(len(names) - 1, -1, -1):
        if names[index] == '':
            del annotations[index]
            del names[index]
            del types[index]
            del indexes[index]
            del consts[index]

    fileHandlerModel = codecs.open(Consts.CODE_ROOT_PATH + Consts.CODE_SUBFOLDER_MODEL + fileName + Consts.CODE_MODEL_NAME_SUFFIX + '.cs', 'w', Consts.CS_CODE_ENCODING)
    fileHandlerModel.write('namespace ' + Consts.NAMESPACE + '\n')
    fileHandlerModel.write('{\n')
    fileHandlerModel.write(Consts.TAB + 'public class ' + fileName + ' : ConfigModelBase\n')
    fileHandlerModel.write(Consts.TAB + '{\n')
    for index in range(len(names)):
        fileHandlerModel.write(Consts.TAB + Consts.TAB + '/// <summary>\n')
        fileHandlerModel.write(Consts.TAB + Consts.TAB + '///' + annotations[index] + '\n')
        fileHandlerModel.write(Consts.TAB + Consts.TAB + '/// </summary>\n')
        fileHandlerModel.write(Consts.TAB + Consts.TAB + 'public ' + types[index] + ' ' + names[index] + ' { get; private set; }\n')

    fileHandlerModel.write('\n')
    fileHandlerModel.write(Consts.TAB + Consts.TAB + 'public override void ParseData(string[] pData)\n')
    fileHandlerModel.write(Consts.TAB + Consts.TAB + '{\n')
    fileHandlerModel.write(Consts.TAB + Consts.TAB + Consts.TAB + 'if (pData == null || pData.Length < ' + str(len(names)) + ')\n')
    fileHandlerModel.write(Consts.TAB + Consts.TAB + Consts.TAB + '{\n')
    fileHandlerModel.write(Consts.TAB + Consts.TAB + Consts.TAB + Consts.TAB + 'LogManager.LogError("' + fileName + '.ParseData param wrong!");\n')
    fileHandlerModel.write(Consts.TAB + Consts.TAB + Consts.TAB + Consts.TAB + 'return;\n')
    fileHandlerModel.write(Consts.TAB + Consts.TAB + Consts.TAB + '}\n')
    fileHandlerModel.write('\n')
    for index in range(len(names)):
        fileHandlerModel.write(Consts.TAB + Consts.TAB + Consts.TAB + 'if (!string.IsNullOrEmpty(pData[' + str(index) + ']))\n')
        fileHandlerModel.write(Consts.TAB + Consts.TAB + Consts.TAB + '{\n')
        dataType = types[index]
        rawDataString = 'pData[' + str(index) + ']'
        parsedDataString = ''
        if Tools.IsArrayType(dataType):
            if dataType == Consts.DATA_TYPE_ARRAY_STRING:
                parsedDataString = 'TableParser.ParseArrayData(' + rawDataString + ')'
            else:
                parsedDataString = 'TableParser.ParseArrayData<' + dataType[0:len(dataType) - 2] + '>(' + rawDataString + ')'
        elif dataType == Consts.DATA_TYPE_STRING:
            parsedDataString = rawDataString
        elif dataType == Consts.DATA_TYPE_BOOL:
            parsedDataString = '!' + rawDataString + '.Equals("0")'
        else:
            parsedDataString = dataType + '.Parse(' + rawDataString + ')'
        fileHandlerModel.write(Consts.TAB + Consts.TAB + Consts.TAB + Consts.TAB + names[index] + ' = ' + parsedDataString + ';\n')
        fileHandlerModel.write(Consts.TAB + Consts.TAB + Consts.TAB + '}\n')

    fileHandlerModel.write(Consts.TAB + Consts.TAB + '}\n')
    fileHandlerModel.write(Consts.TAB + '}\n')
    fileHandlerModel.write('}')
    fileHandlerModel.close()

    fileHandlerController = codecs.open(Consts.CODE_ROOT_PATH + Consts.CODE_SUBFOLDER_CONTROLLER + fileName + Consts.CODE_CONTROLLER_NAME_SUFFIX + '.cs', 'w', Consts.CS_CODE_ENCODING)
    fileHandlerController.write('using System.Collections.Generic;\n\n')
    fileHandlerController.write('namespace ' + Consts.NAMESPACE + '\n')
    fileHandlerController.write('{\n')
    fileHandlerController.write(Consts.TAB + 'public class ' + fileName + Consts.CODE_CONTROLLER_NAME_SUFFIX + ' : ' + Consts.CODE_CONTROLLER_BASE_CLASS + '<' + fileName + '>\n')
    fileHandlerController.write(Consts.TAB + '{\n')
    fileHandlerController.write(Consts.TAB + Consts.TAB + 'protected override string GetFileName()\n')
    fileHandlerController.write(Consts.TAB + Consts.TAB + '{\n')
    fileHandlerController.write(Consts.TAB + Consts.TAB + Consts.TAB + 'return "' + fileName + '";\n')
    fileHandlerController.write(Consts.TAB + Consts.TAB + '}\n\n')
    fileHandlerController.write(Consts.TAB + Consts.TAB + 'protected override void AddPrimaryDict(' + fileName + ' pModel)\n')
    fileHandlerController.write(Consts.TAB + Consts.TAB + '{\n')
    if types[0] == Consts.DATA_TYPE_STRING:
        fileHandlerController.write(Consts.TAB + Consts.TAB + Consts.TAB + 'primaryDict[pModel.' + names[0] + '] = pModel;\n')
    else:
        fileHandlerController.write(Consts.TAB + Consts.TAB + Consts.TAB + 'primaryDict[pModel.' + names[0] + '.ToString()] = pModel;\n')
    fileHandlerController.write(Consts.TAB + Consts.TAB + '}\n')
    indexesNames = []
    for index in range(len(indexes)):
        if indexes[index].lower() == Consts.CONFIG_INDEX.lower():
            indexesNames.append(names[index])

    if len(indexesNames) == 1:
        pass
    if indexes[0].lower() == Consts.CONFIG_INDEX.lower():
        raise Exception("Primary can't be the only index!")
    elif len(indexesNames) > 0:
        fileHandlerController.write('\n')
        fileHandlerController.write(Consts.TAB + Consts.TAB + 'protected override void AddIndexesDict(' + fileName + ' pModel)\n')
        fileHandlerController.write(Consts.TAB + Consts.TAB + '{\n')
        getIndexesKeyParams = ''
        for index in range(len(indexesNames)):
            if index != len(indexesNames) - 1:
                getIndexesKeyParams = getIndexesKeyParams + 'pModel.' + indexesNames[index] + '.ToString(), '
            else:
                getIndexesKeyParams = getIndexesKeyParams + 'pModel.' + indexesNames[index] + '.ToString()'

        fileHandlerController.write(Consts.TAB + Consts.TAB + Consts.TAB + 'string tKey = GetIndexesKey(' + getIndexesKeyParams + ');\n')
        fileHandlerController.write(Consts.TAB + Consts.TAB + Consts.TAB + 'if (!indexesDict.ContainsKey(tKey))\n')
        fileHandlerController.write(Consts.TAB + Consts.TAB + Consts.TAB + '{\n')
        fileHandlerController.write(Consts.TAB + Consts.TAB + Consts.TAB + Consts.TAB + 'indexesDict[tKey] = new List<' + fileName + '>();\n')
        fileHandlerController.write(Consts.TAB + Consts.TAB + Consts.TAB + '}\n\n')
        fileHandlerController.write(Consts.TAB + Consts.TAB + Consts.TAB + 'indexesDict[tKey].Add(pModel);\n')
        fileHandlerController.write(Consts.TAB + Consts.TAB + '}\n')

    fileHandlerController.write(Consts.TAB + '}\n')
    fileHandlerController.write('}')
    fileHandlerController.close()
 
    constIndexes = []
    for index in range(len(consts)):
        if str(consts[index]).strip() != '':
            constIndexes.append(index)
           
    if len(constIndexes) > 1:
        raise Exception('Const can only mark one column!')
    if len(constIndexes) == 1:
        constIndex = constIndexes[0]
        WriteConstCodes(fileName, str(consts[constIndex]).strip(), types[0], types[constIndex], constIndex)


def Write(excelPath, fileName):
    coloredPrint.print_blue_text('>>>>>>> Build Config: ' + fileName)

    excelAllData = Tools.GetExcelFileAllData(excelPath)
    excelData = excelAllData[0]
    csvFileName = Consts.CONFIG_CSV_FILE_NAME_PREFIX + fileName + Consts.CONFIG_CSV_FILE_NAME_SUFFIX
    
    if Consts.isWriteAllSheet:
        WriteCSVFileAllSheet(excelAllData, csvFileName)
    else:
        WriteCSVFile(excelData, csvFileName) 
    # WriteCSVFile(excelData, csvFileName)
    WriteCodes(excelData, csvFileName)

    coloredPrint.print_green_text('<<<<<<< Complete Build Config: ' + fileName)
