using Npgsql;
using System;

namespace uidlookup
{
    class Program
    {
        private static string _dbHost = string.Empty;
        private static int _dbPort = 0;
        private static string _dbDB = string.Empty;
        private static string _dbUserID = string.Empty;
        private static string _dbPass = string.Empty;
        private static string _uid = string.Empty;

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
                    case "--dbhost":
                        _dbHost = args[i + 1];
                        break;
                    case "--dbport":
                        _dbPort = Int32.Parse(args[i + 1]);
                        break;
                    case "--dbdb":
                        _dbDB = args[i + 1];
                        break;
                    case "--dbuserid":
                       _dbUserID = args[i + 1];
                        break;
                    case "--dbpass":
                        _dbPass = args[i + 1];
                        break;
                    case "--uid":
                        _uid = args[i + 1];
                        break;
                }
            }

            // main function
            var cs = NewCS(dbHost: _dbHost, dbPort: _dbPort, dbDB: _dbDB, dbUserID: _dbUserID, dbPass: _dbPass);
            int output = UIDMatch(cs: cs.ToString(), tableName: "public.tbl_appdata", UIDValue: _uid);

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
                        cmd.CommandText = $"SELECT * FROM {tableName} WHERE uid = '{UIDValue}';";
                        cmd.Connection = con;
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
