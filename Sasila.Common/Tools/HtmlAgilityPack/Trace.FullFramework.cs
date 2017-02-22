using System;
using System.Collections.Generic;
using System.Text;

namespace Sasila.Common.Tools
{
   partial class Trace
    {
       partial void WriteLineIntern(string message,string category)
       {
           System.Diagnostics.Debug.WriteLine(message,category);
       }
    }
}
