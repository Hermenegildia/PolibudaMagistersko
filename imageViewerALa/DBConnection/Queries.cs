using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnection
{
    public static class Queries
    {
        public static string CreateDatabase()
        {
            return @"CREATE DATABASE [bazka] ";
        }

        public static string CreateDatabase(string dbName)
        {
            string query = CreateDatabase();
            query = query.Replace("bazka", dbName);
            return query;
        }

        public static string CreateTable(string tableName)
        {
            string query = @"CREATE TABLE [dbo].[Table_1] ([id] [int] IDENTITY(1,1) NOT NULL, [name] [nvarchar](max) NULL, [last_name] [nvarchar](max) NULL, CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED ([id] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            query = query.Replace("Table_1", tableName);
            return query;
        }
    }
}
