using System;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.Windows.Forms;

namespace ReadingTools
{
        
    public static class PdftoText
    {
        /// <summary>
        /// extracts pdf, needs file path 
        /// </summary>
        /// <param name="Filepath"></param>
        /// <param name="errorFlag"></param>
        /// <returns></returns>
        public static string ExtractTextFromPdf(string Filepath)
        {
            try
            {
                using (PdfReader reader = new PdfReader(Filepath))
                {
                    StringBuilder text = new StringBuilder();

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                    }
                    
                    return text.ToString();
                }                
            }
            catch (Exception)
            {
                MessageBox.Show("Error Converting", "Plugin Error",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }
        /// <summary>
        /// Change state code (DL) to names(DELHI)
        /// </summary>
        /// <param name="Contration"></param>
        /// <returns></returns>
        public static string FindState(string Contration)
        {
            switch (Contration)
            {
                case "IN-AP":
                    return "ANDHRA PRADESH";
                case "IN-AR":
                    return "ARUNACHAL PRADESH";
                case "IN-AS":
                    return "ASSAM";
                case "IN-BR":
                    return "BIHAR";
                case "IN-CG":
                    return "CHHATTISGARH";
                case "IN-DL":
                    return "DELHI";
                case "IN-GA":
                    return "GOA";
                case "IN-GJ":
                    return "GUJRAT";
                case "IN-HR":
                    return "HARYANA";
                case "IN-HP":
                    return "HIMACHAL PRADESH";
                case "IN-JK":
                    return "JAMMU & KASHMIR";
                case "IN-JS":
                    return "JHARKHAND";
                case "IN-KA":
                    return "KARNATAKA";
                case "IN-KL":
                    return "KERALA";
                case "IN-MP":
                    return "MADHYA PRADESH";
                case "IN-MH":
                    return "MAHARASHTRA";
                case "IN-MN":
                    return "MANIPUR";
                case "IN-ML":
                    return "MEGHALAYA";
                case "IN-MZ":
                    return "MIZORAM";
                case "IN-NL":
                    return "NAGALAND";
                case "IN-OR":
                    return "ORISSA";
                case "IN-PB":
                    return "PUNJAB";
                case "IN-RJ":
                    return "RAJASTHAN";
                case "IN-SK":
                    return "SIKKIM";
                case "IN-TN":
                    return "TAMIL NADU";
                case "IN-TR":
                    return "TRIPURA";
                case "IN-TS":
                    return "TELANGANA";
                case "IN-UK":
                    return "UTTARAKHAND";
                case "IN-UP":
                    return "UTTAR PRADESH";
                case "IN-WB":
                    return "WEST BENGAL";
                case "IN-AN":
                    return "ANDAMAN & NICOBAR";
                case "IN-CH":
                    return "CHANDIGARH";
                case "IN-DN":
                    return "DADRA AND NAGAR HAVELI";
                case "IN-DD":
                    return "DAMAN & DIU";
                case "IN-LD":
                    return "LAKSHADWEEP";
                case "IN-PY":
                    return "PONDICHERRY";
                default:
                    return "None";
            }
        }
    }
}
