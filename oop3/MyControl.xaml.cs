using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
// New
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System.IO;


namespace japroc_company.oop3
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        public MyControl()
        {
            InitializeComponent();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]


        private string result = "";
        String[] fileContent;

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            /*EnvDTE80.DTE2 dte2;
            dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.
            GetActiveObject("VisualStudio.DTE.12.0");*/

            // Get DTE
            EnvDTE.DTE dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));

            // Get Full Path to Document
            String DocPath = dte.ActiveDocument.FullName;

            if (DocPath.EndsWith(".py"))
            {
                PythonParser PP = new PythonParser();
                TextBox1.Text = PP.getResult(DocPath);
                return;
            }

            // Get File Content
            fileContent = File.ReadAllLines(System.IO.Path.GetFullPath(dte.ActiveDocument.FullName));

            // Get File Code Model
            FileCodeModel fcm = dte.ActiveDocument.ProjectItem.FileCodeModel;

            // Clear result
            result = "";

            int counter = 0;
            int start = 0;
            int end = 0;
            for (int i = 0; i < fileContent.Length; ++i)
            {
                for (int j = 0; j < fileContent[i].Length; ++j)
                {
                    if (fileContent[i][j] == '{')
                    {
                        if (counter == 0)
                        {
                            start = i;
                        }
                        counter++;
                    }
                    else if (fileContent[i][j] == '}')
                    {
                        if (counter == 1)
                        {
                            end = i;

                            int start_line = start;
                            int end_line = end + 1;
                            int lines_Empty = 0;
                            int lines_Comments = 0;
                            int amount_key_words = 0;
                            String test = "";


                            String func_name = "";
                            int line_func_name;
                            line_func_name = start_line - ((fileContent[start_line].StartsWith("{") == true) ? 1 : 0);
                            for (j = 0; j < fileContent[line_func_name].Length; j++ )
                            {
                                char c = fileContent[line_func_name][j];
                                if (c == '{')
                                    break;

                                func_name += c;
                            }


                            Function_Handler(line_func_name, end_line, ref lines_Empty, ref lines_Comments, ref amount_key_words, ref test);

                            result += func_name + Environment.NewLine +
                                "Total Lines: " + (end_line - line_func_name).ToString() + Environment.NewLine +
                                "Empty Lines: " + lines_Empty.ToString() + Environment.NewLine +
                                "Comment Lines: " + lines_Comments.ToString() + Environment.NewLine +
                                "Key Words: " + amount_key_words.ToString() + Environment.NewLine +
                                //"Test: " + test + Environment.NewLine +
                                Environment.NewLine;

                        }
                        if (counter > 0) counter--;
                    }
                }
            }


            // Get and handle only functions
            /*foreach (CodeElement elem in fcm.CodeElements)
            {
                switch (elem.Kind)
                {
                    case EnvDTE.vsCMElement.vsCMElementFunction:
                        {
                            int start_line = elem.StartPoint.Line;
                            int end_line = elem.EndPoint.Line;
                            int lines_Empty = 0;
                            int lines_Comments = 0;
                            int amount_key_words = 0;
                            String test = "";

                            Function_Handler(start_line + 1, end_line - 1, ref lines_Empty, ref lines_Comments, ref amount_key_words, ref test);

                            result += fileContent[start_line - 1] + Environment.NewLine +
                                "Total Lines: " + (end_line - start_line + 1).ToString() + Environment.NewLine +
                                "Empty Lines: " + lines_Empty.ToString() + Environment.NewLine +
                                "Comment Lines: " + lines_Comments.ToString() + Environment.NewLine +
                                "Key Words: " + amount_key_words.ToString() + Environment.NewLine +
                                //"Test: " + test + Environment.NewLine +
                                Environment.NewLine;

                            break;

                        }
                    default: break;
                }
            }*/

            TextBox1.Text = result;
        }

        private void Function_Handler(
            int start_line,
            int end_line,
            ref int lines_Empty,
            ref int line_Comments,
            ref int amount_key_words,
            ref String test_string)
        {
            if (start_line >= end_line)
                return;

            // Calculate Empty Lines
            for (int i = start_line; i < end_line; i++)
            {
                if (fileContent[i].Length == 0)
                {
                    lines_Empty++;
                }
            }

            // Searching for const strings
            // Clear_Const_Strings(start_line, end_line, ref test_string);

            // Searching for everything
            bool is_string = false;
            bool is_comment = false;
            bool is_bigcomment = false;
            for (int i = start_line; i < end_line; i++)
            {
                StringBuilder sb = new StringBuilder(fileContent[i]);

                bool is_line_commented = is_comment || is_bigcomment;

                if (is_comment == true)
                {
                    for (int j = 0; j < sb.Length; ++j )
                    {
                        sb[j] = 'A';
                    }

                    if (fileContent[i].Length > 0 && fileContent[i][fileContent[i].Length - 1] == '\\')
                    {}
                    else
                    {
                        is_comment = false;

                        // uncomment not to calculate comments lines if they are empty
                        // continue; 
                    }

                }
                for (int j = 0; j < fileContent[i].Length; ++j)
                {
                    if (is_string == true)
                    {
                        if (fileContent[i][j] == '\"' && j - 1 >= 0 && fileContent[i][j - 1] != '\\')
                        {
                            is_string = false;
                            continue;
                        }
                        sb[j] = 'A';
                    }
                    else if (is_bigcomment == true)
                    {
                        if (fileContent[i][j] == '*' && j + 1 < fileContent[i].Length && fileContent[i][j + 1] == '/')
                        {
                            is_bigcomment = false;
                            continue;
                        }
                        sb[j] = 'A';
                    }
                    else
                    {
                        if (fileContent[i][j] == '\"')
                        {
                            is_string = true;
                        }
                        else if (fileContent[i][j] == '/' && j + 1 < fileContent[i].Length && fileContent[i][j + 1] == '/')
                        {
                            is_line_commented = true;

                            if (fileContent[i][fileContent[i].Length - 1] == '\\')
                            {
                                is_comment = true;
                            }

                            for (; j < sb.Length; ++j)
                            {
                                sb[j] = 'A';
                            }

                            break;
                        }
                        else if (fileContent[i][j] == '/' && j + 1 < fileContent[i].Length && fileContent[i][j + 1] == '*')
                        {
                            is_bigcomment = true;
                            is_line_commented = true;
                        }
                    }
                }
                if (is_line_commented)
                    line_Comments++;

                fileContent[i] = sb.ToString();
            }


            //----------------------
            // Calculating key words
            Calculate_Key_Words(start_line, end_line, ref amount_key_words);
        }

        private void Calculate_Key_Words(int start_line, int end_line, ref int amount_key_words)
        {
            String[] arr = { 
                           "alignas", "alignof", "and", "and_eq", "asm", "auto", "bitand", 
                           "bitor", "bool", "break", "case", "catch", "char", "char16_t", 
                           "char32_t", "class", "compl", "const", "constexpr", "const_cast", 
                           "continue", "decltype", "default", "delete", "do", "double", "dynamic_cast", 
                           "else", "enum", "explicit", "export", "extern", "false", "float", "for", 
                           "friend", "goto", "if", "inline", "int", "long", "mutable", "namespace", 
                           "new", "noexcept", "not", "not_eq", "nullptr", "operator", "or", "or_eq", 
                           "private", "protected", "public", "register", "reinterpret_cast", "return", 
                           "short", "signed", "sizeof", "static", "static_assert", "static_cast", 
                           "struct", "switch", "template", "this", "thread_local", "throw", "true", 
                           "try", "typedef", "typeid", "typename", "union", "unsigned", "using", 
                           "virtual", "void", "volatile", "wchar_t", "while", "xor", "xor_eq"
                           };


            for (int i = start_line; i < end_line; i++)
            {
                foreach (String key in arr)
                {
                    //StringBuilder sb = new StringBuilder(fileContent[i]);
                    String str = fileContent[i];
                    while (true)
                    {

                        int idx = str.IndexOf(key);
                        if (idx == -1)
                        {
                            break;
                        }

                        if (idx - 1 >= 0 && ((str[idx - 1] >= 'a' && str[idx - 1] <= 'z') || (str[idx - 1] >= '0' && str[idx - 1] <= '9')))
                        {
                            str = str.Substring(idx + key.Length);
                            continue;
                        }

                        int idx_plus = idx + key.Length;
                        if (idx_plus < str.Length && ((str[idx_plus] >= 'a' && str[idx_plus] <= 'z') || (str[idx_plus] >= '0' && str[idx_plus] <= '9')))
                        {
                            str = str.Substring(idx + key.Length);
                            continue;
                        }

                        str = str.Substring(idx + key.Length);
                        amount_key_words++;
                    }
                }
            }
        }
    }

    class PythonParser
    {
        private String[] fileContent;
        private String result;
        public string getResult(String DocPath)
        {
            fileContent = File.ReadAllLines(System.IO.Path.GetFullPath(DocPath));
            int start = -1;
            int end = 0;



            while (true)
            {
                start = find_function(start + 1);
                if (start != -1)
                {
                    end = find_function(start + 1);
                    if (end == -1) end = fileContent.Length - 1;
                    else end--;

                    end = find_func_end(start, end);
                    // logically end always not -1

                    int comments = 0;
                    int empty = 0;
                    int key_words = 0;
                    String test = "";
                    Function_Handler(start + 1, end, ref comments, ref empty);
                    Calculate_Key_Words(start + 1, end, ref key_words, ref test);

                    result += fileContent[start] + Environment.NewLine +
                        "Total Lines : " + (end - start).ToString() + Environment.NewLine +
                        "Empty Lines : " + empty.ToString() + Environment.NewLine +
                        "Comment Lines : " + comments.ToString() + Environment.NewLine +
                        "Key Words : " + key_words.ToString() + Environment.NewLine +
                        "Test : " + test + Environment.NewLine +
                        Environment.NewLine;

                    continue;
                }

                return result;
            }
        }

        private void Function_Handler(int start, int end, ref int comments, ref int empty)
        {

            int is_bigcomment = -1;

            for (int i = start; i <= end; i++)
            {
                if (is_empty_line(fileContent[i]))
                    empty++;

                for (int j = 0; j < fileContent[i].Length; ++j)
                {
                    if (is_bigcomment > 0)
                    {
                        if (j + 2 < fileContent[i].Length && fileContent[i][j] == '\"' &&
                            fileContent[i][j + 1] == '\"' && fileContent[i][j + 2] == '\"')
                        {
                            comments += i - is_bigcomment + 1;
                            is_bigcomment = -1;
                        }
                    }
                    else
                    {
                        if (fileContent[i][j] == '#')
                        {
                            comments++;
                            break;
                        }
                        else if (j + 2 < fileContent[i].Length && fileContent[i][j] == '\"' &&
                            fileContent[i][j + 1] == '\"' && fileContent[i][j + 2] == '\"')
                        {
                            j += 2;
                            is_bigcomment = i;
                        }
                    }
                }
            }
        }
        private void Calculate_Key_Words(int start_line, int end_line, ref int amount_key_words, ref String test)
        {
            String[] arr = { 
                           "and", "del", "from", "not", "while", "as", 
                           "elif", "global", "or", "with", "assert", 
                           "else", "if", "pass", "yield", "break", 
                           "except", "import", "print", "class", "exec", 
                           "in", "raise", "continue", "finally", "is", 
                           "return", "def", "for", "lambda", "try"};


            for (int i = start_line; i <= end_line; i++)
            {
                foreach (String key in arr)
                {
                    String str = fileContent[i];
                    while (true)
                    {

                        int idx = str.IndexOf(key);
                        if (idx == -1)
                        {
                            break;
                        }
                        str = str.Substring(idx + key.Length);
                        if (idx - 1 >= 0 && str[idx - 1] >= 'a' && str[idx - 1] <= 'z')
                            continue;
                        int idx_plus = idx + key.Length;
                        if (idx_plus < str.Length && str[idx_plus] >= 'a' && str[idx_plus] <= 'z')
                            continue;
                        amount_key_words++;
                        test += key + ", ";

                    }
                }
            }


        }

        private bool is_empty_line(String str)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] != '\t' && str[i] != ' ')
                {
                    return false;
                }
            }
            return true;
        }

        private int find_function(int start)
        {
            for (int i = start; i < fileContent.Length; ++i)
            {
                if (fileContent[i].StartsWith("def "))
                {
                    return i;
                }
            }
            return -1;
        }

        private int find_func_end(int foo1, int foo2)
        {
            for (int i = foo2; i > foo1; --i)
            {
                if (!is_empty_line(fileContent[i]))
                    return i;
            }
            return -1;
        }
    }
}