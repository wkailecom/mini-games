# -*- coding: utf-8 -*-
import ColoredPrint
import Consts
import sys
import xlrd

coloredPrint = ColoredPrint.ColoredPrint()

#根据扩展名判断是否是Excel文件
def IsExcel(fileName):
    return fileName.endswith('.xls') or fileName.endswith('.xlsx')

#打开Excel文件
def OpenExcelFile(excelFilePath):
    try:
        return xlrd.open_workbook(excelFilePath)
    except Exception as e:
        coloredPrint.print_red_text('Open ' + excelFilePath + " failed")
        raise Exception

#获得Excel文件中的所有Sheet的数据
def GetExcelFileAllData(excelFilePath):
    excelFile = OpenExcelFile(excelFilePath)
    return excelFile.sheets()

#获得Excel文件中的第一个Sheet的数据
def GetExcelFileData(excelFilePath):
    return GetExcelFileAllData(excelFilePath)[0]
    
#根据文件路径获得文件名
def GetFileNameByFilePath(filePath):
    pointIndex = filePath.rfind('.')
    if(pointIndex > 0):
        return filePath[filePath.rfind(Consts.FILE_PATH_SEPARATOR) + 1 : pointIndex]
    else:
        return filePath[filePath.rfind(Consts.FILE_PATH_SEPARATOR) + 1 : ]

#首字母大写
def UpperFirstLetter(string):
    return string.upper()[0 : 1] + string[1 :]
 
#数据类型是否为数组
def IsArrayType(pDataType):
    if(pDataType.find('[]') >= 0):
        return True
    else:
        return False

#是否为固定长度的数组
def IsConstLengthArray(pDataType):
    return IsArrayType(pDataType) and len(pDataType) > (pDataType.find('[]') + 2)

#获取固定长度数组类型的长度
def GetLengthOfConstLengthArray(pDataType):
    lengthString = pDataType[pDataType.find('[]') + 2 :].strip()
    return TryParseInt(lengthString)

#获取固定长度数组类型对应的原类型
def GetOriginTypeOfConstLengthArray(pDataType):
    return pDataType[:pDataType.find('[]') + 2].strip()

#是否是指定的数组类型
def IsSpecifiedArrayType(pDataType, pArrayType):
    if(pDataType == pArrayType):
        return True
    if(pDataType.startswith(pArrayType)):
        GetLengthOfConstLengthArray(pDataType)
        return True
    return False

#检查数据类型是否有效
def CheckDataTypeValid(pDataType):
    if(pDataType == ''   #excel中有些列只是用作备注，类型一栏为空
    or pDataType == Consts.DATA_TYPE_BOOL
    or pDataType == Consts.DATA_TYPE_INT
    or pDataType == Consts.DATA_TYPE_LONG
    or pDataType == Consts.DATA_TYPE_FLOAT
    or pDataType == Consts.DATA_TYPE_DOUBLE
    or pDataType == Consts.DATA_TYPE_STRING
    #限定长度的数组
    or IsSpecifiedArrayType(pDataType, Consts.DATA_TYPE_ARRAY_BOOL)
    or IsSpecifiedArrayType(pDataType, Consts.DATA_TYPE_ARRAY_INT)
    or IsSpecifiedArrayType(pDataType, Consts.DATA_TYPE_ARRAY_LONG)
    or IsSpecifiedArrayType(pDataType, Consts.DATA_TYPE_ARRAY_FLOAT)
    or IsSpecifiedArrayType(pDataType, Consts.DATA_TYPE_ARRAY_DOUBLE)
    or IsSpecifiedArrayType(pDataType, Consts.DATA_TYPE_ARRAY_STRING)):
        return True

    coloredPrint.print_red_text("数据类型不支持 " + pDataType)
    raise Exception

#解析数据类型
def ParseDataType(pDataType, pConvertConstLengthArrayTypeToOriginType):
    pDataType = str(pDataType).strip()
    CheckDataTypeValid(pDataType)

    if(IsConstLengthArray(pDataType) and pConvertConstLengthArrayTypeToOriginType):
        pDataType = GetOriginTypeOfConstLengthArray(pDataType)

    return pDataType
        
#解析一组数据类型
def ParseDataTypes(pDataTypes, pConvertConstLengthArrayTypeToOriginType):
    dataTypesResult = []
    for tDataType in pDataTypes:
        tDataType = ParseDataType(tDataType, pConvertConstLengthArrayTypeToOriginType)
        dataTypesResult.append(tDataType)
    return dataTypesResult

def TryParseInt(pCellData):
    pCellData = pCellData.strip()
    
    if(".0" in pCellData):#从excel中读取的整数会自动添加.0
        pCellData = pCellData[0:pCellData.find('.0')]
    try:
        tInt = int(pCellData)
    except ValueError:
        coloredPrint.print_red_text("%s.%s Failed, Value : %s " %(__file__, sys._getframe().f_code.co_name, pCellData))
        raise Exception
    return tInt

def TryParseFloat(pCellData):
    pCellData = pCellData.strip()
    
    try:
        tFloat = float(pCellData)
    except ValueError:
        coloredPrint.print_red_text("%s.%s Failed, Value : %s " %(__file__, sys._getframe().f_code.co_name, pCellData))
        raise Exception
        
    return tFloat

def TryParseBool(pCellData):
    pCellData = pCellData.strip()

    tInt = TryParseInt(pCellData)#要先转换成int再转换成bool，因为字符串‘0’转换成bool是true
    return bool(tInt)

#根据Excel中的一格数据获得CSV数据
def GetCsvCellDataByExcelCellData(pExcelCellData, pDataType):
    pExcelCellData = str(pExcelCellData).strip()
    if(len(pExcelCellData) == 0):
        if(pDataType == Consts.DATA_TYPE_BOOL or pDataType == Consts.DATA_TYPE_ARRAY_BOOL):
            return '0'
        return pExcelCellData
        
    if(pDataType == Consts.DATA_TYPE_BOOL):
        if(TryParseBool(pExcelCellData)):
            return '1'
        else:
            return '0'
            
    if(pDataType == Consts.DATA_TYPE_INT or pDataType == Consts.DATA_TYPE_LONG):#3.0以后没有long,和int情况相同
        return str(TryParseInt(pExcelCellData))
        
    if(pDataType == Consts.DATA_TYPE_FLOAT or pDataType == Consts.DATA_TYPE_DOUBLE):#python没有double,和float情况相同
        return str(TryParseFloat(pExcelCellData))
        
    if(pDataType == Consts.DATA_TYPE_STRING):
        return pExcelCellData
    
    #数组
    tArrayValue = pExcelCellData.split(Consts.ARRAY_DATA_SEPARATOR)
    tArryLength = len(tArrayValue)
    if(IsConstLengthArray(pDataType)):
        tConstLength = GetLengthOfConstLengthArray(pDataType)
        if(tConstLength != tArryLength):
            coloredPrint.print_red_text("%s.%s：固定数组长度与数据长度不等 —— Type: %s Data: %s " %(__file__, sys._getframe().f_code.co_name, pDataType, pExcelCellData))
            raise Exception
        pDataType = GetOriginTypeOfConstLengthArray(pDataType)

    tResultString = ''
    if(pDataType == Consts.DATA_TYPE_ARRAY_BOOL):
        for item in tArrayValue:
            if(TryParseBool(item)):
                item = '1'
            else:
                item = '0'
            if(tResultString == ""):
                tResultString = item
            else:
                tResultString = tResultString + Consts.ARRAY_DATA_SEPARATOR + item
        return tResultString
        
    if(pDataType == Consts.DATA_TYPE_ARRAY_INT or pDataType == Consts.DATA_TYPE_ARRAY_LONG):#3.0以后没有long,和int情况相同
        for item in tArrayValue:
            item = str(TryParseInt(item))
            if(tResultString == ""):
                tResultString = item
            else:
                tResultString = tResultString + Consts.ARRAY_DATA_SEPARATOR + item
        return tResultString
        
    if(pDataType == Consts.DATA_TYPE_ARRAY_FLOAT or pDataType == Consts.DATA_TYPE_ARRAY_DOUBLE):#python没有double,和float情况相同
        for item in tArrayValue:
            item = str(TryParseFloat(item))
            if(tResultString == ""):
                tResultString = item
            else:
                tResultString = tResultString + Consts.ARRAY_DATA_SEPARATOR + item
        return tResultString
        
    if(pDataType == Consts.DATA_TYPE_ARRAY_STRING):
        for item in tArrayValue:
            item = item.strip()
            if(tResultString == ""):
                tResultString = item
            else:
                tResultString = tResultString + Consts.ARRAY_DATA_SEPARATOR + item
        return tResultString
    
    coloredPrint.print_red_text("%s.%s：未知类型 —— %s " %(__file__, sys._getframe().f_code.co_name, pDataType))
    raise Exception