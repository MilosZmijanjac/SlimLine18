using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;

namespace InLine18
{
    class MyParser
    {
        public string parse(string s)
        {
            DataTable dt = new DataTable();
            var v = dt.Compute(s, "");
            return v.ToString();
        }
        public string parseString(string input)
        {
            input = input.Replace(" ", "");
            Regex literal_str = new Regex("\".*?\"");
            try
            {
                if (literal_str.IsMatch(input))
                {
                    input = input.Replace("+", "");
                    input = input.Replace("\"", "");
                    input = input.Insert(0, "\"");
                    input = input.Insert(input.Length, "\"");
                }
                else
                    input = parse(input);
            }
            catch (Exception e) { }
            return input;
        }
    }
}
