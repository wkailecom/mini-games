using System;

namespace Config
{
    public static class TableParser
    {
        const char ARRAY_DATA_SEPARATOR = '&';

        public static string[] ParseArrayData(string pArrayData)
        {
            return pArrayData.Split(ARRAY_DATA_SEPARATOR);
        }

        public static V[] ParseArrayData<V>(string pArrayData)
        {
            string[] tStringVs = pArrayData.Split(ARRAY_DATA_SEPARATOR);
            if (typeof(V) == typeof(bool))
            {
                for (int i = 0; i < tStringVs.Length; i++)
                {
                    if (string.IsNullOrEmpty(tStringVs[i]))
                    {
                        tStringVs[i] = "false";
                    }
                    else if (tStringVs[i].Equals("0"))
                    {
                        tStringVs[i] = "false";
                    }
                    else
                    {
                        tStringVs[i] = "true";
                    }
                }
            }

            V[] tResult = new V[tStringVs.Length];
            for (int i = 0; i < tStringVs.Length; i++)
            {
                tResult[i] = (V)Convert.ChangeType(tStringVs[i], typeof(V));
            }

            return tResult;
        }
    }
}