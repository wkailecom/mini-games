using System.Collections.Generic;

namespace Config
{
    public abstract class ConfigControllerBase<T> where T : ConfigModelBase, new()
    {
        public IReadOnlyList<T> DataList => dataList;

        protected List<T> dataList;
        protected Dictionary<string, T> primaryDict;
        protected Dictionary<string, List<T>> indexesDict;

        public void LoadData(string pFolderPath, string pExtension = null)
        {
            dataList = new List<T>();

            string tableFilePath;
            if (pFolderPath.EndsWith("/"))
                tableFilePath = pFolderPath + GetFileName();
            else
                tableFilePath = pFolderPath + "/" + GetFileName();

            TableFileReader tFileReader = new TableFileReader(tableFilePath + pExtension);

            string[] tRowData = tFileReader.ReadLine();
            while (tRowData != null)
            {
                T tModel = new T();
                tModel.ParseData(tRowData);
                dataList.Add(tModel);

                tRowData = tFileReader.ReadLine();
            }

            tFileReader.Dispose();
            BuildDictionary();
        }

        protected abstract string GetFileName();

        void BuildDictionary()
        {
            primaryDict = new Dictionary<string, T>(dataList.Count);
            indexesDict = new Dictionary<string, List<T>>();

            for (int i = 0; i < dataList.Count; i++)
            {
                T tModel = dataList[i];

                AddPrimaryDict(tModel);
                AddIndexesDict(tModel);
            }
        }

        protected virtual void AddPrimaryDict(T pModel) { }

        protected virtual void AddIndexesDict(T pModel) { }

        public T GetByPrimary(int pPrimary, bool pLogErrorIfNull = true)
        {
            return GetByPrimary(pPrimary.ToString(), pLogErrorIfNull);
        }

        public T GetByPrimary(string pPrimary, bool pLogErrorIfNull = true)
        {
            T tResult;
            primaryDict.TryGetValue(pPrimary, out tResult);
            if (tResult == null && pLogErrorIfNull)
            {
                LogManager.LogError($"{typeof(T).Name}.GetByPrimary \"{pPrimary}\"  return null!");
            }

            return tResult;
        }

        public List<T> GetByIndexes(params int[] pIndexes)
        {
            string[] tParams = new string[pIndexes.Length];
            for (int i = 0; i < tParams.Length; i++)
            {
                tParams[i] = pIndexes[i].ToString();
            }

            return GetByIndexes(tParams);
        }

        public List<T> GetByIndexes(params string[] pIndexes)
        {
            List<T> tResults;
            indexesDict.TryGetValue(GetIndexesKey(pIndexes), out tResults);

            return tResults;
        }

        public T GetFirstOneByIndexes(params int[] pIndexes)
        {
            string[] tParams = new string[pIndexes.Length];
            for (int i = 0; i < tParams.Length; i++)
            {
                tParams[i] = pIndexes[i].ToString();
            }

            return GetFirstOneByIndexes(tParams);
        }

        public T GetFirstOneByIndexes(params string[] pIndexes)
        {
            List<T> tResults = GetByIndexes(pIndexes);
            if (tResults == null || tResults.Count == 0)
            {
                return default(T);
            }

            return tResults[0];
        }

        protected string GetIndexesKey(params string[] pIndexes)
        {
            if (pIndexes.Length == 0)
            {
                LogManager.LogError("GetIndexesKey params length is 0");
                return string.Empty;
            }

            string tResult = pIndexes[0];
            for (int i = 1; i < pIndexes.Length; i++)
            {
                tResult = tResult + "|" + pIndexes[i];
            }

            return tResult;
        }
    }
}