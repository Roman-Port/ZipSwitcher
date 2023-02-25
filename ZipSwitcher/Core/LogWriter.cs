using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZipSwitcher.Core
{
    class LogWriter : IDisposable
    {
        public LogWriter(string filename, string[] columnHeaders)
        {
            if (File.Exists(filename))
            {
                //Open in append mode
                file = new FileStream(filename, FileMode.Append, FileAccess.Write);
                writer = new StreamWriter(file);
            } else
            {
                //Open in create mode
                file = new FileStream(filename, FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(file);

                //Write headers
                WriteLine(columnHeaders);
            }
        }

        private readonly FileStream file;
        private readonly StreamWriter writer;

        public void WriteLine(object[] columns)
        {
            //Write each item
            for (int i = 0; i < columns.Length; i++)
            {
                //Write item
                if (columns[i].GetType() == typeof(int) || columns[i].GetType() == typeof(long))
                {
                    //Write number, no double quotes needed
                    writer.Write(columns[i].ToString());
                }
                else if (columns[i].GetType() == typeof(DateTime))
                {
                    //Format date
                    DateTime value = (DateTime)columns[i];
                    writer.Write("\"" + value.ToString("M/d/yy HH:mm:ss") + "\"");
                } else
                {
                    //Fallback to string
                    string value = columns[i].ToString();
                    if (value.Contains('"'))
                        throw new NotImplementedException("Double quotes are not implimented in log writer.");

                    //Write
                    writer.Write("\"" + value + "\"");
                }

                //If this isn't the last, add comma
                if (i != columns.Length - 1)
                    writer.Write(",");
            }

            //Finalize line
            writer.WriteLine();
            writer.Flush();
        }

        public void Dispose()
        {
            writer.Dispose();
            file.Dispose();
        }
    }
}
