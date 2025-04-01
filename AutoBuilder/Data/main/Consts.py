# -*- coding: utf-8 -*-
import Tools

pathConfig = Tools.GetExcelFileData('PathConfig.xls').col_values(0)
isWriteAllSheet = False  # 是否写入所有表格Sheet

#源Excel文件目录
EXCLE_ROOT_PATH = pathConfig[0]
#生成的CSV文件目录
CSV_ROOT_PATH = pathConfig[1]
#生成的代码文件目录
CODE_ROOT_PATH = pathConfig[2]
#生成的代码Model文件目录
CODE_SUBFOLDER_MODEL = 'Model/'
#生成的代码Controller文件目录
CODE_SUBFOLDER_CONTROLLER = 'Controller/'

#忽略名字以此开头的文件夹或文件
IGNORE_PREFIX = '_'

#识别Define文件的后缀
DEFINE_SUFFIX = "Define"

#DataManager名字
DATA_MANAGER_NAME = "ConfigData"

#配置表索引标识
CONFIG_INDEX = "index"

#制表符
TAB = "\t"
#文件路径分隔符
FILE_PATH_SEPARATOR = '/'


CS_CODE_ENCODING = 'utf-8'
CSV_ENCODING = 'utf-16'  #使用utf-16的原因：excel能够正确识别用gb2312、gbk、gb18030或utf_8 with BOM 编码的中文，如果是utf_8 no BOM编码的中文文件，excel打开会乱码
                         #而如果使用"utf-8-sig"（utf_8 with BOM），则打开之后所有列的数据都挤在了一列，使用utf-32没有必要，会增加文件大小，所以使用utf-16

#配置表生成的CSV文件名称前缀、后缀
CONFIG_CSV_FILE_NAME_PREFIX = ''
CONFIG_CSV_FILE_NAME_SUFFIX = 'Config'
#定义表生成的CSV文件名称前缀、后缀
DEFINE_CSV_FILE_NAME_PREFIX = ''
DEFINE_CSV_FILE_NAME_SUFFIX = ''

#生成的文本文件数据分隔符
TEXT_DATA_SEPARATOR = '\t'

CODE_MODEL_NAME_SUFFIX = ''
CODE_CONTROLLER_NAME_SUFFIX = 'Controller'
NAMESPACE = 'Config'
CODE_CONTROLLER_BASE_CLASS = 'ConfigControllerBase'

#数据类型
DATA_TYPE_BOOL = 'bool'
DATA_TYPE_INT = 'int'
DATA_TYPE_LONG = 'long'
DATA_TYPE_FLOAT = 'float'
DATA_TYPE_DOUBLE = 'double'
DATA_TYPE_STRING = 'string'
DATA_TYPE_ARRAY_BOOL = 'bool[]'
DATA_TYPE_ARRAY_INT = 'int[]'
DATA_TYPE_ARRAY_LONG = 'long[]'
DATA_TYPE_ARRAY_FLOAT = 'float[]'
DATA_TYPE_ARRAY_DOUBLE = 'double[]'
DATA_TYPE_ARRAY_STRING = 'string[]'

#数组数据分隔符
ARRAY_DATA_SEPARATOR = "&"

      
#配置表格式
ANNOTATION = 0      #备注，生成程序的代码注释
CLIENT_NAME = 1     #客户端字段名称，如果不填，客户端会忽略此列
SERVER_NAME = 2     #服务器字段名称
DATA_TYPE = 3       #数据类型
INDEX_IDENTIFY = 4  #索引标识，填index表示索引，可支持多个索引（第一列默认是主键，主键也可以是索引，但不可以是唯一索引）
CONST_IDENTIFY = 5  #常量名，填入常量名的列会生成一个常量类。只可以标识一列，且该列必须是string类型。
                    #该列的每一个值对应常量的名字（会自动把路径拆分。例如：A/B/C，会取C为常量名），主键列（第一列）为常量的值。也可以标识主键列（如果主键列是string类型）
                    #如果值是int类型，则生成枚举，如果是string类型，则生成const string
HEADERS_COUNT = 6   #表头个数

#定义表格式
DEFINE_ANNOTATION = 0
DEFINE_NAME = 1
DEFINE_TYPE = 2
DEFINE_VALUE = 3
