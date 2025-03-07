using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace RevindexToCTBC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private const string BankUrl = "https://testepos.ctbcbank.com/mauth/SSLAuthUI.jsp";
        private const string Key = "lamJTYhT6XoSjFoIZOhPcTGU";
        private const string MerchantID = "8220550000148";
        private const string TerminalID = "91500148";
        private const string AuthResURL = "https://www.uttpro.com:50001/api/Transaction/AuthResHandler";
        private const int DebugMode = 1;
        public TransactionController()
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }


        [HttpPost("AuthResponse")]
        public IActionResult AuthResponse([FromForm] string URLEnc)
        {
            Decrypt dec = new Decrypt();
            dec.Key = Key;
            dec.EncRes = URLEnc;
            dec.Debug = DebugMode;

            if (dec.LastError == 0)
            {
                return Ok(new
                {
                    Status = dec.Status,
                    AuthCode = dec.AuthCode,
                    Amount = dec.AuthAmt,
                    Debug = DebugMode
                });
            }
            else
            {
                return BadRequest("解密錯誤: " + dec.LastError);
            }
        }

        [HttpGet("ProcessTransaction")]
        public async Task<IActionResult> ProcessTransaction([FromQuery] Guid paymentId)
        {
            if (paymentId == Guid.Empty)
            {
                return BadRequest("Invalid PaymentId.");
            }

            try
            {
                // 建立交易參數
                string orderNo = "20";
                string authAmt = "100";
                string txType = "0";
                string inMac = GenerateInMac(orderNo, authAmt, txType);

                string urlEnc = EncryptData(orderNo, authAmt, txType, inMac);

                string formHtml = $@"
                <html>
                <body onload='document.forms[""bankForm""].submit();'>
                    <form name='bankForm' method='post' action='{BankUrl}'>
                        <input type='hidden' name='merID' value='{MerchantID}'>
                        <input type='hidden' name='URLEnc' value='{urlEnc}'>
                        <input type='hidden' name='debug' value='{DebugMode}'>
                    </form>
                </body>
                </html>";

                return Content(formHtml, "text/html");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 產生 InMac (身份驗證壓碼)
        /// </summary>
        private string GenerateInMac(string orderNo, string authAmt, string txType)
        {
            Encrypt enc = new Encrypt();
            enc.MerchantID = MerchantID;
            enc.TerminalID = TerminalID;
            enc.OrderNo = orderNo;
            enc.AuthAmt = authAmt;
            enc.TxType = txType;
            enc.Key = Key;
            enc.Debug = DebugMode; // 設定 Debug 模式
            enc.EncryptData();

            if (enc.LastError == 0)
            {
                return enc.EncodeData;
            }
            else
            {
                throw new Exception($"InMac 產生失敗，錯誤代碼: {enc.LastError}");
            }
        }

        /// <summary>
        /// 加密交易參數
        /// </summary>
        private string EncryptData(string orderNo, string authAmt, string txType, string inMac)
        {
            Encrypt enc = new Encrypt();
            enc.MerchantID = MerchantID;
            enc.TerminalID = TerminalID;
            enc.OrderNo = orderNo;
            enc.AuthAmt = authAmt;
            enc.TxType = txType;
            enc.AuthResURL = AuthResURL;
            enc.InMac = inMac;  
            enc.Key = Key;
            enc.Debug = DebugMode; 
            enc.CardholderName = "Tester";
            enc.Email = "test@gmail.com";
            enc.PhoneCc = "886";
            enc.PhoneSub = "0912345678";
            enc.BillAddrLine1 = "測試市測試區測試路123號10樓";
            enc.BillAddrCountry = "158";
            enc.BillAddrCity = "測試";
            enc.BillAddrPostCode = "12345";
            enc.BillAddrState = "TEST";
            enc.PromoCode = "12345678901234567890";

            enc.EncryptData();

            if (enc.LastError == 0)
            {
                return enc.EncodeData;
            }
            else
            {
                throw new Exception($"加密失敗，錯誤代碼: {enc.LastError}");
            }
        }
    }
}
