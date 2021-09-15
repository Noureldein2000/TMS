using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Infrastructure.Utils
{
    public static class GetData
    {
        public static int GetCode(string msg)
        {
            int code = 0;
            try
            {
                if (msg.Contains('(') && msg.Contains(')'))
                {
                    int start = 0, end = 0;
                    for (int j = 0; j < msg.Length; j++)
                    {
                        string test = msg[j].ToString();
                        if (msg[j].ToString() == "(")
                            start = j;

                        else if (msg[j].ToString() == ")")
                            end = j;

                    }
                    start++;
                    return int.Parse(msg.Substring(start, end - start));
                }
                return code;
            }
            catch
            {
                //Trace.WriteLine(ex);
                return code;
            }
        }
    }
}
