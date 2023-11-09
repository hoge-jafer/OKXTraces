using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKX.UI.Helper
{
    public class IniConfigHelper
    {
        /// <summary>
        /// 声明INI文件的写操作函数 WritePrivateProfileString()
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// 声明INI文件的读操作函数 GetPrivateProfileString()
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="retVal"></param>
        /// <param name="size"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        private string sPath = null;

        /// <summary>
        /// 传入地址
        /// </summary>
        /// <param name="path">路径</param>
        public IniConfigHelper(string path)
        {
            this.sPath = path;
        }

        /// <summary>
        /// 写入值
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <param name="value">键值</param>
        public void ConfigWriteValue(string section, string key, string value)
        {
            // section=配置节，key=键名，value=键值，path=路径
            WritePrivateProfileString(section, key, value, sPath);
        }

        /// <summary>
        /// 读取值
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public string ConfigReadValue(string section, string key)
        {
            // 每次从ini中读取多少字节
            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
            // section=配置节，key=键名，temp=上面，读取该键名的值最大长度，path=路径
            GetPrivateProfileString(section, key, "", temp, 255, sPath);
            return temp.ToString();
        }

    }
}
