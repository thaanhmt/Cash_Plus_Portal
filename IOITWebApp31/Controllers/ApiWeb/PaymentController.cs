using IOITWebApp31.Models;
using IOITWebApp31.Models.EF;
using IOITWebApp31.Models.Payment;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace IOITWebApp31.Controllers.ApiWeb
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private static readonly ILog log = LogMaster.GetLogger("payment", "payment");

        [HttpGet("getHashKey")]
        public async Task<ActionResult> GetHashKey()
        {
            string test = "https://mtf.onepay.vn/vpcpay/vpcpay.op?AgainLink=Onepay.vn&Title=DUYTEST&vpc_AccessCode=6BEB2546&vpc_Amount=300000000&vpc_Command=pay&vpc_Locale=vn&vpc_MerchTxnRef=5521235456456&vpc_Merchant=TESTONEPAY&vpc_OrderInfo=DUYNQTEST&vpc_ReturnURL=http://sis.ou.edu.vn/payment/bidvonepay/cq/tthp&vpc_TicketNo=192.168.35.3&vpc_Version=2&vpc_SecureHash=32149F60AD77329B22196AD84CF3C3ABAEE9143ACCD68E7479950366281F7798";
            string test1 = "https://mtf.onepay.vn/vpcpay/vpcpay.op?AgainLink=evisavietnam.org.vn&Title=EVISA VIET NAM&vpc_AccessCode=6BEB2546&vpc_Amount=300000000&vpc_Command=pay&vpc_Locale=vn&vpc_MerchTxnRef=E191230AU12312339&vpc_Merchant=TESTONEPAY&vpc_OrderInfo=EVS655DBXPEFTQU&vpc_ReturnURL=https://evisavietnam.org.vn/apply-evisa.html&vpc_TicketNo=171.255.72.78&vpc_Version=2&vpc_SecureHash=6a3b4452798818dd0d7b3d4dd53d2793b42807c8568349a993ceca57f5a9905d";

            DefaultResponse def = new DefaultResponse();
            string key = "6D0870CDE5F24F34F3915FB0045120DB";
            string messege = "vpc_AccessCode=6BEB2546&vpc_Amount=300000000&vpc_Command=pay&vpc_Locale=vn&vpc_MerchTxnRef=E191230AU12312339&vpc_Merchant=TESTONEPAY&vpc_OrderInfo=EVS655DBXPEFTQU&vpc_ReturnURL=https://evisavietnam.org.vn/apply-evisa.html&vpc_TicketNo=171.255.72.78&vpc_Version=2";
            def.data = Security.HashHMACHex(key, messege);
            def.meta = new Meta(200, "Success");
            return Ok(def);

        }

        [HttpGet("ipn")]
        public async Task<IActionResult> ResuftIPN([FromQuery] PaymentResponse query)
        {
            string response = "";
            //IPN
            using (var db = new IOITDataContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        log.Info("vpc_Amount:" + query.vpc_Amount);
                        log.Info("vpc_Card:" + query.vpc_Card);
                        log.Info("vpc_CardUid:" + query.vpc_CardUid);
                        log.Info("vpc_Command:" + query.vpc_Command);
                        log.Info("vpc_CurrencyCode:" + query.vpc_CurrencyCode);
                        log.Info("vpc_Locale:" + query.vpc_Locale);
                        log.Info("vpc_Merchant:" + query.vpc_Merchant);
                        log.Info("vpc_MerchTxnRef:" + query.vpc_MerchTxnRef);
                        log.Info("vpc_Message:" + query.vpc_Message);
                        log.Info("vpc_OrderInfo:" + query.vpc_OrderInfo);
                        log.Info("vpc_PayChannel:" + query.vpc_PayChannel);
                        log.Info("vpc_SecureHash:" + query.vpc_SecureHash);
                        log.Info("vpc_TransactionNo:" + query.vpc_TransactionNo);
                        log.Info("vpc_TxnResponseCode:" + query.vpc_TxnResponseCode);
                        log.Info("------------------------");
                        //check tính toàn vẹn dữ liệu
                        string id = query.vpc_MerchTxnRef;
                        var orderVisa = db.Order.Where(e => e.Code.Trim().ToUpper().Equals(id.Trim().ToUpper()) && e.Status != (int)Const.Status.DELETED).FirstOrDefault();
                        if (orderVisa.OrderId > 0)
                        {
                            if (orderVisa.PaymentStatusId != (int)Const.PaymentStatus.FULL)
                            {
                                var paymentHistory = await db.PaymentHistory.Where(e => e.PaymentHistoryId.ToString() == query.vpc_MerchTxnRef && e.OrderInfo == query.vpc_OrderInfo
                                    && e.Status != (int)Const.Status.DELETED && e.TransactionNo != "0").FirstOrDefaultAsync();
                                if (paymentHistory != null)
                                {
                                    paymentHistory.TxnResponseCode = query.vpc_TxnResponseCode;
                                    paymentHistory.TransactionNo = query.vpc_TransactionNo;
                                    paymentHistory.Message = query.vpc_Message;
                                    paymentHistory.Card = query.vpc_Card;
                                    paymentHistory.PayChannel = query.vpc_PayChannel;
                                    paymentHistory.CardUid = query.vpc_CardUid;
                                    paymentHistory.UpdatedAt = DateTime.Now;
                                    db.PaymentHistory.Update(paymentHistory);
                                    await db.SaveChangesAsync();

                                    if (query.vpc_TransactionNo == "0")
                                    {
                                        if (query.vpc_Amount == paymentHistory.Amount + "00")
                                        {
                                            orderVisa.PaymentStatusId = (int)Const.PaymentStatus.FULL;
                                        }
                                        else
                                        {
                                            orderVisa.PaymentStatusId = (int)Const.PaymentStatus.NOT_ENOUGH;
                                        }
                                    }
                                    else
                                    {
                                        orderVisa.PaymentStatusId = (int)Const.PaymentStatus.ERROR_PAYMENT;
                                    }

                                    orderVisa.PaymentMethodId = paymentHistory.PayType;
                                    orderVisa.UpdatedAt = DateTime.Now;
                                    db.Order.Update(orderVisa);
                                    await db.SaveChangesAsync();

                                    transaction.Commit();
                                    response = "responsecode=1&desc=confirm-success";
                                    return Ok(response);
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response = "responsecode=1&desc=confirm-success";
                                    return Ok(response);
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                                response = "responsecode=1&desc=confirm-success";
                                return Ok(response);
                            }
                        }
                        else
                        {
                            transaction.Rollback();
                            response = "responsecode=0&desc=confirm-not-success";
                            return Ok(response);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        response = "responsecode=0&desc=confirm-not-success";
                        return Ok(response);
                    }
                }
            }
        }
    }
}