using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TMS.Infrastructure.Utils
{
    public static class Validates
    {
        //public static bool CheckLimit(int ServId, float Amt, out Single MaxLimit, out Single MinLimit, out string Msg)
        //{
        //    bool flag = false;
        //    Msg = "";
        //    // MaxLimit = Single.Parse(DB.ExecuteScalar(ConnectionString2, "GetLimit", (ServId)).ToString());
        //    DataSet myDataSet = DB.ExecuteDataset(ConnectionString2, "GetLimit", ServId);

        //    MaxLimit = Convert.ToSingle(myDataSet.Tables[0].Rows[0][1]);
        //    MinLimit = Convert.ToSingle(myDataSet.Tables[0].Rows[0][0]);

        //    if (MaxLimit != 0 && Amt > MaxLimit)
        //    {
        //        flag = true;
        //        Msg = " لا يسمح بقيمه اكبر من " + MaxLimit + " جنيها ";
        //    }
        //    else if (MinLimit != 0 && Amt < MinLimit)
        //    {
        //        flag = true;
        //        Msg = " لا يسمح بقيمه اقل من " + MinLimit + " جنيها ";
        //    }

        //    if (flag)
        //        return false;
        //    else
        //        return true;
        //}

        //public static bool CheckTopUpLimit(string Provider, int CenterId)
        //{
        //    int flag = 1;



        //    DataSet myDataSet = DB.ExecuteDataset(ConnectionString2, "GetLimitTopUp", Provider, CenterId);

        //    flag = Convert.ToInt32(myDataSet.Tables[0].Rows[0][0]);



        //    if (flag == 0)
        //        return false;
        //    else
        //        return true;
        //}

        public static bool CheckInteger(string Val)
        {

            foreach (char c in Val)
            {
                if (!Char.IsNumber(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckJSON(this string s)
        {
            try
            {
                JToken.Parse(s);
                return true;
            }
            catch (JsonReaderException ex)
            {
                //Trace.WriteLine(ex);
                return false;
            }
        }

        public static bool CheckLandLineNumber(string Number)
        {
            Regex rgx1 = new Regex(@"^(03|068|097|082|057|064|095|050|048|066|065|062|069|055|088|013|045|084|047|093|096|013|055|092|040|046|086)[0-9]{7}$");
            Regex rgx2 = new Regex(@"^(02)[0-9]{8}$");
            if (rgx1.IsMatch(Number))
            {
                return true;
            }
            else if (rgx2.IsMatch(Number))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool CheckEmptyString(string S)
        {

            if (!string.IsNullOrEmpty(S))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool CheckMobileNumber(string s)
        {
            Regex rgx = new Regex(@"^[0-9]{11}$");
            if (rgx.IsMatch(s))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool CheckSSN(string s)
        {
            Regex rgx = new Regex(@"^[0-9]{14}$");
            if (rgx.IsMatch(s))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckDate(string From, string To)
        {
            int value = DateTime.Compare(Convert.ToDateTime(From), Convert.ToDateTime(To));

            // checking 
            if ((value > 0) || (value == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string[] GetCodeAndTelephone(string Number)
        {
            string[] CodeAndTelephone = new string[2];
            string Code = "";
            Regex rgx1 = new Regex(@"^(03|068|097|082|057|064|095|050|048|066|065|062|069|055|088|013|045|084|047|093|096|013|055|092|040|046|086)[0-9]{7}$");
            Regex rgx2 = new Regex(@"^(02)[0-9]{8}$");
            if (rgx1.IsMatch(Number))
            {

                if (Number.Substring(0, 2) == "03")
                {
                    CodeAndTelephone[0] = Number.Substring(0, 2);


                }
                else if (Number.Substring(0, 3) == "068" || Number.Substring(0, 3) == "097" || Number.Substring(0, 3) == "082" || Number.Substring(0, 3) == "057" || Number.Substring(0, 3) == "064"
                         || Number.Substring(0, 3) == "095" || Number.Substring(0, 3) == "050" || Number.Substring(0, 3) == "048" || Number.Substring(0, 3) == "066" || Number.Substring(0, 3) == "065"
                         || Number.Substring(0, 3) == "062" || Number.Substring(0, 3) == "069" || Number.Substring(0, 3) == "055" || Number.Substring(0, 3) == "088" || Number.Substring(0, 3) == "013"
                         || Number.Substring(0, 3) == "045" || Number.Substring(0, 3) == "084" || Number.Substring(0, 3) == "047" || Number.Substring(0, 3) == "093" || Number.Substring(0, 3) == "096"
                         || Number.Substring(0, 3) == "013" || Number.Substring(0, 3) == "055" || Number.Substring(0, 3) == "092" || Number.Substring(0, 3) == "040" || Number.Substring(0, 3) == "046"
                         || Number.Substring(0, 3) == "086")
                {
                    CodeAndTelephone[0] = Number.Substring(0, Number.Substring(0, 3).Length);

                }
                CodeAndTelephone[1] = Number.Substring(CodeAndTelephone[0].Length, Number.Length - CodeAndTelephone[0].Length);
                //return true;
            }
            else if (rgx2.IsMatch(Number))
            {
                CodeAndTelephone[0] = Number.Substring(0, Number.Substring(0, 2).Length);
                CodeAndTelephone[1] = Number.Substring(CodeAndTelephone[0].Length, Number.Length - Number.Substring(0, 2).Length);
            }

            return CodeAndTelephone;
        }
        
    }
}
