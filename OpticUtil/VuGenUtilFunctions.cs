﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OpticUtil
{
    public static class VuGenUtilFunctions
    {
        public static int ProcessTransText(string[] rgLines, ref StringBuilder newLines)
        {
            int newLineCount = 0;
            int transFoundCount = 0;

            const string lrScriptTransFunctionIdString = "lr_start_transaction(";
            const string lrScriptAddHeaderFunctionName = "web_add_header";
            const string lrScriptHeaderTransIdString = "AppDHeader";
            const string lrScriptComment = "//OPTIC: Adds a LoadRunner header that AppDynamics will recognize.";

            //Used to contain new file contents
            newLines = new StringBuilder();

            for (int i = 0; i < rgLines.Length; i++)
            {
                //Store the current line
                string line = rgLines[i];

                //Add a line to the new file for each line of the old file
                newLines.AppendLine(line);

                //Store the position of the transaction if there is one
                int pos = line.IndexOf(lrScriptTransFunctionIdString, StringComparison.CurrentCultureIgnoreCase);

                //If found get value of transaction name and create new line
                if (pos > 0)
                {
                    transFoundCount++;
                    //Find the line prefix spaces or tab to use for the new line of code
                    string linePrefix = string.Empty;
                    int prefixPos = 0;
                    while (true)
                    {
                        //Get next character of line prefix
                        string prefixString = line.Substring(prefixPos++, 1);

                        //If it contains a space or tab store and continue
                        if (prefixString == " " || prefixString == "\t")
                        {
                            //add prefix character
                            linePrefix += prefixString;
                        }
                        else
                        {
                            //exit while loop
                            break;
                        }
                    }

                    //Increment the postion to transaction name string
                    pos = pos + lrScriptTransFunctionIdString.Length;

                    //Find the end of the function
                    int endPos = line.IndexOf(")", pos);

                    //Store the transaction name
                    string transName = line.Substring(pos, endPos - pos);

                    //Format a new line to add to the new file
                    string newLine = string.Format("{0}{1}(\"{2}\", {3});", linePrefix, lrScriptAddHeaderFunctionName, lrScriptHeaderTransIdString, transName);

                    //Determine if the code has already been added
                    bool found = false;
                    for (int j = i + 1; j < rgLines.Length; j++)
                    {
                        //Check for exact line we are about to add, ignoring case
                        if (rgLines[j].ToLower() == newLine.ToLower())
                        {
                            //Already contains exact line so break out
                            found = true;
                            break;
                        }

                        //If we get to a new transaction, stop looking
                        if (rgLines[j].IndexOf(lrScriptTransFunctionIdString, StringComparison.CurrentCultureIgnoreCase) > 0)
                        {
                            break;
                        }
                    }

                    if (!found)
                    {
                        //Add a new line, a comment, and the code line to the new file
                        newLines.AppendLine("");
                        newLines.AppendLine(string.Format("{0}{1}", linePrefix, lrScriptComment));
                        newLines.AppendLine(newLine);
                        newLineCount++;
                    }
                }
            }

            Console.WriteLine("Transactions found: {0}", transFoundCount);
            Console.WriteLine("New Lines added:    {0}", newLineCount);
            Console.WriteLine();

            return newLineCount;
        }

        public static void ProcessDirectory(string dir)
        {
            //For every file match in directory process the file
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir, "*.c", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    Console.WriteLine(file);
                    ProcessFile(file);
                }
            }
        }

        public static void ProcessFile(string file)
        {
            string[] rgLines = File.ReadAllLines(file);

            StringBuilder newLines = null;
            int newLineCount = ProcessTransText(rgLines, ref newLines);

            //If lines were added to file then create backup and rewrite new file
            if (newLineCount > 0)
            {
                //Create backup
                string backupfileName = string.Format("{0}.bak", file);
                File.WriteAllLines(backupfileName, rgLines);

                //Overwrite existing file with new contents
                File.WriteAllText(file, newLines.ToString());
            }

        }

    }

}
