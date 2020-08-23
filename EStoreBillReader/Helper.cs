using System.Windows;
using Microsoft.Win32;
using System.Data.OleDb;
using System.Data;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReadingTools;
using System.Windows.Controls;
using dBASE.NET;
using System.Runtime.CompilerServices;

namespace EStoreBillReader
{
    internal static class Helper
    {
        internal static Properties.Settings SetProperty = Properties.Settings.Default;

        public static string MonthNow { get => DateTime.Now.ToString("MMyyyy"); }
        public static string ShortMonthNow { get => DateTime.Now.ToString("MMyy"); }
        internal static string DirPath
        {
            get => SetProperty.DirPath; set
            {
                SetProperty.DirPath = value;
                SetProperty.Save();
            }
        }
        internal static string DbPath(string MonthYear)
        {
            var dbSource = DirPath + "\\" + MonthYear;
            if (!CheckPathExists(dbSource))
                CreatePath(dbSource);
            return dbSource;
        }

        internal static string DBTablePath(string DbTable, string Monthyear)
        {
            return DirPath + "\\" + Monthyear + "\\" + DbTable + Monthyear.Remove(2, 2) + ".DBF";
        }

        internal static bool CheckPathExists(string filepath)
        {
            if (filepath.Contains(".")) return File.Exists(filepath);
            else return Directory.Exists(filepath);
        }

        internal static void CreatePath(string filepath)
        {
            if (filepath.EndsWith(".dbf")) File.Create(filepath);
            else Directory.CreateDirectory(filepath);
        }
        internal static void checkDatabasePath(string Monthyear)
        {
            if (!CheckPathExists(DBTablePath(DBTableType.Sales, Monthyear)))
                CreateTable(DBTableType.Sales, Monthyear);

            if (!CheckPathExists(DBTablePath(DBTableType.Purchase, Monthyear)))
                CreateTable(DBTableType.Purchase, Monthyear);

            if (!CheckPathExists(DBTablePath(DBTableType.Charges, Monthyear)))
                CreateTable(DBTableType.Charges, Monthyear);
        }
        /// <summary>
        /// Creates custom DBF file accordingly
        /// </summary>
        /// <param name="DbTable"></param>
        internal static void CreateTable(string DbTable, string Monthyear)
        {
            var file = DbPath(Monthyear) + "\\" + DbTable + Monthyear.Remove(2, 2) + ".dbf";
            if (DbTable == DBTableType.Sales)
                CreateSalesDB(file);
            else if (DbTable == DBTableType.Purchase)
                CreatePurchaseDB(file);
            else if (DbTable == DBTableType.Charges)
                CreateChargesDB(file);
        }
        //TODO: dbase.net
        private static void CreateChargesDB(string file)
        {

            var CDB = new Dbf();
            var fields = new List<DbfField>()
            {
                    new DbfField("TRTYPE",      DbfFieldType.Character, 30),
                    new DbfField("INV",         DbfFieldType.Character, 20),
                    new DbfField("INVDATE",     DbfFieldType.Character, 10),
                    new DbfField("ORDER",       DbfFieldType.Character, 25),
                    new DbfField("GSTNO",       DbfFieldType.Character, 15),
                    new DbfField("BASIC",       DbfFieldType.Numeric, 17,2),
                    new DbfField("TAXRATE",     DbfFieldType.Numeric, 7,2),
                    new DbfField("SGST",        DbfFieldType.Numeric, 12,2),
                    new DbfField("CGST",        DbfFieldType.Numeric, 12,2),
                    new DbfField("IGST",        DbfFieldType.Numeric, 12,2),
                    new DbfField("TOTAMT",      DbfFieldType.Numeric, 17,2),
                    new DbfField("ESTORE",      DbfFieldType.Character, 30),
                    new DbfField("PAYRECD",     DbfFieldType.Numeric, 17,2),
                    new DbfField("PAIDON",      DbfFieldType.Character,10),
                    new DbfField("CANCELLED",   DbfFieldType.Character,10),
                    new DbfField("STATUS",      DbfFieldType.Character, 20),
                    new DbfField("STATDT",      DbfFieldType.Character,10),
                    new DbfField("DLVRYSTATE",  DbfFieldType.Character, 25)
            };
            CDB.Fields.AddRange(fields);
            CDB.Write(file, DbfVersion.VisualFoxPro);

            ///Sql
            /*
             str = "CREATE TABLE " + DbTable + Monthyear.Remove(2, 2) +
             "(TRTYPE Char(30), INV Char(20), INVDATE DateTime,ORDER Char(25), GSTNO Char(15), BASIC Numeric(15,2), TAXRATE Numeric(5,2), SGST Numeric(10,2)," +
             " CGST Numeric(10,2), IGST Numeric(10,2), TOTAMT Numeric(15,2), ESTORE Char(30), PAYRECD Numeric(15,2), PAIDON DateTime,CANCELLED DateTime," +
             " STATUS Char(20), STATDT DateTime,DLVRYSTATE Char(20))";
            */
            /*
            using (Stream fos = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var writer = new DBFWriter
                {
                    DataMemoLoc = Path.ChangeExtension(file, "DBT")
                };
                writer.Fields = new[] {
                    new DBFField("TRTYPE", NativeDbType.Char, 30),
                    new DBFField("INV", NativeDbType.Char, 20),
                    new DBFField("INVDATE", NativeDbType.Char, 10),
                    new DBFField("ORDER", NativeDbType.Char, 25),
                    new DBFField("GSTNO", NativeDbType.Char, 15),
                    new DBFField("BASIC", NativeDbType.Numeric, 17,2),
                    new DBFField("TAXRATE", NativeDbType.Numeric, 7,2),
                    new DBFField("SGST", NativeDbType.Numeric, 12,2),
                    new DBFField("CGST", NativeDbType.Numeric, 12,2),
                    new DBFField("IGST", NativeDbType.Numeric, 12,2),
                    new DBFField("TOTAMT", NativeDbType.Numeric, 17,2),
                    new DBFField("ESTORE", NativeDbType.Char, 30),
                    new DBFField("PAYRECD", NativeDbType.Numeric, 17,2),
                    new DBFField("PAIDON", NativeDbType.Char,10),
                    new DBFField("CANCELLED", NativeDbType.Char,10),
                    new DBFField("STATUS", NativeDbType.Char, 20),
                    new DBFField("STATDT", NativeDbType.Char,10),
                    new DBFField("DLVRYSTATE", NativeDbType.Char, 25)
                };
                writer.Write(fos);
                writer.Close();
                File.Delete(Path.ChangeExtension(file, "DBT"));
            }*/
        }

        private static void CreatePurchaseDB(string file)
        {
            var PDB = new Dbf();
            var fields = new List<DbfField>()
            {
                    new DbfField("TRTYPE",     DbfFieldType.Character, 30),
                    new DbfField("INV",        DbfFieldType.Character, 20),
                    new DbfField("INVDATE",    DbfFieldType.Character, 10),
                    new DbfField("ORDER",      DbfFieldType.Character, 25),
                    new DbfField("GSTNO",      DbfFieldType.Character, 15),
                    new DbfField("BASIC",      DbfFieldType.Numeric, 17,2),
                    new DbfField("TAXRATE",    DbfFieldType.Numeric, 7,2),
                    new DbfField("SGST",       DbfFieldType.Numeric, 12,2),
                    new DbfField("CGST",       DbfFieldType.Numeric, 12,2),
                    new DbfField("IGST",       DbfFieldType.Numeric, 12,2),
                    new DbfField("TOTAMT",     DbfFieldType.Numeric, 17,2),
                    new DbfField("ESTORE",     DbfFieldType.Character, 30),
                    new DbfField("PAYRECD",    DbfFieldType.Numeric, 17,2),
                    new DbfField("PAIDON",     DbfFieldType.Character,10),
                    new DbfField("CANCELLED",  DbfFieldType.Character,10),
                    new DbfField("STATUS",     DbfFieldType.Character, 20),
                    new DbfField("STATDT",     DbfFieldType.Character,10),
                    new DbfField("DLVRYSTATE", DbfFieldType.Character, 25)
            };
            PDB.Fields.AddRange(fields);
            PDB.Write(file, DbfVersion.VisualFoxPro);

            ///sql    
            /*
             str = "CREATE TABLE " + DbTable + Monthyear.Remove(2, 2) +
             "(TRTYPE Char(25), INV Char(20), INVDATE DateTime,ORDER Char(25), GSTNO Char(15), BASIC Numeric(15,2), TAXRATE Numeric(5,2)," +
             " SGST Numeric(10,2), CGST Numeric(10,2), IGST Numeric(10,2), TOTAMT Numeric(15,2), ESTORE Char(30), PAYRECD Numeric(15,2), PAIDON DateTime," +
             " CANCELLED DateTime, STATUS Char(20), STATDT DateTime,DLVRYSTATE Char(20))";
            */
            /*
            using (Stream fos = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var writer = new DBFWriter
                {
                    DataMemoLoc = Path.ChangeExtension(file, "DBT")
                };
                writer.Fields = new[] {
                    new DBFField("TRTYPE", NativeDbType.Char, 30),
                    new DBFField("INV", NativeDbType.Char, 20),
                    new DBFField("INVDATE", NativeDbType.Char, 10),
                    new DBFField("ORDER", NativeDbType.Char, 25),
                    new DBFField("GSTNO", NativeDbType.Char, 15),
                    new DBFField("BASIC", NativeDbType.Numeric, 17,2),
                    new DBFField("TAXRATE", NativeDbType.Numeric, 7,2),
                    new DBFField("SGST", NativeDbType.Numeric, 12,2),
                    new DBFField("CGST", NativeDbType.Numeric, 12,2),
                    new DBFField("IGST", NativeDbType.Numeric, 12,2),
                    new DBFField("TOTAMT", NativeDbType.Numeric, 17,2),
                    new DBFField("ESTORE", NativeDbType.Char, 30),
                    new DBFField("PAYRECD", NativeDbType.Numeric, 17,2),
                    new DBFField("PAIDON", NativeDbType.Char,10),
                    new DBFField("CANCELLED", NativeDbType.Char,10),
                    new DBFField("STATUS", NativeDbType.Char, 20),
                    new DBFField("STATDT", NativeDbType.Char,10),
                    new DBFField("DLVRYSTATE", NativeDbType.Char, 25)
                };
                writer.Write(fos);
                writer.Close();
                File.Delete(Path.ChangeExtension(file, "DBT"));
            }
            */
        }

        private static void CreateSalesDB(string file)
        {
            var SDB = new Dbf();
            var fields = new List<DbfField>()
            {
                    new DbfField("TRTYPE",     DbfFieldType.Character, 30),
                    new DbfField("INV",        DbfFieldType.Character, 20),
                    new DbfField("INVDATE",    DbfFieldType.Character, 10),
                    new DbfField("ORDER",      DbfFieldType.Character, 25),
                    new DbfField("ORDDT",      DbfFieldType.Character, 10),
                    new DbfField("SKU",        DbfFieldType.Character, 10),
                    new DbfField("QTY",        DbfFieldType.Character, 5),
                    new DbfField("GSTNO",      DbfFieldType.Character, 15),
                    new DbfField("BASIC",      DbfFieldType.Numeric, 17,2),
                    new DbfField("SHIP",       DbfFieldType.Numeric, 12,2),
                    new DbfField("TAXRATE",    DbfFieldType.Numeric, 7,2),
                    new DbfField("SGST",       DbfFieldType.Numeric, 12,2),
                    new DbfField("CGST",       DbfFieldType.Numeric, 12,2),
                    new DbfField("IGST",       DbfFieldType.Numeric,12,2),
                    new DbfField("TOTAMT",     DbfFieldType.Numeric,17,2),
                    new DbfField("ESTORE",     DbfFieldType.Character, 30),
                    new DbfField("CANCELLED",  DbfFieldType.Character,10),
                    new DbfField("STATUS",     DbfFieldType.Character, 20),
                    new DbfField("STATDT",     DbfFieldType.Character, 10),
                    new DbfField("DLVRYSTATE", DbfFieldType.Character, 25)
            };
            SDB.Fields.AddRange(fields);
            SDB.Write(file, DbfVersion.VisualFoxPro);
            ///Sql
            /*
             str = "CREATE TABLE " + DbTable + Monthyear.Remove(2, 2) +
             " (TRTYPE Char(25), INV Char(20), INVDATE Char(10), ORDER Char(25), ORDDT Char(10), SKU Char(10), QTY Integer," +
             " GSTNO Char(15), BASIC Numeric(15,2), SHIP Numeric(10,2), TAXRATE Numeric(5,2), SGST Numeric(10,2), CGST Numeric(10,2)," +
             " IGST Numeric(10,2), TOTAMT Numeric(15,2), ESTORE Char(30), CANCELLED Char(10), STATUS Char(20), STATDT Char(10), DLVRYSTATE Char(25))";
            */
            /*
            using (Stream fos = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var writer = new DBFWriter
                {
                    DataMemoLoc = Path.ChangeExtension(file, "DBT")
                };
                writer.Fields = new[] {
                    new DBFField("TRTYPE",      NativeDbType.Char, 30),
                    new DBFField("INV",         NativeDbType.Char, 20),
                    new DBFField("INVDATE",     NativeDbType.Char, 10),
                    new DBFField("ORDER",       NativeDbType.Char, 25),
                    new DBFField("ORDDT",       NativeDbType.Char, 10),
                    new DBFField("SKU",         NativeDbType.Char, 10),
                    new DBFField("QTY",         NativeDbType.Char,5),
                    new DBFField("GSTNO",       NativeDbType.Char,15),
                    new DBFField("BASIC",       NativeDbType.Numeric, 17,2),
                    new DBFField("SHIP",        NativeDbType.Numeric, 12,2),
                    new DBFField("TAXRATE",     NativeDbType.Numeric, 7,2),
                    new DBFField("SGST",        NativeDbType.Numeric, 12,2),
                    new DBFField("CGST",        NativeDbType.Numeric, 12,2),
                    new DBFField("IGST",        NativeDbType.Numeric, 12,2),
                    new DBFField("TOTAMT",      NativeDbType.Numeric, 17,2),
                    new DBFField("ESTORE",      NativeDbType.Char, 30),
                    new DBFField("CANCELLED",   NativeDbType.Char, 10),
                    new DBFField("STATUS",      NativeDbType.Char, 20),
                    new DBFField("STATDT",      NativeDbType.Char, 10),
                    new DBFField("DLVRYSTATE",  NativeDbType.Char, 25),
                };
                writer.Write(fos);
                writer.Close();
                File.Delete(Path.ChangeExtension(file, "DBT"));
            }
            */
        }

        #region DBConnection
        internal static OleDbConnection DbConnection(string MonthYear)
        {
            return DbConnection(SetProperty.Provider, DirPath + "\\" + MonthYear);
        }
        private static OleDbConnection DbConnection(string dbProvider, string dbSource)
        {
            if (!CheckPathExists(dbSource))
                CreatePath(dbSource);
            OleDbConnectionStringBuilder csb = new OleDbConnectionStringBuilder()
            {
                DataSource = dbSource,
                Provider = dbProvider
            };
            return new OleDbConnection(csb.ConnectionString);
        }
        #endregion
    }
    internal static class DBTableType
    {
        internal static string Sales = "sale";
        internal static string Purchase = "purs";
        internal static string Charges = "chrg";
    }
}
