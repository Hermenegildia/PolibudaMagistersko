﻿using System;
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
            //return @"CREATE DATABASE [bazka] CONTAINMENT = NONE ON  PRIMARY ( NAME = N'bazia', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\bazia.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) LOG ON ( NAME = N'bazia_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\bazia_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)";
        }

        public static string CreateDatabase(string dbName)
        {
            string query = CreateDatabase();
            query = query.Replace("bazka", dbName);
            return query;
        }

        public static string CreateTablePatients()
        {
            return @"CREATE TABLE [dbo].[patients] ([id] [int] IDENTITY(1,1) NOT NULL, [name] [nvarchar](max) NULL, [last_name] [nvarchar](max) NULL, CONSTRAINT [PK_patients] PRIMARY KEY CLUSTERED ([id] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
        
        }

        public static string CreateTableDocumentation()
        {
            return @"CREATE TABLE [dbo].[documentation]([id] [int] IDENTITY(1,1) NOT NULL, [patient_id] [int] NULL, [filePath] [nvarchar](max) NULL, CONSTRAINT [PK_documentation] PRIMARY KEY CLUSTERED ([id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
        }
    }
}
