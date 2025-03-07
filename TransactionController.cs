using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RevindexToCTBC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        [HttpPost("AuthResponse")]
        public IActionResult AuthResponse([FromForm] string URLEnc)
        {
            string key = "lamJTYhT6XoSjFoIZOhPcTGU"; // 24 bytes 金鑰
            Decrypt dec = new Decrypt();
            dec.Key = key;
            dec.EncRes = URLEnc;

            if (dec.LastError == 0)
            {
                return Ok(new
                {
                    Status = dec.Status,
                    AuthCode = dec.AuthCode,
                    Amount = dec.AuthAmt
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
            string logFilePath = @"C:\Users\frank.huang\webapi\log.txt";

            using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
            {
                writer.AutoFlush = true;
                Console.SetOut(writer);
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - 收到轉送請求");
                Console.WriteLine("--------------------------------------------------");
                Console.Out.Flush();
            }

            if (paymentId == Guid.Empty)
            {
                return BadRequest("Invalid PaymentId.");
            }

            try
            {
                // 1️⃣ 設定銀行 URL (測試環境)
                string bankUrl = "https://testepos.ctbcbank.com/mauth/SSLAuthUI.jsp";

                // 2️⃣ 取得交易相關參數（這些數據應該來自你的資料庫）
                string merID = "8220550000148";  
                string terminalID = "91500148"; 
                string orderNo = "20";
                string authAmt = "100"; 
                string txType = "0"; 
                string authResURL = "https://www.uttpro.com:50001/api/Transaction//AuthResHandler"; 
                string key = "lamJTYhT6XoSjFoIZOhPcTGU"; 

                // 3️⃣ 加密參數
                string urlEnc = EncryptData(merID, terminalID, orderNo, authAmt, txType, authResURL, key);

                // 4️⃣ 產生自動提交的 HTML 表單
                string formHtml = $@"
                <html>
                <body onload='document.forms[""bankForm""].submit();'>
                    <form name='bankForm' method='post' action='{bankUrl}'>
                        <input type='hidden' name='merID' value='{merID}'>
                        <input type='hidden' name='URLEnc' value='{urlEnc}'>
                    </form>
                </body>
                </html>";

                // 5️⃣ 記錄交易日誌
                using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
                {
                    writer.AutoFlush = true;
                    Console.SetOut(writer);
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - 轉送至銀行支付頁面: {bankUrl}");
                    Console.WriteLine("--------------------------------------------------");
                    Console.Out.Flush();
                }

                // 6️⃣ 回傳 HTML 讓用戶自動轉跳
                return Content(formHtml, "text/html");
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
                {
                    writer.AutoFlush = true;
                    Console.SetOut(writer);
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - 發生錯誤: {ex.Message}");
                    Console.WriteLine("--------------------------------------------------");
                    Console.Out.Flush();
                }
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 加密交易參數
        /// </summary>
        private string EncryptData(string merID, string terminalID, string orderNo, string authAmt, string txType, string authResURL, string key)
        {
            Encrypt enc = new Encrypt();
            enc.MerchantID = merID;
            enc.TerminalID = terminalID;
            enc.OrderNo = orderNo;
            enc.AuthAmt = authAmt;
            enc.TxType = txType;
            enc.AuthResURL = authResURL;
            enc.Key = key;

            // 執行加密
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
