using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace Sasila.Common.Http
{
    /// <summary>
    /// 存储请求的状态
    /// </summary>
    public class RequestState
    {
        /// <summary>
        /// 缓存大小1024
        /// </summary>
        public const int BUFFER_SIZE = 1024;
        /// <summary>
        /// 返回的html字符串
        /// </summary>
        public string resHtmlStr { get; set; }
        /// <summary>
        /// post提交数组
        /// </summary>
        public byte[] postData { get; set; }
        /// <summary>
        /// 读取数据bate数组
        /// </summary>
        public byte[] BufferRead { get; set; }
        /// <summary>
        /// 返回字节数组
        /// </summary>
        public byte[] resBytes { get; set; }
        /// <summary>
        /// 请求对象
        /// </summary>
        public HttpWebRequest request { get; set; }
        /// <summary>
        /// 响应对象
        /// </summary>
        public HttpWebResponse response { get; set; }
        /// <summary>
        /// 响应流
        /// </summary>
        public Stream streamResponse { get; set; }
        /// <summary>
        /// 是否返回字节数组
        /// </summary>
        public bool IsReturnBytes { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public RequestState()
        {
            postData = new byte[0];
            resBytes = new byte[0];
            BufferRead = new byte[BUFFER_SIZE];
            resHtmlStr = string.Empty;
            request = null;
            streamResponse = null;
            IsReturnBytes = false;
        }
    }
}
