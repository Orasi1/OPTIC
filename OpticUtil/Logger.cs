using System;
using System.Diagnostics;
using System.IO;

namespace OpticUtil
{
    public class Logger
    {
        TextWriterTraceListener textWriterTraceListener;
        Stream myFile;
        public Logger(string logFileName)
        {
            if (File.Exists(logFileName))
            {
                myFile = File.OpenWrite(logFileName);
                myFile.Seek(0, SeekOrigin.End);
            }
            else
            {
                myFile = File.Create(logFileName);
            }
            textWriterTraceListener = new TextWriterTraceListener(myFile);
            Trace.Listeners.Add(textWriterTraceListener);
        }

        ~Logger()
        {
            if (textWriterTraceListener != null)
            {
                textWriterTraceListener.Flush();
                textWriterTraceListener.Close();
            }
            if (myFile != null)
            {
                myFile.Close();
            }
        }


        public void LoggerClose()
        {
            if (textWriterTraceListener != null)
            {
                textWriterTraceListener.Flush();
                textWriterTraceListener.Close();
            }
            if(myFile != null)
            {
                myFile.Close();
            }
        }

        public void LoggerInit(TextWriterTraceListener myTextListener, string logFileName)
        {
            Stream myFile = File.Create(logFileName);
            myTextListener = new TextWriterTraceListener(myFile);
            Trace.Listeners.Add(myTextListener);
        }

        public void WriteEntry(string message, string type, string module)
        {
            Trace.WriteLine(
                    string.Format("{0}, {1}, {2}, {3}",
                                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                  type,
                                  module,
                                  message));
        }
    }
}
