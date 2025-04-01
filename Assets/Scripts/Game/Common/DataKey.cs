
namespace Game
{
    public class DataKey
    {
        //ECPM 累计值
        public const string ECPM_LOOP_SUM = "EcpmLoopSum_Key";                //激励和插屏Banner ECPM 累加值
        public const string ECPM_REPORT_SUM = "EcpmReportSum_Key";            //激励和插屏Banner ECPM 上报次数

        public const string BASE_DATA_REPORTED_KEY = "BaseDataReported";      //BaseData是否上报BQ
        public const string FIRST_VERSIOM_KEY = "AppFirstVersionKey";         //应用首次安装版本
        public const string IS_KILL_END_KEY = "IsKillEndKey";                 //是否杀端 
         
        public const string RestAdAccumulatedTime = "RestAdAccumulatedTime";  //休息一下广告累计时长 
    }
}