using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Sasila.Common.Tools
{
    /// <summary>
    /// winrar压缩类（根目录需要有Rar.exe），基于命令压缩
    /// </summary>
    public class RarHelper
    {
        /// <summary>
        /// 压缩多个文件，注意处理异常
        /// </summary>
        /// <param name="files">文件名</param>
        /// <param name="rarFileName">压缩包文件名</param>
        /// <returns>返回0成功，其他则是相应错误码。</returns>
        public static int Rar(string[] files, string rarFileName)
        {
            var rarFiles = files.Where(f => File.Exists(f)).ToArray();
            if (null == rarFiles || rarFiles.Length == 0)
            {
                throw new FileNotFoundException("未找到指定压缩的文件");
            }
            List<string> args = new List<string>();
            args.Add("a");
            args.Add("-m5");
            args.Add("-ep");
            args.Add(rarFileName);
            args.Add("a");
            args.AddRange(rarFiles);

            Process rarProcess = new Process();
            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.FileName = "Rar.exe";
            startinfo.WindowStyle = ProcessWindowStyle.Hidden;
            startinfo.Arguments = string.Join(" ", args.ToArray());
            rarProcess.StartInfo = startinfo;
            rarProcess.Start();
            rarProcess.WaitForExit();
            int exitCode = rarProcess.ExitCode;
            return exitCode;
        }
    }
}
