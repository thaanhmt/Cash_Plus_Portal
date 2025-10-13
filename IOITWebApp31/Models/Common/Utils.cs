using IOITWebApp31.Models;
using IOITWebApp31.Models.Data;
using IOITWebApp31.Models.EF;
using log4net;
using NPOI.SS.UserModel;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace IOITWebApp31
{
    public class Utils
    {
        private static readonly ILog log = LogMaster.GetLogger("utils", "utils");

        public static bool sendEmail(Config config, OrderWebDTO order, string url_temp, string subject, int type, string name, string email)
        {
            try
            {
                //email body
                string url = Directory.GetCurrentDirectory();
                string email_sent = config.EmailUserName;
                string email_name = config.EmailSender;
                string email_host = config.EmailHost;
                int email_port = (int)config.EmailPort;
                string email_pass = config.EmailPasswordHash;
                //string email_domain = config.Website;
                string urlTemp = url + "/wwwroot/template/email/" + url_temp;
                //Sinh động danh sách sản phẩm
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"

                //Lấy trạng thái đơn hàng
                string orderStatus = "";
                string titleStatus = "";
                if (order.OrderStatusId == (int)Const.OrderStatus.INIT)
                {
                    orderStatus = "Tạo mới";
                    titleStatus = "Ðơn hàng của Quý khách hiện đã được tiếp nhận và đang trong quá trình xử lý.";
                }
                else if (order.OrderStatusId == (int)Const.OrderStatus.CONFIRM)
                {
                    orderStatus = "Đã xác nhận";
                    titleStatus = "Ðơn hàng của Quý khách hiện đã được xác nhận và đang trong quá trình xử lý giao hàng.";
                }
                else if (order.OrderStatusId == (int)Const.OrderStatus.DELIVERY)
                {
                    orderStatus = "Đang giao hàng";
                    titleStatus = "Ðơn hàng của Quý khách hiện đang trong quá trình giao hàng.";
                }
                else if (order.OrderStatusId == (int)Const.OrderStatus.DELIVED)
                {
                    orderStatus = "Hoàn thành";
                    titleStatus = "Ðơn hàng của Quý khách đã hoàn thành.";
                }
                else if (order.OrderStatusId == (int)Const.OrderStatus.ORDER_RETURNED)
                {
                    orderStatus = "Hủy";
                    titleStatus = "Ðơn hàng của Quý khách đã bị hủy.";
                }
                else
                {
                    orderStatus = "Tạo mới";
                    titleStatus = "Ðơn hàng của Quý khách hiện đã được tiếp nhận và đang trong quá trình xử lý.";
                }

                string listProduct = "";
                foreach (var item in order.listOrderItem.Where(e => e.Status != (int)Const.Status.DELETED))
                {
                    string price = double.Parse(item.Price.ToString()).ToString("#,###", cul.NumberFormat);
                    string priceTotal = double.Parse(item.PriceTotal.ToString()).ToString("#,###", cul.NumberFormat);
                    listProduct += "<tr><td style=\" width:35%; padding: 5px;\">" +
                    "<a href='" + config.Website + "/chi-tiet-san-pham/" + item.ProductUrl + "-1-" + item.ProductId + ".html' style=\"color: #005999; text-decoration: none;\"" +
                    " title=''>" + item.ProductName + "</a></td>" +
                    "<td style=\"padding: 5px;\"> " + orderStatus + " </td>" +
                    "<td style=\"padding: 5px;\"> " + item.Quantity + " </td>" +
                    "<td style=\"padding: 5px;\"> " + price + "đ </td>" +
                    "<td style=\"padding: 5px; text-align:right;\"> " + priceTotal + "đ </td>" +
                    "</tr> ";
                }
                //Thông tin tài khoản với tài khoản mới tạo
                string account = "";
                if (order.PassHash != null && order.PassHash != "")
                {
                    account += "<tr> " +
                                "<td style=\"padding-bottom: 10px; font-weight:bold; width:40%\"> Tài khoản </td>" +
                                 "<td style=\"padding-bottom: 10px; font-weight:bold;\">" + email + "</td>" +
                              "</tr>" +
                              "<tr>" +
                                "<td style=\"padding-bottom: 10px; font-weight:bold; width:40%\"> Mật khẩu </td>" +
                                 "<td style=\"padding-bottom: 10px; font-weight:bold;\">" + order.PassHash + "</td>" +
                              "</tr>";
                }
                //Hình thức thanh toán
                string paymentMethod = "";
                if (order.PaymentMethodId == (int)Const.PaymentMethod.COD)
                {
                    paymentMethod = "Thanh toán trực tiếp khi giao hàng";
                }
                else if (order.PaymentMethodId == (int)Const.PaymentMethod.ONEPAY_IN)
                {
                    paymentMethod = "Thanh toán bằng thẻ ATM";
                }
                if (order.PaymentMethodId == (int)Const.PaymentMethod.ONEPAY_OUT)
                {
                    paymentMethod = "Thanh toán bằng thẻ thanh toán quốc tế";
                }
                //Trạng thái thanh toán
                string paymentStatus = "";
                if (order.PaymentStatusId == (int)Const.PaymentStatus.INIT)
                {
                    paymentStatus = "Chưa thanh toán";
                }
                else if (order.PaymentStatusId == (int)Const.PaymentStatus.NOT_ENOUGH)
                {
                    paymentStatus = "Chưa thanh toán hết";
                }
                else if (order.PaymentStatusId == (int)Const.PaymentStatus.FULL)
                {
                    paymentStatus = "Hoàn tất thanh toán";
                }
                else if (order.PaymentStatusId == (int)Const.PaymentStatus.ERROR_PAYMENT)
                {
                    paymentStatus = "Thanh toán không thành công";
                }
                else if (order.PaymentStatusId == (int)Const.PaymentStatus.NOT_PAYMENT)
                {
                    paymentStatus = "Không thanh toán";
                }
                else
                {
                    paymentStatus = "Chưa thanh toán";
                }
                string linkCheckOrder = config.Website + "theo-doi-don-hang.html";

                string OrderTotal = order.OrderTotal != 0 ? double.Parse(order.OrderTotal.ToString()).ToString("#,###", cul.NumberFormat) : "0";
                string OrderDelivery = order.OrderDelivery != 0 ? double.Parse(order.OrderDelivery.ToString()).ToString("#,###", cul.NumberFormat) : "0";
                string OrderDiscount = order.OrderDiscount != 0 ? double.Parse(order.OrderDiscount.ToString()).ToString("#,###", cul.NumberFormat) : "0";
                string OrderPrice = (order.OrderTotal + order.OrderDelivery - order.OrderDiscount) != 0 ? double.Parse((order.OrderTotal + order.OrderDelivery - order.OrderDiscount).ToString()).ToString("#,###", cul.NumberFormat) : "0";
                string OrderPaid = order.OrderPaid != 0 ? double.Parse(order.OrderPaid.ToString()).ToString("#,###", cul.NumberFormat) : "0";
                string OrderPricePaid = (order.OrderTotal + order.OrderDelivery - order.OrderDiscount - order.OrderPaid) != 0 ? double.Parse((order.OrderTotal + order.OrderDelivery - order.OrderDiscount - order.OrderPaid).ToString()).ToString("#,###", cul.NumberFormat) : "0";

                String sBody = "";
                sBody = File.ReadAllText(urlTemp);
                if (type == 1)
                    sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                        config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                        config.Website, order.Code, order.FullName, order.CreatedAt.Value.ToString("dd/MM/yyyy"),
                        order.FullName, order.Phone, order.Address, listProduct,
                        OrderTotal, OrderDelivery, OrderDiscount,
                        OrderPrice, OrderPaid, OrderPricePaid,
                        paymentMethod, paymentStatus, linkCheckOrder, account);
                else if (type == 2)
                    sBody = String.Format(sBody, config.EmailColorHeader, config.EmailLogo, config.EmailColorBody,
                        config.EmailColorFooter, config.EmailDisplayName, config.Address, config.Phone,
                        config.Website, order.Code, order.FullName, order.CreatedAt.Value.ToString("dd/MM/yyyy"),
                        orderStatus, listProduct, OrderTotal, OrderDelivery,
                        OrderDiscount, OrderPrice, OrderPaid, OrderPricePaid,
                        paymentMethod, paymentStatus, titleStatus);

                var fromAddress = new MailAddress(email_sent, email_name);
                var toAddress = new MailAddress(email, name);

                using (var smtp = new SmtpClient
                {
                    Host = email_host,
                    Port = (int)email_port,
                    Timeout = 30000,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, email_pass)
                })
                {
                    using (var message = new MailMessage(fromAddress, toAddress))
                    {
                        message.IsBodyHtml = true;
                        message.Subject = subject;
                        message.Body = sBody;
                        try
                        {
                            smtp.Send(message);
                            message.Dispose();
                            smtp.Dispose();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            log.Info(ex.Message + "-" + ex.StackTrace);
                            return false;
                        }
                        finally
                        {
                            message.Dispose();
                            smtp.Dispose();
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                log.Info("send email error : " + ex.Message + "-" + ex.StackTrace);
                return false;
            }
        }

        public static string TextToHtml(string text)
        {
            //text = WebUtility.HtmlEncode(text);
            //text = text.Replace("\\\"", "\"");
            text = text.Replace("\r\n", "\r");
            text = text.Replace("\n", "\r");
            text = text.Replace("\r", "");
            text = text.Replace("\n", "");
            //text = text.Replace("\r", "</br>");
            //text = text.Replace("\n", "</br>");
            text = text.Replace("  ", " &nbsp;");
            return text;
        }

        public static int RandomNumberRank(int minimum, int maximum)
        {
            Random random = new Random();
            return random.Next(minimum, maximum);
        }

        public static string findDayOfWeek(string day)
        {
            string str = "";
            if (day == "Monday")
                str = "Thứ Hai";
            else if (day == "Tuesday")
                str = "Thứ Ba";
            else if (day == "Wednesday")
                str = "Thứ Tư";
            else if (day == "Thursday")
                str = "Thứ Năm";
            else if (day == "Friday")
                str = "Thứ Sau";
            else if (day == "Saturday")
                str = "Thứ Bảy";
            else if (day == "Sunday")
                str = "Chủ Nhật";
            return str;
        }

        public static string getCellValue(ICell cell)
        {
            //Lấy giá trị trong cell
            string str = "";
            if (cell.CellType.ToString().Equals("String"))
                str = cell.StringCellValue;
            else if (cell.CellType.ToString().Equals("Numeric"))
                str = cell.NumericCellValue + "";
            else if (cell.CellType.ToString().Equals("Formula"))
                str = cell.NumericCellValue + "";
            else if (cell.CellType.ToString().Equals("Boolean"))
                str = cell.BooleanCellValue + "";
            else if (cell.CellType.ToString().Equals("Date"))
                str = cell.DateCellValue + "";
            else if (cell.CellType.ToString().Equals("Error"))
                str = cell.ErrorCellValue + "";
            else if (cell.CellType.ToString().Equals("RichString"))
                str = cell.RichStringCellValue + "";

            return str;
        }

        public static string NonUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ",
                "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                "í","ì","ỉ","ĩ","ị",
                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "d",
                "e","e","e","e","e","e","e","e","e","e","e",
                "i","i","i","i","i",
                "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                "u","u","u","u","u","u","u","u","u","u","u",
                "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }

            return Regex.Replace(text.ToLower().Replace(@"'", String.Empty), @"[^\w]+", "-").Replace("\"", "-").Replace(":", "-").ToLower();
        }
        //public static string getCellValue(ICell cell)
        //{
        //    //Lấy giá trị trong cell
        //    string str = "";
        //    if (cell.CellType.ToString().Equals("String"))
        //        str = cell.StringCellValue;
        //    else if (cell.CellType.ToString().Equals("Numeric"))
        //        str = cell.NumericCellValue + "";
        //    else if (cell.CellType.ToString().Equals("Formula"))
        //        str = cell.NumericCellValue + "";
        //    else if (cell.CellType.ToString().Equals("Boolean"))
        //        str = cell.BooleanCellValue + "";
        //    else if (cell.CellType.ToString().Equals("Date"))
        //        str = cell.DateCellValue + "";
        //    else if (cell.CellType.ToString().Equals("Error"))
        //        str = cell.ErrorCellValue + "";
        //    else if (cell.CellType.ToString().Equals("RichString"))
        //        str = cell.RichStringCellValue + "";

        //    return str;
        //}

        public static string ColumnAdress(int col)
        {
            if (col <= 26)
            {
                return Convert.ToChar(col + 64).ToString();
            }
            int div = col / 26;
            int mod = col % 26;
            if (mod == 0) { mod = 26; div--; }
            return ColumnAdress(div) + ColumnAdress(mod);
        }

        public static int GetWeekOrderInYear(DateTime time)
        {
            CultureInfo myCI = CultureInfo.CurrentCulture;
            Calendar myCal = myCI.Calendar;
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            return myCal.GetWeekOfYear(time, myCWR, myFirstDOW);
        }

        public static string ConvertRomanNumber(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("Value must be between 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ConvertRomanNumber(number - 1000);
            if (number >= 900) return "CM" + ConvertRomanNumber(number - 900); //EDIT: i've typed 400 instead 900
            if (number >= 500) return "D" + ConvertRomanNumber(number - 500);
            if (number >= 400) return "CD" + ConvertRomanNumber(number - 400);
            if (number >= 100) return "C" + ConvertRomanNumber(number - 100);
            if (number >= 90) return "XC" + ConvertRomanNumber(number - 90);
            if (number >= 50) return "L" + ConvertRomanNumber(number - 50);
            if (number >= 40) return "XL" + ConvertRomanNumber(number - 40);
            if (number >= 10) return "X" + ConvertRomanNumber(number - 10);
            if (number >= 9) return "IX" + ConvertRomanNumber(number - 9);
            if (number >= 5) return "V" + ConvertRomanNumber(number - 5);
            if (number >= 4) return "IV" + ConvertRomanNumber(number - 4);
            if (number >= 1) return "I" + ConvertRomanNumber(number - 1);
            throw new ArgumentOutOfRangeException("Value must be between 1 and 3999");
        }

        public static int getQuarter(int month)
        {
            int quarter = 1;
            if (month >= 1 && month <= 3)
                quarter = 1;
            if (month >= 4 && month <= 6)
                quarter = 2;
            if (month >= 7 && month <= 9)
                quarter = 3;
            if (month >= 10 && month <= 12)
                quarter = 4;
            return quarter;
        }

        private static readonly string[] VietnameseSigns = new string[]

        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"

        };

        public static string unsignString(string str)
        {
            try
            {
                for (int i = 1; i < VietnameseSigns.Length; i++)
                {

                    for (int j = 0; j < VietnameseSigns[i].Length; j++)

                        str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

                }
                Regex r = new Regex("(?:[^a-z0-9 @]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                return r.Replace(str, String.Empty).ToLower();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string unsignString2(string str)
        {
            try
            {
                for (int i = 1; i < VietnameseSigns.Length; i++)
                {

                    for (int j = 0; j < VietnameseSigns[i].Length; j++)

                        str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

                }
                Regex r = new Regex("(?:[^a-z0-9 ,@]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                return r.Replace(str, String.Empty).ToLower();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime UnixTimeStampMilisecondToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static long DateTimeToUnixTimeStamp(DateTime date)
        {
            try
            {
                return (long)date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            }
            catch (Exception ex)
            {
                return (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            }
        }

        public static void createFile(string fileName, byte[] input, string file_type)
        {
            //extract path
            int lastIndex = fileName.LastIndexOf("\\");
            string path = fileName.Substring(0, lastIndex);
            Directory.CreateDirectory(path);
            MemoryStream ms = new MemoryStream(input);
            File.WriteAllBytes(fileName, input);
            ms.Dispose();
        }

        //private static RotateFlipType GetOrientationToFlipType(int orientationValue)
        //{
        //    RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;

        //    switch (orientationValue)
        //    {
        //        case 1:
        //            rotateFlipType = RotateFlipType.RotateNoneFlipNone;
        //            break;
        //        case 2:
        //            rotateFlipType = RotateFlipType.RotateNoneFlipX;
        //            break;
        //        case 3:
        //            rotateFlipType = RotateFlipType.Rotate180FlipNone;
        //            break;
        //        case 4:
        //            rotateFlipType = RotateFlipType.Rotate180FlipX;
        //            break;
        //        case 5:
        //            rotateFlipType = RotateFlipType.Rotate90FlipX;
        //            break;
        //        case 6:
        //            rotateFlipType = RotateFlipType.Rotate90FlipNone;
        //            break;
        //        case 7:
        //            rotateFlipType = RotateFlipType.Rotate270FlipX;
        //            break;
        //        case 8:
        //            rotateFlipType = RotateFlipType.Rotate270FlipNone;
        //            break;
        //        default:
        //            rotateFlipType = RotateFlipType.RotateNoneFlipNone;
        //            break;
        //    }

        //    return rotateFlipType;
        //}

        public static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomNumberString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string ConvertUrlpath(string text)
        {
            string res = "";
            if (text != null)
            {
                res = text;
                res = res.Replace("%2e%2e%2f", "../");
                res = res.Replace("%2e%2e", "../");
                res = res.Replace("..%2f", "../");
                res = res.Replace("%2e%2e%5c", "..\\");
                res = res.Replace("%2e%2e\\", "..\\");
                res = res.Replace("..%5c", "..\\");
                res = res.Replace("%252e%252e%255c", "..\\");
                res = res.Replace("..%255c", "..\\");

            }

            return res;
        }

        public static DateTime ConvertStringToDate(string strDate)
        {
            DateTime dateTime = new DateTime(1890, 1, 1);
            try
            {
                dateTime = DateTime.FromOADate(Double.Parse(strDate));
            }
            catch
            {
                try
                {
                    //dateTime = DateTime.Parse(strDate);
                    dateTime = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    try
                    {
                        string[] strT = strDate.Split(' ');
                        if (strT.Length > 1)
                        {
                            string[] str1 = strT[0].Split('/');
                            string[] str2 = strT[1].Split(':');
                            if (str1.Length == 3)
                            {
                                dateTime = new DateTime(int.Parse(str1[2]), int.Parse(str1[1]), int.Parse(str1[0]), int.Parse(str2[0]), int.Parse(str2[1]), int.Parse(str2[2]));
                            }

                        }
                        else
                        {
                            string[] str = strDate.Split('/');
                            if (str.Length == 3)
                            {
                                dateTime = new DateTime(int.Parse(str[2]), int.Parse(str[1]), int.Parse(str[0]));
                            }
                        }
                    }
                    catch
                    {
                        string[] str = strDate.Split('/');
                        if (str.Length == 3)
                        {
                            dateTime = new DateTime(int.Parse(str[2]), int.Parse(str[1]), int.Parse(str[0]));
                        }
                    }
                }
            }


            return dateTime;
        }

        //public static string GenCodeOrder(int customerId)
        //{
        //    using (var db = new IOITDataContext())
        //    {
        //        int count = db.Order.Where(o => o.CustomerId == customerId).Count();
        //        string code = "HD"+ count;
        //        int num = 18 - code.Length;
        //        code += RandomNumberString(num).Trim().ToUpper();
        //        while (db.Order.Where(e => e.Code.Trim().Equals(code)).FirstOrDefault() != null)
        //        {
        //            code = RandomNumberString(num).Trim().ToUpper();
        //        }
        //        return code;

        //    }
        //}

        //public static string GenCodeOrder(int id)
        //{
        //    string str = "HD";
        //    int count = id.ToString().Length;
        //    int rank = count > 5 ? 10 : 5;

        //    for(int i = 0; i < rank - count; i ++)
        //    {
        //        str += "0";
        //    }

        //    str += id;
        //    return str;
        //}

    }
}