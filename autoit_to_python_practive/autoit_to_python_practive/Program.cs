using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Random rand = new Random();
        //dictionary key is the variable name. the value in the dic is the var type
        Dictionary<string, string> init_vars = new Dictionary<string, string>();
        Dictionary<string, string> assigned_vars = new Dictionary<string, string>();
        string input;
        int spaces = 0;
        List<string> str_arr;
        System.IO.StreamWriter write = new StreamWriter("", true);
        System.IO.StreamReader file = new StreamReader("");
        while ((input = file.ReadLine()) != null) {
            
            string line = "";
            bool empty_space = true;
            for(int i = 0; i < input.Length; i++)
            {
                if(input[i]!= ' ')
                {
                    empty_space = false;
                }
                if (!empty_space)
                {
                    line += input[i];
                }
            }
            str_arr = line.Split(' ').ToList<string>();

            if (str_arr.Count > 0)
            {
                if (str_arr[0] == "#include")
                {
                    str_arr[0] = "import";
                    str_arr[1] = str_arr[1].Remove(0, 1);
                    str_arr[1] = str_arr[1].Remove(str_arr[1].Length - 1, 1);
                    int idx = str_arr[1].IndexOf(".");
                    if (idx >= 0)
                    {
                        str_arr[1] = str_arr[1].Remove(idx, 4);
                    }
                    str_arr[1] += ".py";
                    write.WriteLine(string.Join(" ", str_arr));
                }
                else if (str_arr[0] == "Func")
                {
                    spaces += 4;
                    str_arr[0] = "def";
                    write.WriteLine(string.Join(" ", str_arr) + ":");
                }
                else if (str_arr[0] == "Local")
                {
                    string len_arr = "";
                    str_arr[0] = "";
                    if (str_arr.Count > 2)
                    {
                        str_arr[1] = str_arr[1].Substring(1);
                    }
                    else if (str_arr[1].Contains("[") || str_arr.Contains("]"))
                    {
                        str_arr[1] = str_arr[1].Substring(1);
                        int start = 0;
                        int finish = 0;
                        for (int i = 0; i < str_arr[1].Length; i++)
                        {
                            if (str_arr[1][i] == '[')
                            {
                                start = i;
                            }
                            if (str_arr[1][i] == ']')
                            {
                                finish = i;
                            }
                        }
                        len_arr = str_arr[1].Split('[', ']')[1];
                        str_arr[1] = str_arr[1].Remove(start, finish - start + 1);
                    }
                    if (line.Contains("="))
                    {
                        string type = "";
                        for (int i = 0; i < line.Length; i++)
                        {
                            if (line[i] == '=')
                            {
                                type = line;
                                type = type.Substring(i + 1);
                                type = find_type(type);
                            }
                        }
                        if (str_arr[1].Contains("["))
                        {
                            int start = 0;
                            int finish = 0;
                            for (int i = 0; i < str_arr[1].Length; i++)
                            {
                                if (str_arr[1][i] == '[')
                                {
                                    start = i;
                                }
                                if (str_arr[1][i] == ']')
                                {
                                    finish = i;
                                }
                            }
                            str_arr[1] = str_arr[1].Remove(start, finish - start + 1);
                        }
                        //if type is "number" that means it could be int, double or float
                        assigned_vars.Add(str_arr[1], type);
                        if (spaces > 0)
                        {
                            string sentence = "";
                            sentence = put_spaces(spaces);
                            write.WriteLine(sentence + string.Join("", str_arr));
                        }
                        else
                        {
                            write.WriteLine(string.Join(" ", str_arr));
                        }
                    }
                    else
                    {
                        if (line.Contains('['))
                        {
                            init_vars.Add(str_arr[1], "array " + len_arr);
                        }
                        else
                        {
                            init_vars.Add(str_arr[1], "");
                        }
                    }
                }
                else if (line.StartsWith("_ArrayUnique("))
                {
                    string sentence = "";
                    int rand_num;
                    string arguments = line.Split('(', ')')[1];
                    string[] args_arr = arguments.Split(' ');
                    args_arr[0] = args_arr[0].Substring(1);
                    args_arr[0] = args_arr[0].Remove(args_arr[0].Length - 1);
                    args_arr[1] = args_arr[1].Remove(args_arr[1].Length - 1);
                    string arr_len = init_vars[args_arr[0]].Split(' ')[1];
                    Int32.TryParse(arr_len, out int len);
                    Int32.TryParse(args_arr[1], out int min);
                    Int32.TryParse(args_arr[2], out int max);
                    sentence = args_arr[0] + " = [ ";
                    for (int i = 0; i < len; i++)
                    {
                        rand_num = rand.Next(min, max);
                        if (i < len - 1)
                        {
                            sentence += rand_num.ToString() + ", ";
                        }
                        else
                        {
                            sentence += rand_num.ToString() + " ]";
                        }
                    }
                    if (spaces > 0)
                    {
                        string sen = "";
                        sen = put_spaces(spaces);
                        write.WriteLine(sen + sentence);
                    }
                    else
                    {
                        write.WriteLine(sentence);
                    }
                }
                else if (line.StartsWith("If "))
                {
                    string sentence = "";
                    str_arr[0] = str_arr[0].ToLower();
                    sentence = put_spaces(spaces);
                    for (int i = 0; i < str_arr.Count; i++)
                    {
                        int idx = str_arr[i].IndexOf("$");
                        if (idx >= 0)
                        {
                            str_arr[i] = str_arr[i].Remove(idx, 1);
                        }
                        if (str_arr[i] == "Or" || str_arr[i] == "Not" || str_arr[i] == "And")
                        {
                            sentence += str_arr[i].ToLower() + " ";
                        }
                        else if (str_arr[i] == "=")
                        {
                            sentence += "== ";
                        }
                        else if (str_arr[i] != "Then")
                        {
                            sentence += str_arr[i] + " ";
                        }
                        else
                        {
                            sentence = sentence.TrimEnd();
                            sentence += ":";
                        }
                    }
                    write.WriteLine(sentence);
                    spaces += 4;
                }
                else if (line == "Else")
                {
                    spaces -= 4;
                    string sentence = "";
                    sentence = put_spaces(spaces);
                    sentence += line.ToLower() + ":";
                    write.WriteLine(sentence);
                    spaces += 4;
                }
                else if (line == "EndIf" || line.StartsWith("EndFunc"))
                {
                    spaces -= 4;
                }
                else if (str_arr[0].StartsWith("MsgBox("))
                {
                    string sentence = "";
                    string val = "";
                    sentence = put_spaces(spaces);
                    sentence += "raise Exception(";
                    str_arr = line.Split(',').ToList();
                    str_arr = str_arr[2].Split(' ').ToList();
                    for (int i = 1; i < str_arr.Count; i++)
                    {
                        if (str_arr[i].Contains('$'))
                        {
                            str_arr[i] = str_arr[i].Remove(0, 1);

                            if (assigned_vars.ContainsKey(str_arr[i]))
                            {
                                val = assigned_vars[str_arr[i]];
                                val = val.Split(' ')[1];
                                sentence += val + " ";
                            }
                            else
                            {
                                sentence += str_arr[i] + " ";
                            }
                        }
                        else
                        {
                            sentence += str_arr[i] + " ";
                        }
                    }
                    sentence = sentence.Remove(sentence.Length - 2);
                    sentence += "')";
                    write.WriteLine(sentence);
                }
                else if (line.StartsWith("ConsoleWrite("))
                {
                    int idx;
                    string sentence = "";
                    sentence = put_spaces(spaces);
                    sentence += "print '";
                    string arguments = line.Split('(', ')')[1];
                    while (arguments.Contains("$"))
                    {
                        idx = arguments.IndexOf("$");
                        arguments = arguments.Remove(idx, 1);
                    }
                    while (arguments.Contains("'"))
                    {
                        idx = arguments.IndexOf("'");
                        arguments = arguments.Remove(idx, 1);
                    }
                    sentence += arguments+"'";
                    write.WriteLine(sentence);
                }
                else if(line.Contains("Return "))
                {
                    string sentence = put_spaces(spaces);
                    line = line.Replace('R', 'r');
                    sentence += line;
                    write.WriteLine(sentence);
                }
            }
        }
        write.Close();
        Console.ReadLine();
    }

    private static string put_spaces(int spaces)
    {
        string sentence = "";
        for (int i = 0; i < spaces; i++)
        {
            sentence += " ";
        }
        return sentence;
    }
    private static string find_type(string type)
    {
        while (type.Length > 0 && type[0] == ' ')
        {
            type = type.Substring(1);
        }
        if (type.Length > 0) {
            if (type[0] == '[')
            {
                type = type.Split('[', ']')[1];
                return "array " + type;
            }
            else
            {
                bool is_type = false;
                is_type = Int32.TryParse(type, out int num);
                if (is_type)
                {
                    return "int " + type;
                }
                is_type = double.TryParse(type, out double doub);
                if (is_type)
                {
                    return "double " + type;
                }
                is_type = float.TryParse(type, out float floa);
                if (is_type)
                {
                    return "float " + type;
                }
                else if(type.Contains('*') || type.Contains('/') || type.Contains('-') || type.Contains('+'))
                {
                    return "number " + type;
                }
                else if (type.Length == 1)
                {
                    return "char " + type;
                }
                else if(type == "@error")
                {
                    return "some sort of error";
                }
                else
                {
                    return "string " + type;
                }
            }
        }
        return "string";
    }
}