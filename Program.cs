﻿using System;
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

            using (var csv = new CsvHelper.CsvReader(input))
            {
                var source = csv.GetRecords<Supplier>().ToList();
                Console.WriteLine(source.Count);
            }
            input.Dispose();
        }
    }
}
