using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master.Sync
{
    class Program
    {
        static void Main(string[] args)
        {
            TextReader input;

            if (args.Length == 0)
            {
                input = Console.In;
            }
            else
            {
                input = new StreamReader(args[0], Encoding.GetEncoding("Shift_JIS"));
            }
            Cat(input);
            input.Dispose();
        }

        static void Cat(TextReader tr)
        {
            string line;
            int num = 1;

            while ((line = tr.ReadLine()) != null)
            {
                Console.WriteLine("{0, 10} : {1}", num, line);
                num++;
            }
        }
    }
}
