//Code mostly by tmch :)

using System;
using System.IO;

namespace Darkly.GDTMP
{
    internal class CommandMacro
    {
        public void RunByFile(string pedalfilename)
        {
            string macroname = Path.GetFileNameWithoutExtension(pedalfilename);
            try
            {
                StreamReader durr = new StreamReader(pedalfilename);
                ServerObject moo = new ServerObject(null);

                string kitten;
                while ((kitten = durr.ReadLine()) != null) //lol wiki c&p.. C&P. NOT CP. YOU PERVERT.
                    if (kitten.Length > 0 && kitten[0] != ';')
                        moo.runCommand(kitten);

                durr.Close();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                dmultiplayersrv.PrintError("MACRO \"" + macroname + "\" ERROR: " + ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}