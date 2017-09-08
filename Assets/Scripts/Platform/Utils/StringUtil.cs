using System.Collections.Generic;

namespace Platform.Utils
{
    /// <summary>
    /// 字符串工具类
    /// </summary>
    class StringUtil
    {
        /// <summary>
        /// 将启动连接参数转为字典
        /// </summary>
        /// <param name="paramsStr"></param>
        /// <returns></returns>
        public static Dictionary<string,string> ParseParam(string paramsStr)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (paramsStr == null)
            {
                return result;
            }
            string[] paramArr = paramsStr.Split('&');
            for (int i = 0; i < paramArr.Length; i++)
            {
                string[] paramItem = paramArr[i].Split('=');
                result.Add(paramItem[0], paramItem[1]);
            }
            return result;
        }
    }
}
