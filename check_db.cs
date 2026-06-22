using System;
using System.Data;
using MySqlConnector;

class Program
{
    static void Main()
    {
        string connStr = "server=localhost;uid=root;pwd=anna#123;database=srescrowdev2;Convert Zero Datetime=True";
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            using (var cmd = new MySqlCommand("DESCRIBE AbpUsers", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string field = reader.GetString("Field");
                    if (field == "ProfilePictureId")
                    {
                        Console.WriteLine($"Field: {field}, Type: {reader.GetString("Type")}");
                    }
                }
            }
        }
    }
}
