using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataQualityChecker.Helpers
{
    public class JSONHelper
    {
        public const string mainJSONStructure = "[*]";
        public const string repeatableJSONArrayStructure = "{*}";
        public const string repeatableJSONItemStructure = "\"{title}\": \"{value}\"";

        public static string BuildJSON(List<List<string>> cellData, List<string> headers)
        {
            string jsonString = "";
            for (int j = 0; j != cellData.Count; j++) 
            {
                string jsonRepeatString = "";
                for (int i = 0; i != cellData[j].Count; i++) 
                {
                    jsonRepeatString += repeatableJSONItemStructure.Replace("{title}", string.IsNullOrEmpty(headers[i]) ? ("Column" + i) : headers[i]).Replace("{value}", cellData[j][i]);
                    if (i != cellData[j].Count - 1) jsonRepeatString += ",";
                }
                jsonString += repeatableJSONArrayStructure.Replace("*", jsonRepeatString);
                if (j != cellData.Count - 1) jsonString += ",";
            }

            return mainJSONStructure.Replace("*", jsonString);
        }
    }
}
