using Microsoft.AspNetCore.Mvc;
using RevindexToCTBC.Models;
using System.Web;

namespace RevindexToCTBC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok("Web API is running!");
        }

        [HttpPost("ProcessPayment")]
        public IActionResult ProcessPayment([FromForm] PaymentRequest request)
        {

            if (request == null || request.Amount <= 0)
            {
                return Content(CreateResponse("Invalid request. Amount must be greater than 0.", null, 200), "application/x-www-form-urlencoded");
            }

            bool isSuccess = true;
            string redirectUrl = isSuccess
                ? "https://testepos.ctbcbank.com/mauth/SSLAuthUI.jsp"
                : null;


            string message = isSuccess ? "Payment processed successfully." : "Payment declined.";
            int status = 1;

            // 返回符合格式的回應
            return Content(CreateResponse(message, redirectUrl, status), "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 創建回應格式
        /// </summary>
        /// <param name="message">回應訊息</param>
        /// <param name="redirectUrl">跳轉網址</param>
        /// <param name="status">狀態碼</param>
        /// <returns>回應內容</returns>
        private string CreateResponse(string message, string redirectUrl, int status)
        {
            // 構建 key-value 格式的字符串
            var responseParts = new List<string>
            {
                $"Message={HttpUtility.UrlEncode(message)}",
                $"PaymentRedirectUrl={HttpUtility.UrlEncode(redirectUrl)}",
                $"Status={status}"
            };

            return string.Join("&", responseParts);
        }
    }
}
