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
using System.Text;
using System.Security.Policy;
using dBASE.NET;

namespace EStoreBillReader.Views
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class PdfReaderView : UserControl
    {
        private bool isProcessRunning = false;
        private List<string> strSplit = new List<string>();
        private OleDbDataAdapter ap;
        OleDbCommandBuilder CmdBuilder; //for genrating insert command in adapter
        private OleDbConnection conn = new OleDbConnection();
        private DataSet salesDB = new DataSet();

        public PdfReaderView()
        {
            InitializeComponent();
        }
        private void UserControl_Initialized(object sender, EventArgs e)
        {
            Defaults();
        }
        private void Defaults()
        {
            View_textbx.Text = null;
            Result_textbx.Text = null;
            PBar.Value = 0;
            PBarTxt.Text = null;
            checkCanc.Checked = Properties.Settings.Default.CheckCanc;
        }
        //private DBFWriter writer;
        private Dbf dbf;
        private void StreamWrite(string MonthYear, string dbTableName)
        {
            try
            {
                Helper.checkDatabasePath(MonthYear);
                dbf = new Dbf();
                dbf.Read(Helper.DBTablePath(dbTableName, MonthYear));
                // writer = new DBFWriter(Helper.DBTablePath(dbTableName, MonthYear));

                //ap = new OleDbDataAdapter();
                //CmdBuilder = new OleDbCommandBuilder(ap);
                //if (conn.State == ConnectionState.Open)
                //   conn.Close();
                //conn = Helper.DbConnection(MonthYear);
                //ap.SelectCommand = new OleDbCommand("select * from " + dbTableName + MonthYear.Remove(2, 2), conn);
                //ap.SelectCommand.Connection.Open();
                //ap.Fill(salesDB, dbTableName + MonthYear.Remove(2, 2));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.StackTrace}  \n{ex.Message}", "EXP Update Adapter");
            }
        }


        private void Convert_btn_Click(object sender, RoutedEventArgs e)
        {
            if (isProcessRunning)
            {
                return;
            }
            List<string> strFiles = new List<string>();
            OpenFileDialog fd = new OpenFileDialog()
            {
                Title = "Select PDF File",
                Multiselect = true,
                Filter = "PDF|*.pdf",
                FilterIndex = 1,
                RestoreDirectory = false,
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "This Folder"
            };
            if (fd.ShowDialog() ?? false)
            {
                Defaults();
                //Process the list of .Pdf files found in the directory.
                strFiles = (fd.FileName.Contains("This Folder")
                    ? Directory.GetFiles(Path.GetDirectoryName(fd.FileName), "*.pdf").ToList()
                    : fd.FileNames.ToList());
            }
            PBar.Maximum = strFiles.Count;
            Thread backgroundThread = new Thread(
                () => FetchFiles(strFiles));
            backgroundThread.Start();
        }
        private void FetchFiles(List<string> strFiles)
        {
            isProcessRunning = true;
            PBar.Dispatcher.BeginInvoke(new Action(() => PBar.Value = 0));
            int ct = 0;
            foreach (string File in strFiles)
            {
                PBar.Dispatcher.BeginInvoke(new Action(() => PBar.Value = ct));
                PBarTxt.Dispatcher.BeginInvoke(new Action(() => PBarTxt.Text = PBar.Value + "/" + PBar.Maximum));
                ct++;
                if (File.Contains("canc") && (Properties.Settings.Default.CheckCanc))
                {
                    continue;
                }
                else
                {
                    //*/
                    View_textbx.Dispatcher.BeginInvoke(new Action(() => View_textbx.Text = PdftoText.ExtractTextFromPdf(File)));
                    //*/
                    View_textbx.Dispatcher.BeginInvoke(new Action(() => View_textbx.Text += File + "\n"));
                    ReadText(File);
                }
            }
            isProcessRunning = false;
        }

        private void ReadText(string FilePath)
        {
            strSplit = PdftoText.ExtractTextFromPdf(FilePath).Split('\n').ToList();
            if (strSplit[0].Contains("Tax Invoice/Bill of Supply/Cash Memo"))
            {
                AmazonParser();
            }
            else if (strSplit[0].Contains("COD Collect amount :") || strSplit[0].Contains("PREPAID - DO NOT COLLECT CASH"))
            {
                FlipkartParser();
            }
            Result_textbx.Dispatcher.BeginInvoke(new Action(() => Result_textbx.Text += "Done" + "\n"));
        }

        private void FlipkartParser()
        {
            string Monthyear = null;
            int cnt;
            for (cnt = 0; cnt < strSplit.Count; cnt++)
            {
                if (strSplit[cnt].Contains("Tax Invoice"))
                {
                    Monthyear = strSplit[cnt + 1].Split(' ')[7].Replace(",", string.Empty).Replace("-", "/").Remove(0, 3).Remove(2, 1);
                    StreamWrite(Monthyear, DBTableType.Sales);
                    break;
                }
            }
            object TRTYPE = "";
            object INV = "";
            object INVDATE = "";
            object ORDER = "";
            object ORDDT = "";
            object SKU = "";
            object QTY = "";
            object GSTNO = "";
            object BASIC = "";
            object SHIP = "";
            object TAXRATE = "";
            object SGST = "";
            object CGST = "";
            object IGST = "";
            object TOTAMT = "";
            object ESTORE = "";
            object CANCELLED = "";
            object STATUS = "";
            object STATDT = "";
            object DLVRYSTATE = "";

            ESTORE = "FLIPKART";
            TRTYPE = "SALE";
            GSTNO = "09AYGPC2149C1ZU";
            CANCELLED = "2/2/2";
            STATUS = " ";
            STATDT = "2/2/2";
            ORDER = " ";
            try
            {
                string tmp = null;
                for (cnt = 0; cnt < strSplit.Count; cnt++)
                {
                    //Result_textbx.Dispatcher.BeginInvoke(new Action(() => Result_textbx.Text += cnt.ToString() + " " + strSplit[cnt] + "\n"));

                    List<string> Temp = new List<string>();
                    if (strSplit[cnt].Contains("Courier Name: "))
                    {
                        DLVRYSTATE = PdftoText.FindState(strSplit[cnt - 1].Split(' ')[strSplit[cnt - 1].Split(' ').Length - 1]);
                    }
                    if (strSplit[cnt].Contains("Product Qty"))
                    {
                        try
                        {
                            Temp = strSplit[cnt + 1].Replace(" ", string.Empty).Split('|').ToList();
                            SKU = Temp[Temp.Count - 2];
                        }
                        catch (ArgumentOutOfRangeException)
                        {

                            Temp = strSplit[cnt + 2].Replace(" ", string.Empty).Split('|').ToList();
                            SKU = Temp[Temp.Count - 2];
                        }
                    }
                    if (strSplit[cnt].Contains("Handover to"))
                    {
                        try
                        {
                            QTY = strSplit[cnt - 2].Split(' ')[1];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            QTY = strSplit[cnt - 3].Split(' ')[1];
                        }
                    }
                    if (strSplit[cnt].Contains("Tax Invoice"))
                    {
                        ORDER = strSplit[cnt - 1].Split(' ')[2];
                        INV = strSplit[cnt - 1].Split(' ')[5];
                        ORDDT = strSplit[cnt + 1].Split(' ')[2].Replace(",", string.Empty).Replace("-", "/");
                        INVDATE = strSplit[cnt + 1].Split(' ')[7].Replace(",", string.Empty).Replace("-", "/");

                    }
                    if (strSplit[cnt].Contains("HSN:"))
                    {
                        tmp = (strSplit[cnt].Replace("|", string.Empty) + " "
                            + strSplit[cnt + 1].Replace("|", string.Empty) + " "
                            + strSplit[cnt + 2].Replace("|", string.Empty)).Replace("  ", " ");
                        Temp = tmp.Split(' ').ToList();
                        for (int i = 0; i < Temp.Count; i++)
                        {
                            if (Temp[i] == QTY.ToString())
                            {
                                if (Temp[i - 1] != "Charge")
                                {
                                    BASIC = Temp[i + 3];
                                    if (tmp.Contains("CGST"))
                                    {
                                        CGST = Temp[i + 4];
                                        SGST = Temp[i + 4];
                                    }
                                    else if (tmp.Contains("IGST"))
                                    {
                                        IGST = Temp[i + 4];
                                    }
                                }
                            }
                            if (tmp.Contains("CGST"))
                            {
                                if (Temp[i].Contains('%'))
                                {
                                    TAXRATE = (Convert.ToDecimal(Temp[i].Replace("%", string.Empty)) * 2).ToString();
                                    IGST = "0";
                                }
                            }
                            else if (tmp.Contains("IGST"))
                            {
                                if (Temp[i].Contains('%'))
                                {
                                    TAXRATE = Temp[i].Replace("%", string.Empty);
                                    CGST = "0";
                                    SGST = "0";
                                }
                            }
                        }
                    }

                    if (strSplit[cnt].Contains("Shipping Charge"))
                    {
                        SHIP = strSplit[cnt].Split(' ')[5];
                        TOTAMT = strSplit[cnt + 1].Split(' ')[2];
                        decimal ttmp = decimal.Parse(strSplit[cnt].Split(' ')[6]);
                        if (tmp.Contains("CGST"))
                        {
                            CGST = decimal.Parse(CGST.ToString()) + ttmp;
                            SGST = decimal.Parse(SGST.ToString()) + ttmp;
                        }
                        else if (tmp.Contains("IGST"))
                        {
                            IGST = decimal.Parse(IGST.ToString()) + ttmp;
                        }

                        if (isNewRecord(INVDATE.ToString(), ORDER.ToString()))
                        {
                            DbfRecord record = dbf.CreateRecord();
                            record.Data[0] = TRTYPE;
                            record.Data[1] = INV;
                            record.Data[2] = INVDATE;
                            record.Data[3] = ORDER;
                            record.Data[4] = ORDDT;
                            record.Data[5] = SKU;
                            record.Data[6] = QTY;
                            record.Data[7] = GSTNO;
                            record.Data[8] = BASIC;
                            record.Data[9] = SHIP;
                            record.Data[10] = TAXRATE;
                            record.Data[11] = SGST;
                            record.Data[12] = CGST;
                            record.Data[13] = IGST;
                            record.Data[14] = TOTAMT;
                            record.Data[15] = ESTORE;
                            record.Data[16] = CANCELLED;
                            record.Data[17] = STATUS;
                            record.Data[18] = STATDT;
                            record.Data[19] = DLVRYSTATE;
                            dbf.Write(Helper.DBTablePath(DBTableType.Sales, Monthyear), DbfVersion.VisualFoxPro);
                        }
                        // CommitDBChanges(dr["orddt"].ToString(), dr);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error uploading Flipkart order " + dr["order"].ToString(), "Upload Error");
                try
                {
                    Result_textbx.Dispatcher.BeginInvoke(new Action(() => Result_textbx.Text += "Error uploading Flipkart order : " + ORDER + "\n"));
                }
                catch (Exception)
                {
                    MessageBox.Show(ex.Message.ToString() + " " + ex.StackTrace.ToString() + " " + ex.ToString(), "Upload Error");
                }
            }
        }

        private void AmazonParser()
        {
            string Monthyear = null;
            int cnt;
            for (cnt = 0; cnt < strSplit.Count; cnt++)
            {
                if (strSplit[cnt].Contains("Invoice Date : "))
                {
                    Monthyear = strSplit[cnt].Split(' ')[3].Replace(".", "/").Remove(0, 3).Remove(2, 1);
                    StreamWrite(Monthyear, DBTableType.Sales);
                    break;
                }
            }

            object TRTYPE = "";
            object INV = "";
            object INVDATE = "";
            object ORDER = "";
            object ORDDT = "";
            object SKU = "";
            object QTY = "";
            object GSTNO = "";
            object BASIC = "";
            object SHIP = "";
            object TAXRATE = "";
            object SGST = "";
            object CGST = "";
            object IGST = "";
            object TOTAMT = "";
            object ESTORE = "";
            object CANCELLED = "";
            object STATUS = "";
            object STATDT = "";
            object DLVRYSTATE = "";

            ESTORE = "AMAZON";
            TRTYPE = "SALE";
            GSTNO = "09AYGPC2149C1ZU";
            CANCELLED = "2/2/2";
            STATUS = " ";
            STATDT = "2/2/2";
            ORDER = " ";
            try
            {
                for (cnt = 0; cnt < strSplit.Count; cnt++)
                {
                    //Result_textbx.Dispatcher.BeginInvoke(new Action(() => Result_textbx.Text += cnt.ToString() + " " + strSplit[cnt] + "\n"));
                    List<string> Temp = new List<string>();
                    if (strSplit[cnt].Contains("Order Number: "))
                    {
                        ORDER = strSplit[cnt].Split(' ')[2];
                        INV = strSplit[cnt].Split(' ')[6];
                        try
                        {
                            Temp = strSplit[cnt - 2].Split(',').ToList();
                            DLVRYSTATE = Temp[1].Trim();
                        }
                        catch (Exception)
                        {
                            Temp = strSplit[cnt - 3].Split(',').ToList();
                            DLVRYSTATE = Temp[1].Trim();
                        }
                    }
                    if (strSplit[cnt].Contains("Invoice Date : "))
                    {
                        INVDATE = strSplit[cnt].Split(' ')[3].Replace(".", "/");
                    }
                    if (strSplit[cnt].Contains("Order Date: "))
                    {
                        ORDDT = strSplit[cnt].Split(' ')[2].Replace(".", "/");

                    }
                    if (strSplit[cnt].Contains("CGST") || strSplit[cnt].Contains("IGST"))
                    {
                        if (!strSplit[cnt].Contains("₹"))
                            strSplit[cnt] += " " + strSplit[cnt + 1];

                        Temp = strSplit[cnt].Split(' ').ToList();
                        SKU = Temp[Temp.Count - 9];
                        Temp = strSplit[cnt].Replace("₹", " ").Split(' ').ToList();
                        QTY = Temp[Temp.Count - 8];
                        string subtrate = Temp[Temp.Count - 6];
                        BASIC = subtrate;
                        if (strSplit[cnt].Contains("IGST"))
                        {
                            TAXRATE = Temp[Temp.Count - 5].Replace("%", string.Empty).Replace(subtrate, string.Empty);
                            cnt += 2;
                            Temp = strSplit[cnt + 2].Replace("₹", " ").Split(' ').ToList();
                            SHIP = Temp[3];
                            Temp = strSplit[cnt + 4].Replace("₹", "  ").Split(' ').ToList();
                            IGST = Temp[2];
                            CGST = "0";
                            SGST = "0";
                            TOTAMT = Temp[5];
                        }
                        else if (strSplit[cnt].Contains("CGST"))
                        {
                            TAXRATE = ((decimal.Parse(Temp[Temp.Count - 5].Replace("%", string.Empty))) * 2).ToString();

                            if (strSplit[cnt + 4].Contains("Shipping Charges"))
                            {
                                Temp = strSplit[cnt + 3].Replace("₹", " ").Split(' ').ToList();
                                SHIP = Temp[3];

                                Temp = strSplit[cnt + 8].Replace("₹", " ").Split(' ').ToList();
                                CGST = (decimal.Parse(Temp[1]) / 2).ToString();
                                SGST = (decimal.Parse(Temp[1]) / 2).ToString();
                                IGST = "0";
                                TOTAMT = Temp[2];
                            }
                            else if (strSplit[cnt + 5].Contains("Shipping Charges"))
                            {
                                Temp = strSplit[cnt + 5].Replace("₹", " ").Split(' ').ToList();
                                SHIP = Temp[Temp.Count - 7];

                                Temp = strSplit[cnt + 9].Replace("₹", " ").Split(' ').ToList();
                                CGST = (decimal.Parse(Temp[1]) / 2).ToString();
                                SGST = (decimal.Parse(Temp[1]) / 2).ToString();
                                IGST = "0";
                                TOTAMT = Temp[3];
                            }
                        }
                        if (isNewRecord(INVDATE.ToString(), ORDER.ToString()))
                        {
                            DbfRecord record = dbf.CreateRecord();
                            record.Data[0] = TRTYPE;
                            record.Data[1] = INV;
                            record.Data[2] = INVDATE;
                            record.Data[3] = ORDER;
                            record.Data[4] = ORDDT;
                            record.Data[5] = SKU;
                            record.Data[6] = QTY;
                            record.Data[7] = GSTNO;
                            record.Data[8] = BASIC;
                            record.Data[9] = SHIP;
                            record.Data[10] = TAXRATE;
                            record.Data[11] = SGST;
                            record.Data[12] = CGST;
                            record.Data[13] = IGST;
                            record.Data[14] = TOTAMT;
                            record.Data[15] = ESTORE;
                            record.Data[16] = CANCELLED;
                            record.Data[17] = STATUS;
                            record.Data[18] = STATDT;
                            record.Data[19] = DLVRYSTATE;
                            dbf.Write(Helper.DBTablePath(DBTableType.Sales, Monthyear), DbfVersion.VisualFoxPro);

                        }
                        /*
                        if (isNewRecord(ORDDT.ToString(), ORDER.ToString()))
                        {
                            writer.CharEncoding = Encoding.ASCII;
                            writer.WriteRecord(TRTYPE, INV, INVDATE, ORDER, ORDDT, SKU, QTY, GSTNO, BASIC, SHIP, TAXRATE,
                                SGST, CGST, IGST, TOTAMT78, ESTORE, CANCELLED, STATUS, STATDT, DLVRYSTATE);
                        }
                        writer.Close();
                        */
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error uploading Amazon order : " + dr("order"), "Upload Error");
                //writer.Close();
                Result_textbx.Dispatcher.BeginInvoke(new Action(() => Result_textbx.Text += "Error uploading Amazon order : " + ORDER + "\n"));
            }
        }

        private void PBarTxt_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //lbl_progressbar.Location = New Point(Me.Width - lbl_progressbar.Width - ProgressBar1.Width - 35, lbl_progressbar.Location.Y)

        }

        private bool isNewRecord(string InvDate, string Order)
        {
            bool athenticateRow = true;
            string MonthYear = InvDate.Remove(0, 3).Remove(2, 1);
            var D = new Dbf();
            D.Read(Helper.DBTablePath(DBTableType.Sales, MonthYear));
            foreach (DbfRecord record in D.Records)
            {
                if (record[3].ToString().Trim(' ').Equals(Order.Trim(' ')))
                {
                    athenticateRow = false;
                    break;
                }
            }
            /*

            writer.Close();
            using (Stream fos = File.Open(Helper.DBTablePath(DBTableType.Sales, MonthYear), FileMode.Open, FileAccess.Read))
            {
                DBFReader rr = new DBFReader(fos);
                object[] row;
                while ((row = rr.NextRecord()) != null)
                {
                    if (row[3].ToString().Trim(' ').Equals(Order.Trim(' ')))
                    {
                        athenticateRow = false;
                        break;
                    }
                }
            }
            writer = new DBFWriter(Helper.DBTablePath(DBTableType.Sales, MonthYear));
            */

            return athenticateRow;
        }

        private void checkCanc_isCheckedChanged(object sender, Custom.ToggleEventArgs e)
        {
            Properties.Settings.Default.CheckCanc = checkCanc.Checked;
            Properties.Settings.Default.Save();
        }

        private void Manual_btn_Click(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this);
            parent.Hide();
            ManualEntry ME = new ManualEntry();
            ME.Topmost = true;
            ME.ShowInTaskbar = false;
            ME.ShowDialog();
            parent.Show();
        }
    }
}
