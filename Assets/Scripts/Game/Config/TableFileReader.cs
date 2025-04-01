using Game;
using System.IO;
using UnityEngine;

namespace Config
{
    public class TableFileReader
    {
        const char DATA_SEPARATOR_CHAR = '\t';

        MemoryStream mMemoryStream;
        StreamReader mStreamReader;

        public TableFileReader(string pFilePath)
        {
            TextAsset tTextAsset = AssetManager.Instance.LoadAsset<TextAsset>(pFilePath);
            if (tTextAsset == null)
            {
                var tDataPath = AppInfoManager.Instance.GetCurDataPath();
                if (pFilePath.Contains(tDataPath))
                {
                    pFilePath = pFilePath.Replace(tDataPath, GameConst.CONFIG_ROOT_PATH);
                    tTextAsset = AssetManager.Instance.LoadAsset<TextAsset>(pFilePath);
                }

                if (tTextAsset == null)
                {
                    LogManager.LogError($"TableFileReader: no file at path '{pFilePath}'");
                    return;
                }
            }

            mMemoryStream = new MemoryStream(tTextAsset.bytes, false);
            mStreamReader = new StreamReader(mMemoryStream);
            AssetManager.Instance.ReleaseAsset<TextAsset>(pFilePath);
        }

        public string[] ReadLine()
        {
            string tLineString = mStreamReader?.ReadLine();
            return tLineString?.Split(DATA_SEPARATOR_CHAR);
        }

        public void Dispose()
        {
            if (mStreamReader != null)
            {
                mStreamReader.Close();
                mStreamReader.Dispose();
                mStreamReader = null;
            }

            if (mMemoryStream != null)
            {
                mMemoryStream.Close();
                mMemoryStream.Dispose();
                mMemoryStream = null;
            }
        }
    }
}