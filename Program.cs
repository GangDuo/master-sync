using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
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

            var builder = new MySqlConnectionStringBuilder();
            builder.Server = ConfigurationManager.AppSettings["db:server"];
            builder.Port = uint.Parse(ConfigurationManager.AppSettings["db:port"]);
            builder.UserID = ConfigurationManager.AppSettings["db:user"];
            builder.Password = ConfigurationManager.AppSettings["db:password"];
            builder.Database = ConfigurationManager.AppSettings["db:database"];
            builder.ConvertZeroDateTime = true;
            builder.AllowZeroDateTime = true;
            using (var conn = new MySqlConnection(builder.ToString()))
            {
                try
                {
                    Console.WriteLine("Connecting to MySQL...");
                    conn.Open();
                    // Perform database operations
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            Console.WriteLine("Done.");
        }
    }
}
