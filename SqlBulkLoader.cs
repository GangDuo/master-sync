using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master.Sync
{
    class SqlBulkLoader
    {
        private static readonly string ConnectionString = new MySqlConnectionStringBuilder()
        {
            Server = ConfigurationManager.AppSettings["db:server"],
            Port = uint.Parse(ConfigurationManager.AppSettings["db:port"]),
            UserID = ConfigurationManager.AppSettings["db:user"],
            Password = ConfigurationManager.AppSettings["db:password"],
            Database = ConfigurationManager.AppSettings["db:database"],
            ConvertZeroDateTime = true,
        }.ToString();

        public static void Load(List<Supplier> source)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                try
                {
                    Console.WriteLine("Connecting to MySQL...");
                    conn.Open();
                    // Perform database operations
                    string sql = "SELECT * FROM suppliers;";
                    var adapter = new MySqlDataAdapter(sql, conn);
                    MySqlCommandBuilder cb = new MySqlCommandBuilder(adapter);

                    var data = new DataSet();
                    adapter.Fill(data, "suppliers");
                    Console.WriteLine(data.Tables["suppliers"].Rows.Count);

                    // テーブルの差分を取得
                    var first = source.Select(x => new Supplier()
                    {
                        Code = x.Code,
                        Name = x.Name
                    });
                    var second = data.Tables["suppliers"].AsEnumerable().Select(row => new Supplier()
                    {
                        Code = row["code"].ToString(),
                        Name = row["name"].ToString(),
                        //Postcode = row[""].ToString(),
                        //Prefecture = row[""].ToString(),
                        //Address1 = row[""].ToString(),
                        //Tel = row[""].ToString(),
                        //Fax = row[""].ToString()

                    });
                    // 完全一位を除外
                    var deff = first.Except(second);
                    foreach (var supplier in deff)
                    {
                        if (second.Where(x => x.Code == supplier.Code).Count() > 0)
                        {
                            // 修正
                            var a = data.Tables["suppliers"].AsEnumerable().Where(row => row["code"].ToString() == supplier.Code).First();
                            a["name"] = supplier.Name;
                        }
                        else
                        {
                            // 新規
                            var row = data.Tables["suppliers"].NewRow();
                            row["code"] = supplier.Code;
                            row["name"] = supplier.Name;
                            data.Tables["suppliers"].Rows.Add(row);
                        }
                    }
                    //データベース更新
                    var updatedRowCount = adapter.Update(data, "suppliers");
                    //データ更新終了をDataTableに伝える
                    data.AcceptChanges();
                    Debug.WriteLine("更新された行数: " + updatedRowCount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
