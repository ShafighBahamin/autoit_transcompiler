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
        //dictionary key is the variable name. the value in the dic is the var type + the assigned value
        Dictionary<string, string> assigned_vars = new Dictionary<string, string>();
        string input;
        int spaces = 0;
        List<string> str_arr;
        //this is where you specify where the auto it script exists and where the python script should go.
        System.IO.StreamWriter write = new StreamWriter("", true);
        System.IO.StreamReader file = new StreamReader("");
        while ((input = file.ReadLine()) != null) {
            
            //get rid of the leading empty spaces
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
                //take care of the includes first
                if (str_arr[0] == "#include")
                {
                    //import instead of include -- next steps should be to translate the include libs so that we can import them
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
                    //arguments passed into functions in auto it start with $-I remove those
                    int idx;
                    while (line.Contains("$"))
                    {
                        idx = line.IndexOf("$");
                        line = line.Remove(idx, 1);
                    }
                    str_arr = line.Split(' ').ToList();
                    //indentation
                    spaces += 4;
                    str_arr[0] = "def";
                    write.WriteLine(string.Join(" ", str_arr) + ":");
                }
                else if (str_arr[0] == "Local")
                {
                    //the local vars are processed here
                    string len_arr = "";
                    str_arr[0] = "";
                    //remove the $ sign of the vars
                    if (str_arr.Count > 2)
                    {
                        str_arr[1] = str_arr[1].Substring(1);
                    }
                    else if (str_arr[1].Contains("[") || str_arr.Contains("]"))
                    {
                        //here we want to see if the var is a array
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
                        //get length of the array
                        len_arr = str_arr[1].Split('[', ']')[1];
                        str_arr[1] = str_arr[1].Remove(start, finish - start + 1);
                    }
                    //if the var is assigned to process diferently
                    if (line.Contains("="))
                    {
                        string type = "";
                        //get the type of var
                        for (int i = 0; i < line.Length; i++)
                        {
                            if (line[i] == '=')
                            {
                                type = line;
                                type = type.Substring(i + 1);
                                type = find_type(type);
                            }
                        }
                        //check to see if var is array
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

                        //if the var is assigned to then we need to write it in the python file
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
                        //if the var is not assigned to check to see if var is array and put in the init vars dict
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
                    //python doesnt have a method that creates a unique array so we do that instead of writing the python code
                    string sentence = "";
                    int rand_num;
                    //manuplating the strings so that we can process them more easily
                    string arguments = line.Split('(', ')')[1];
                    string[] args_arr = arguments.Split(' ');
                    args_arr[0] = args_arr[0].Substring(1);
                    args_arr[0] = args_arr[0].Remove(args_arr[0].Length - 1);
                    args_arr[1] = args_arr[1].Remove(args_arr[1].Length - 1);
                    //get the init var because it needs to be stored ahead of time
                    string arr_len = init_vars[args_arr[0]].Split(' ')[1];
                    //get the integers len of arr min max numbers
                    Int32.TryParse(arr_len, out int len);
                    Int32.TryParse(args_arr[1], out int min);
                    Int32.TryParse(args_arr[2], out int max);
                    sentence = args_arr[0] + " = [ ";
                    //create the random array
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
                    //create the correct indentation
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
                    //process if statements
                    string sentence = "";
                    str_arr[0] = str_arr[0].ToLower();
                    sentence = put_spaces(spaces);
                    for (int i = 0; i < str_arr.Count; i++)
                    {
                        //get rid of the $ in front of vars
                        int idx = str_arr[i].IndexOf("$");
                        if (idx >= 0)
                        {
                            str_arr[i] = str_arr[i].Remove(idx, 1);
                        }
                        //see if one of the operators are used
                        if (str_arr[i] == "Or" || str_arr[i] == "Not" || str_arr[i] == "And")
                        {
                            sentence += str_arr[i].ToLower() + " ";
                        }
                        else if (str_arr[i] == "=")
                        {
                            //equal operator is different in auto it so change it
                            sentence += "== ";
                        }
                        else if (str_arr[i] != "Then")
                        {
                            //end of the iof statement 
                            sentence += str_arr[i] + " ";
                        }
                        else
                        {
                            //trim the white spaces
                            sentence = sentence.TrimEnd();
                            sentence += ":";
                        }
                    }
                    write.WriteLine(sentence);
                    //increase the indentation
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
                    //when we get to the end of funcs or if statements we need to decrease the indentation
                    spaces -= 4;
                }
                else if (str_arr[0].StartsWith("MsgBox("))
                {
                    //Im gonna treat message boxes as exceptions
                    string sentence = "";
                    string val = "";
                    int idx;
                    sentence = put_spaces(spaces);
                    sentence += "raise Exception('";
                    str_arr = line.Split(',').ToList();
                    str_arr = str_arr[2].Split(' ').ToList();
                    //we only care about whats inside the paranthesis
                    for (int i = 1; i < str_arr.Count; i++)
                    {
                        while (str_arr[i].Contains("'"))
                        {
                            idx = str_arr[i].IndexOf("'");
                            str_arr[i] = str_arr[i].Remove(idx, 1);
                        }
                        if (str_arr[i].Contains('$'))
                        {
                            str_arr[i] = str_arr[i].Remove(0, 1);
                            //get he var from the init dict
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
                    //finish creating the string and write to python file
                    sentence = sentence.Remove(sentence.Length - 2);
                    sentence += "')";
                    write.WriteLine(sentence);
                }
                else if (line.StartsWith("ConsoleWrite("))
                {
                    //consolewrite --> print
                    int idx;
                    string sentence = "";
                    sentence = put_spaces(spaces);
                    sentence += "print '";
                    //extract whats in between the parans
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
                    //the return is going to need more work but for now return the string as is
                    string sentence = put_spaces(spaces);
                    line = line.Replace('R', 'r');
                    sentence += line;
                    write.WriteLine(sentence);
                }
            }
            else
            {
                write.WriteLine(line);
            }
        }
        write.Close();
    }

    /// <summary>
    /// this func takes care of the indentation of the file
    /// </summary>
    /// <param name="Shafigh"></param>
    /// <returns></returns>
    private static string put_spaces(int spaces)
    {
        string sentence = "";
        for (int i = 0; i < spaces; i++)
        {
            sentence += " ";
        }
        return sentence;
    }

    /// <summary>
    /// thsi func takes care of finding the data type for vars
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
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