# -*- coding: utf-8 -*-
import codecs
import ColoredPrint
import Consts
import Tools

coloredPrint = ColoredPrint.ColoredPrint()

def WriteCSVFile(excelData, csvFileName):
    csvFileHandle = codecs.open(Consts.CSV_ROOT_PATH + csvFileName + ".csv", "w", Consts.CSV_ENCODING)
    
    #在同一个excel数据中，读取任意一列的数据，得到的数组长度都是一样的，都为excelData.nrows
    dataTypes = Tools.ParseDataTypes(excelData.col_values(Consts.DEFINE_TYPE), False)
    dataValues = excelData.col_values(Consts.DEFINE_VALUE)
    
    for rowIndex in range(excelData.nrows):
        csvFileHandle.write(Tools.GetCsvCellDataByExcelCellData(dataValues[rowIndex], dataTypes[rowIndex]))
        if(rowIndex != excelData.nrows - 1):
            csvFileHandle.write("\n")
    csvFileHandle.close()


class ParsedDefineRowData:

    def __init__(self, excelRowData):
        self.annotation = str(excelRowData[Consts.DEFINE_ANNOTATION]).strip()
        self.name = str(excelRowData[Consts.DEFINE_NAME]).strip()
        self.type = Tools.ParseDataType(excelRowData[Consts.DEFINE_TYPE], True)
        self.value = Tools.GetCsvCellDataByExcelCellData(excelRowData[Consts.DEFINE_VALUE], self.type)
        if self.type == '':
            raise Exception
        if self.type == Consts.DATA_TYPE_BOOL:
            tInt = Tools.TryParseInt(self.value)
            tBool = bool(tInt)
            if tBool:
                self.value = 'true'
            else:
                self.value = 'false'
        elif self.type == Consts.DATA_TYPE_STRING:
            self.value = '"' + self.value + '"'
        elif self.type == Consts.DATA_TYPE_FLOAT:
            self.value = self.value + 'f'
        elif Tools.IsArrayType(self.type):
            values = self.value.split(Consts.ARRAY_DATA_SEPARATOR)
            self.value = '{ '
            for index in range(len(values)):
                if self.type == Consts.DATA_TYPE_ARRAY_BOOL:
                    tInt = Tools.TryParseInt(values[index])
                    tBool = bool(tInt)
                    if tBool:
                        values[index] = 'true'
                    else:
                        values[index] = 'false'
                if index > 0:
                    self.value += ', '
                if self.type == Consts.DATA_TYPE_ARRAY_STRING:
                    self.value += '"'
                self.value += values[index]
                if self.type == Consts.DATA_TYPE_ARRAY_STRING:
                    self.value += '"'
                elif self.type == Consts.DATA_TYPE_ARRAY_FLOAT:
                    self.value += 'f'

            self.value += ' }'


def WriteCodes(excelData, fileName):
    modelName = fileName + Consts.CODE_MODEL_NAME_SUFFIX
    fileHandlerModel = codecs.open(Consts.CODE_ROOT_PATH + Consts.CODE_SUBFOLDER_MODEL + modelName + '.cs', 'w', Consts.CS_CODE_ENCODING)
    fileHandlerModel.write('namespace ' + Consts.NAMESPACE + '\n')
    fileHandlerModel.write('{\n')
    fileHandlerModel.write(Consts.TAB + 'public class ' + fileName + '\n')
    fileHandlerModel.write(Consts.TAB + '{\n')
    for rowIndex in range(excelData.nrows):
        parsedDefineRowData = ParsedDefineRowData(excelData.row_values(rowIndex))
        fileHandlerModel.write(Consts.TAB + Consts.TAB + '///<summary>\n')
        fileHandlerModel.write(Consts.TAB + Consts.TAB + '///' + parsedDefineRowData.annotation + '\n')
        fileHandlerModel.write(Consts.TAB + Consts.TAB + '///</summary>\n')
        fileHandlerModel.write(Consts.TAB + Consts.TAB + 'public static ' + parsedDefineRowData.type + ' ' + parsedDefineRowData.name + ' = ' + parsedDefineRowData.value + ';\n')

    fileHandlerModel.write(Consts.TAB + '}\n')
    fileHandlerModel.write('}')
    fileHandlerModel.close()
    fileHandlerController = codecs.open(Consts.CODE_ROOT_PATH + Consts.CODE_SUBFOLDER_CONTROLLER + fileName + Consts.CODE_CONTROLLER_NAME_SUFFIX + '.cs', 'w', Consts.CS_CODE_ENCODING)
    fileHandlerController.write('namespace ' + Consts.NAMESPACE + '\n')
    fileHandlerController.write('{\n')
    fileHandlerController.write(Consts.TAB + 'public class ' + fileName + Consts.CODE_CONTROLLER_NAME_SUFFIX + '\n')
    fileHandlerController.write(Consts.TAB + '{\n')
    fileHandlerController.write(Consts.TAB + Consts.TAB + 'const string FILE_NAME = "' + fileName + '";\n')
    fileHandlerController.write('\n')
    fileHandlerController.write(Consts.TAB + Consts.TAB + 'public void LoadData(string pFolderPath)\n')
    fileHandlerController.write(Consts.TAB + Consts.TAB + '{\n')
    fileHandlerController.write(Consts.TAB + Consts.TAB + Consts.TAB + 'TableFileReader tFileReader = new TableFileReader(pFolderPath + "/" + FILE_NAME);\n')
    rawDataString = 'tFileReader.ReadLine()[0]'
    parsedDataString = ''
    for rowIndex in range(excelData.nrows):
        parsedDefineRowData = ParsedDefineRowData(excelData.row_values(rowIndex))
        dataType = parsedDefineRowData.type
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
        fileHandlerController.write(Consts.TAB + Consts.TAB + Consts.TAB + modelName + '.' + parsedDefineRowData.name + ' = ' + parsedDataString + ';\n')

    fileHandlerController.write(Consts.TAB + Consts.TAB + '}\n')
    fileHandlerController.write(Consts.TAB + '}\n')
    fileHandlerController.write('}')
    fileHandlerController.close()


def Write(excelPath, fileName):
    coloredPrint.print_blue_text(">>>>>>> Build Define: " + fileName)

    excelData = Tools.GetExcelFileData(excelPath)
    csvFileName = Consts.DEFINE_CSV_FILE_NAME_PREFIX + fileName + Consts.DEFINE_CSV_FILE_NAME_SUFFIX
    
    WriteCSVFile(excelData, csvFileName)
    WriteCodes(excelData, csvFileName)
    
    coloredPrint.print_green_text("<<<<<<< Complete Build Define: " + fileName)
