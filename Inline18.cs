using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace InLine18
{
    class Inline18
    {
        Dictionary<string, string> variables = new Dictionary<string, string>();
        private List<string> busyFiles = new List<string>();

        Syntax syntax = new Syntax();
        MyParser expPars = new MyParser();

        bool IsVarDeclared(string var)
        {
            return variables.ContainsKey(var);
        }
        string VarReturn(string input)
        {
            string oldStr, newStr, varName;
            Regex varRex = new Regex(@"\[(?'var'[A-z]+[0-9]*[_\.\$#]*?)\]");
            try
            {
                while (varRex.IsMatch(input))
                {
                    oldStr = varRex.Match(input).Value.ToString();
                    varName = varRex.Match(input).Groups["var"].ToString();
                    newStr = variables[varName].ToString();
                    if (IsVarDeclared(varName))
                        input = input.Replace(oldStr, newStr);
                    else
                    {
                        Console.WriteLine("Variable " + varName + " not declared"); return "";
                    }
                }
            }
            catch (Exception e)
            { Console.WriteLine("Error"); Console.WriteLine(e.Message); }
            return input;
        }
        string VarOutput(string input)
        {
            string oldStr, newStr, varName;
            Regex varRex = new Regex(@"=\[(?'var'.+?)\]");
            try
            {
                while (varRex.IsMatch(input))
                {
                    oldStr = varRex.Match(input).Value.ToString();
                    varName = varRex.Match(input).Groups["var"].ToString();
                    newStr = variables[varRex.Match(input).Groups["var"].ToString()].ToString();
                    if (IsVarDeclared(varName))
                        input = input.Replace(oldStr, newStr);
                    else
                    { Console.WriteLine("Variable " + varName + " not declared"); return ""; }

                }
            }
            catch (Exception e) { Console.WriteLine("Error"); Console.WriteLine(e.Message); }
            input = input.Replace("\"", "");
            return input;
        }
        string DeclareVar(string input)
        {

            Regex decRex = new Regex(@"^\[(?'var'.*)\] *= *(?'rest'[^\}]*)");
            string varName = decRex.Match(input).Groups["var"].ToString();
            string restLine = decRex.Match(input).Groups["rest"].ToString();
            string temp = restLine;
            restLine = VarReturn(restLine);
            if (restLine == "") return restLine;

            if (!IsVarDeclared(varName))
            {
                variables.Add(varName, expPars.parseString(restLine));
            }
            else
            {
                string t = expPars.parseString(restLine);
                if (t.Contains("\"") == variables[varName].Contains("\""))
                {
                    variables[varName] = t;
                }
                else
                {
                    Console.WriteLine("<< Error variable " + varName + " declared with different type >>");
                }
            }
            input = input.Replace(temp, "");
            input = input.TrimEnd('=');
            return input;
        }
        string InlineBlock(string input)
        {
            Regex inRex = new Regex(@"@\{(?'inline'.*)\}\s?");
            string oldStr, newStr;
            while (inRex.IsMatch(input))
            {
                oldStr = inRex.Match(input).Value.ToString();
                newStr = inRex.Match(input).Groups["inline"].ToString();
                input = input.Replace(oldStr, "");
                DeclareVar(newStr);
            }
            return input;
        }
        void IncludeFile(string input)
        {
            Regex incRex = new Regex(@"include (?'path'.+\.txt)");
            string oldStr, newStr;
            while (incRex.IsMatch(input))
            {
                oldStr = incRex.Match(input).Value.ToString();
                newStr = Path.GetFullPath(incRex.Match(input).Groups["path"].ToString());
                try
                {
                    if (!File.Exists(newStr))
                        Console.WriteLine("The file " + newStr + " doesn't exist");
                    Run(newStr);
                    input = input.Replace(oldStr, "");
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file " + newStr + " could not be read");
                    Console.WriteLine(e.Message);

                }

            }
        }
        string Tags(string input)
        {
            Regex tagRex = new Regex(@"<all_caps>+(?'tag'.*)</all_caps>+");
            string oldStr, newStr;
            while (tagRex.IsMatch(input))
            {
                oldStr = tagRex.Match(input).Value.ToString();
                newStr = tagRex.Match(input).Groups["tag"].ToString();
                input = input.Replace(oldStr, newStr.ToUpper());
            }
            return input;
        }

        public void Run(string putanja)
        {
            if (busyFiles.Contains(putanja)) return;
            busyFiles.Add(putanja);
            using (StreamReader sr = new StreamReader(putanja))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();                            
                    if (!syntax.IsValidLine(line))
                        return ;
                    if (syntax.IsValidPath(line))
                        IncludeFile(line);
                    if (syntax.IsDeclaration(line))
                        DeclareVar(line);
                    if (!syntax.IsDeclaration(line) && !syntax.IsValidPath(line))
                        Console.WriteLine(Tags(VarOutput(InlineBlock(line))));
                    
                }

            }
            busyFiles.Remove(putanja);
        }
    }
}
