using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sasila.Common.Tools;

namespace Sasila.Common.Http
{
    /// <summary>
    /// 网络访问设置
    /// </summary>
    public class HttpWebSet
    {
        /// <summary>
        /// 网络异常重试次数
        /// </summary>
        public int WebExceptionRetries { get; set; }
        /// <summary>
        /// 网络访问间隔最小时间（毫秒）
        /// </summary>
        public int WebWaitMin { get; set; }
        /// <summary>
        /// 网络访问间隔最大时间（毫秒）
        /// </summary>
        public int WebWaitMax { get; set; }
        /// <summary>
        /// 暂停程序等待时间（毫秒）
        /// </summary>
        public int PauseWaitTime { get; set; }

        /// <summary>
        /// 网络访问等待
        /// </summary>
        public void WebWait() 
        {
            if (WebWaitMin > 0 && WebWaitMax >= WebWaitMin)
            {
                int space = 0;
                if (WebWaitMax == WebWaitMin)
                {
                    space = WebWaitMax;
                }
                else
                {
                    space = RandomHelper.GetIntRandom(WebWaitMin, WebWaitMax);
                }
                Thread.Sleep(space);
            }
        }

        /// <summary>
        /// 程序暂停等待
        /// </summary>
        public void PauseWait() 
        {
            if (this.PauseWaitTime > 0)
            {
                Thread.Sleep(this.PauseWaitTime);
            }
            else
            {
                Thread.Sleep(500);
            }
        }
    }
}
