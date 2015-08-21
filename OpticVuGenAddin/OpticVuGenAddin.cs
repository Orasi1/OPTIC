using System;
using System.Collections.Generic;
using HP.Utt.UttCore;
using HP.Utt.CodeEditorUtils;
using ICSharpCode.SharpDevelop.Editor;
using System.Text;
using System.Xml;
using HP.Utt.UttDialog;
//using HP.LR.VuGen.XmlViewer;
using System.Xml.Linq;
using ICSharpCode.Core;
using OpticUtil;

//https://github.com/BorisKozo/XmlViewAddin
//http://h30499.www3.hp.com/t5/LoadRunner-Information-and-News/VuGen-Extensibility-Example-Do-more-with-VuGen-11-5X/td-p/6065567#.VYr_4vlVhBc

namespace OpticVuGenAddin
{
    public class AddTransactionWebHeaderSelection : UttBaseWpfCommand
    {
        public static bool IsValidCode(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return false;
            }

            return true;
        }

        public static string GetSelectedString()
        {
            ITextEditor editor = UttCodeEditor.GetActiveTextEditor();
            if (editor == null)
                return null;
            return editor.SelectedText;
        }

        public override void Run()
        {
            string code = GetSelectedString();
            if (IsValidCode(code))
            {
                InsertOpticTransactionHeader(code);
            }
        }

        private void InsertOpticTransactionHeader(string code)
        {
            ITextEditor editor = UttCodeEditor.GetActiveTextEditor(); 

            string[] rgLines = editor.SelectedText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            StringBuilder newLines = null;
            int newLineCount = VuGenUtilFunctions.ProcessTransText(rgLines, ref newLines);
            if (newLineCount > 0)
            {
                editor.Document.Replace(editor.SelectionStart, editor.SelectionLength, newLines.ToString());
            }
        }
    }

    public class SharedFunctions
    {

        internal static void InsertLines(string[] lines)
        {
            ITextEditor editor = UttCodeEditor.GetActiveTextEditor();

            StringBuilder newLines = new StringBuilder();
            string linePrefix =
                string.IsNullOrEmpty(editor.SelectedText) ?
                "\t" :
                VuGenUtilFunctions.GetLinePrefixSpacesTabs(editor.SelectedText);
            string lineSuffix = string.Empty;

            newLines.AppendLine(editor.SelectedText);
            //newLines.AppendLine("");
            for (int i = 0; i < lines.Length; i++)
            {
                newLines.Append(linePrefix);
                newLines.Append(lines[i]);
                lineSuffix = (i == (lines.Length - 1)) ? string.Empty : System.Environment.NewLine;
                newLines.Append(lineSuffix);
            }
            editor.Document.Replace(editor.SelectionStart, editor.SelectionLength, newLines.ToString());
        }
    }

    public class AddInitialization : UttBaseWpfCommand
    {
        public override void Run()
        {
            SharedFunctions.InsertLines(
                new string[]
                {
                    "//Initialize Optic",
                    "lr_load_dll(\"Optic.dll\");"
                });
        }
    }

    public class AddVUserCount : UttBaseWpfCommand
    {
        public override void Run()
        {
            SharedFunctions.InsertLines(
                new string[]
                {
                    "//Increment VUsers",
                    "IncrementCounter(\"LoadRunner(VUsers)\\\\Count\", 1);"
                });
        }
    }

    public class GetVUserCount : UttBaseWpfCommand
    {
        public override void Run()
        {
            SharedFunctions.InsertLines(
                new string[]
                {
                    "//Get a performance counter",
                    "int counterValue = 0;",
                    "counterValue = GetCounter(\"LoadRunner(VUsers)\\\\Count\");",
 	                "lr_log_message(\"LoadRunner(VUsers)\\Count: %d\", counterValue);"
                });
        }
    }

    public class GetCPUCounter : UttBaseWpfCommand
    {
        public override void Run()
        {
            SharedFunctions.InsertLines(
                new string[]
                {
                    "//Get a performance counter",
                    "int counterValue = 0;",
                    "counterValue = GetCounter(\"Processor Information(_Total)\\\\% Processor Time\");",
 	                "lr_log_message(\"Processor Information(_Total)\\% Processor Time: %d\", counterValue);"
                });
        }
    }

    public class ResetVUserCount : UttBaseWpfCommand
    {
        public override void Run()
        {
            SharedFunctions.InsertLines(
                new string[]
                {
                    "//Reset a performance counter",
                    "ResetCounter(\"LoadRunner(VUsers)\\\\Count\", 0);"
                });
        }
    }

    public class AddAppDynamicsCustomEvent : UttBaseWpfCommand
    {
        public override void Run()
        {
            SharedFunctions.InsertLines(
                new string[]
                {
	                "CreateCustomEvent(",
		                "\"https://controller URL\",",
		                "\"your_username>@customer1:your_password\",",
		                "\"Application Name\",",
		                "\"INFO\",",
		                "\"OPTIC\",",
		                "\"Load Test - Start\");"
                });
        }
    }

    public class AddTransactionWebHeaderFile : UttBaseWpfCommand
    {
        public static bool IsValidCode(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return false;
            }

            return true;
        }

        public static string GetSelectedString()
        {
            ITextEditor editor = UttCodeEditor.GetActiveTextEditor();
            if (editor == null)
                return null;
            return editor.Document.Text.ToString();
        }

        public override void Run()
        {
            string code = GetSelectedString();
            if (IsValidCode(code))
            {
                InsertOpticTransactionHeader(code);
            }
        }

        private void InsertOpticTransactionHeader(string code)
        {
            ITextEditor editor = UttCodeEditor.GetActiveTextEditor();

            //string[] rgLines = editor.SelectedText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            string[] rgLines = editor.Document.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            StringBuilder newLines = null;
            int newLineCount = VuGenUtilFunctions.ProcessTransText(rgLines, ref newLines);
            if (newLineCount > 0)
            {
                editor.Document.Replace(0, editor.Document.TextLength, newLines.ToString());
            }
        }
    }

    public class AddTransactionWebHeaderProject : UttBaseWpfCommand
    {
        public static bool IsValidCode(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return false;
            }

            return true;
        }

        public override void Run()
        {
            ITextEditor editor = UttCodeEditor.GetActiveTextEditor();

            //Get the file name and extract the directory from it
            string fileName = editor.FileName.ToString();
            int pos = fileName.LastIndexOf("\\");
            
            if (pos > 0)
            {
                string directory = fileName.Substring(0, pos);
                VuGenUtilFunctions.ProcessDirectory(directory);
            }
        }
    }

    public class IsValidCodeSelectedCondition : IConditionEvaluator
    {
        public bool IsValid(object owner, Condition condition)
        {
            return AddTransactionWebHeaderSelection.IsValidCode(AddTransactionWebHeaderSelection.GetSelectedString());
        }
    }

    public class IsNotValidCodeSelectedCondition : IConditionEvaluator
    {
        public bool IsValid(object owner, Condition condition)
        {
            return !AddTransactionWebHeaderSelection.IsValidCode(AddTransactionWebHeaderSelection.GetSelectedString());
        }
    }
}
