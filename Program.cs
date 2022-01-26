using Npgsql;
using System;

namespace uidlookup
{
    class Program
    {
        private static string csHost = string.Empty;
        private static int csPort = 0;
        private static string csDB = string.Empty;
        private static string csUserID = string.Empty;
        private static string csPass = string.Empty;
        private static string uid = string.Empty;

        static void Main(string[] args)
        {
            // error trap and exit
            if (args.Length != 12)
            {
                Console.Out.Write("ERROR: Incorrect args lenght provided");
                Environment.Exit(exitCode: 1);
            }

            // passed and now read args to vars
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].Trim().ToLowerInvariant())
                {
                    case "--cshost":
                        csHost = args[i + 1];
                        break;
                    case "--csport":
                        csPort = Int32.Parse(args[i + 1]);
                        break;
                    case "--csdb":
                        csDB = args[i + 1];
                        break;
                    case "--csuserid":
                       csUserID = args[i + 1];
                        break;
                    case "--cspass":
                        csPass = args[i + 1];
                        break;
                    case "--uid":
                        uid = args[i + 1];
                        break;
                }
            }

            // main function
            NpgsqlConnectionStringBuilder cs = NewCS(dbHost: csHost, dbPort: csPort, dbDB: csDB, dbUserID: csUserID, dbPass: csPass);
            int output = UIDMatch(cs: cs.ToString(), tableName: "tbl_appdata", UIDValue: uid);

            // value out to return
            Console.Out.Write(output.ToString());
        }

        // build connection string secretly
        private static NpgsqlConnectionStringBuilder NewCS(string dbHost, int dbPort, string dbDB, string dbUserID, string dbPass)
        {
            NpgsqlConnectionStringBuilder BuiltString = new NpgsqlConnectionStringBuilder
            {
                Host = dbHost,
                Port = dbPort,
                Database = dbDB,
                Username = dbUserID,
                Password = dbPass,
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return BuiltString;
        }

        // lookup UID and return >=1 found, <1 not found
        private static int UIDMatch(string cs, string tableName, string UIDValue)
        {
            int returnValue = 0;
            using (var con = new NpgsqlConnection(cs))
            {
                con.Open();
                try
                {
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = $"SELECT uid FROM {tableName} WHERE (uid = '{UIDValue}');";
                        using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                if (rdr.GetString(0).Length > 0)
                                {
                                    returnValue++;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("ERROR: {0}", e.Message.ToString());
                    return 0;
                }
            }
            return returnValue;
        }
    }
}
