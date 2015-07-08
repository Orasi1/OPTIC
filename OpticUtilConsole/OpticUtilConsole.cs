using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpticUtil;
using System.IO;

namespace PerformanceCounterUtilityConsole
{
    class OpticUtilConsole
    {
        static void Main(string[] args)
        {
            string dir = args[0];
            LRTransHeader(dir);
        }

        private static void LRTransHeader(string dir)
        {
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir, "*.c", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    Console.WriteLine(file);
                    int transCount = ProcessTransFile(file);
                }
            }
        }

        private static int ProcessTransFile(string file)
        {
            int newLineCount = 0;
            int transFoundCount = 0;

            string lrCodeTransFunctionIdString = "lr_start_transaction(";
            string lrCodeAddHeaderFunctionName = "web_add_header";
            string lrHeaderTransIdString = "AppDHeader";
            string lrCodeComment = "//OPTIC: This adds a LoadRunner header that AppDynamics will recognize.";
            
            string[] lines = File.ReadAllLines(file);
            List<string> newLines = new List<string>();

            for(int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                
                //Add a line to the new file for each line of the old file
                newLines.Add(line);
                
                //Store the position of the transaction if there is one
                int pos = line.IndexOf(lrCodeTransFunctionIdString);
                if(pos > 0)
                {
                    transFoundCount++;
                    //Find the line prefix spaces or tab to use for the new line of code
                    string linePrefix = string.Empty;
                    int prefixPos = 0;
                    while(true)
                    {
                        string prefixString = line.Substring(prefixPos++, 1);
                        if(prefixString == " " || prefixString == "\t")
                        {
                            linePrefix += prefixString;
                        }
                        else
                        {
                            break;
                        }
                    }
                
                    //Increment the postion to transaction name string
                    pos = pos + lrCodeTransFunctionIdString.Length;
                    
                    //Find the end of the function
                    int endPos = line.IndexOf(")", pos);
                    
                    //Store the transaction name
                    string transName = line.Substring(pos, endPos - pos);

                    //Format a new line to add to the new file
                    string newLine = string.Format("{0}{1}(\"{2}\", {3});", linePrefix, lrCodeAddHeaderFunctionName, lrHeaderTransIdString, transName);

                    //Determine if the code has already been added
                    bool found = false;
                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        //Check for exact line we are about to add
                        if(lines[j] == newLine)
                        {
                            found = true;
                            break;
                        }

                        //If we get to a new transaction, stop looking
                        if(lines[j].IndexOf(lrCodeTransFunctionIdString) > 0)
                        {
                            break;
                        }
                    }

                    if (!found)
                    {
                        //Add a new line, a comment, and the code line to the new file
                        newLines.Add("");
                        newLines.Add(string.Format("{0}{1}", linePrefix, lrCodeComment));
                        newLines.Add(newLine);
                        newLineCount++;
                        //Console.WriteLine("\t" + line);
                        //Console.WriteLine("\t" + newLine);
                    }
                }
            }

            Console.WriteLine("Trans found count: {0}", transFoundCount);
            Console.WriteLine("New Line count: {0}", newLineCount);
            Console.WriteLine();
            if(newLineCount > 0)
            {
                string backupfileName = string.Format("{0}.bak", file);
                if(File.Exists(backupfileName))
                {
                    File.Delete(backupfileName);
                }
                File.WriteAllLines(backupfileName, lines);
                File.WriteAllLines(file, newLines);
            }
            return newLineCount;
        }

        private static void WriteFile(string file, List<string> lines, string suffix)
        {
            string newfileName = string.Format("{0}_new.txt", file);
            File.WriteAllLines(newfileName, lines);
        }

        private static void TestCounters()
        {
            TestCounters();
            string myCategory = "Prudent Counters";
            string myInstance;

            Counter.DeleteCounterCategory(myCategory);

            myInstance = "Instance1";
            Counter.ResetCounter(string.Format("{0}({1})", myCategory, myInstance), 0);
            Counter.IncrementCounter(string.Format("{0}({1})", myCategory, myInstance), 1);

            myInstance = "Instance2";
            Counter.IncrementCounter(string.Format("{0}({1})", myCategory, myInstance), 2);

            myInstance = "Instance3";
            Counter.IncrementCounter(string.Format("{0}({1})", myCategory, myInstance), 3);
        }
    }
}
