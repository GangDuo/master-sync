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
                    const string tableName = "suppliers";
                    string sql = String.Format("SELECT * FROM `{0}`;", tableName);
                    var adapter = new MySqlDataAdapter(sql, conn);
                    MySqlCommandBuilder cb = new MySqlCommandBuilder(adapter);
                    //adapter.InsertCommand = cb.GetInsertCommand();
                    //adapter.UpdateCommand = cb.GetUpdateCommand();
                    //adapter.DeleteCommand = cb.GetDeleteCommand();

                    var id = new MySqlParameter("@id", MySqlDbType.UInt64, 0, "id");
                    var code = new MySqlParameter("@code", MySqlDbType.String, 0, "code");
                    var name = new MySqlParameter("@name", MySqlDbType.String, 0, "name");
                    var paymentDate = new MySqlParameter("@payment_date", MySqlDbType.String, 0, "payment_date");

                    //adapter.InsertCommand = new MySqlCommand(@"INSERT INTO suppliers (code, name) VALUES (@code, @name)");
                    //adapter.InsertCommand.Parameters.Add(id);
                    //adapter.InsertCommand.Parameters.Add(name);

                    adapter.UpdateCommand = new MySqlCommand(String.Format(@"UPDATE `{0}` SET `code` = @code, `name` = @name, `payment_date` = @payment_date WHERE (`id` = @id)", tableName));
                    adapter.UpdateCommand.Parameters.Add(code);
                    adapter.UpdateCommand.Parameters.Add(name);
                    adapter.UpdateCommand.Parameters.Add(paymentDate);
                    adapter.UpdateCommand.Parameters.Add(id).SourceVersion = DataRowVersion.Original;

                    var data = new DataSet();
                    adapter.Fill(data, tableName);
                    Console.WriteLine(data.Tables[tableName].Rows.Count);

                    // テーブルの差分を取得
                    var first = source.Select(x => new Supplier()
                    {
                        Code = x.Code,
                        Name = x.Name,
                        Postcode = x.Postcode,
                        Prefecture = x.Prefecture,
                        Address1 = x.Address1,
                        Tel = x.Tel,
                        Fax = x.Fax,
                        PaymentDate = x.PaymentDate,
                    });
                    var second = data.Tables[tableName].AsEnumerable().Select(row => new Supplier()
                    {
                        Code = row["code"].ToString(),
                        Name = row["name"].ToString(),
                        Postcode = row["postal_code"].ToString(),
                        Prefecture = row["prefecture"].ToString(),
                        Address1 = row["address1"].ToString(),
                        Tel = row["tel"].ToString(),
                        Fax = row["fax"].ToString(),
                        PaymentDate = row["payment_date"].ToString(),
                    });
                    // 完全一位を除外
                    var deff = first.Except(second);
                    foreach (var supplier in deff)
                    {
                        Console.WriteLine(@"code: {0}, name: {1}", supplier.Code, supplier.Name);
                        if (second.Where(x => x.Code == supplier.Code).Count() > 0)
                        {
                            // 修正
                            var a = data.Tables[tableName].AsEnumerable().Where(row => row["code"].ToString() == supplier.Code).First();
                            a["name"] = supplier.Name;
                            a["payment_date"] = supplier.PaymentDate;
                        }
                        else
                        {
                            // 新規
                            var row = data.Tables[tableName].NewRow();
                            row["code"] = supplier.Code;
                            row["name"] = supplier.Name;
                            row["postal_code"] = supplier.PostcodeAsNumberOrDbNull;
                            row["prefecture"] = supplier.Prefecture;
                            row["address1"] = supplier.Address1;
                            row["tel"] = supplier.Tel;
                            row["fax"] = supplier.Fax;
                            row["payment_date"] = supplier.PaymentDate;
                            data.Tables[tableName].Rows.Add(row);
                        }
                    }
                    //データベース更新
                    var updatedRowCount = adapter.Update(data, tableName);
                    //データ更新終了をDataTableに伝える
                    data.AcceptChanges();
                    Console.WriteLine("更新された行数: " + updatedRowCount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
