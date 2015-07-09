using System;
using System.Collections.Generic;
using HP.Utt.UttCore;
using HP.Utt.CodeEditorUtils;
using ICSharpCode.SharpDevelop.Editor;
using System.Text;
using System.Xml;
using HP.Utt.UttDialog;
using HP.LR.VuGen.XmlViewer;
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
