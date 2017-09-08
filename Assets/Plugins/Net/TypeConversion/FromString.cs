using System.Text;

namespace LZR.Data.TypeConversion
{
    public class FromString
    {
        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <param name="content">文字内容</param>
        /// <param name="encoding">编码格式</param>
        /// <returns></returns>
        public static byte[] GetBytes(string content,Encoding encoding)
        {
            return encoding.GetBytes(content);
        }
    }
}
