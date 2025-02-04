using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
namespace RevindexToCTBC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionTestController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ConnectionTestController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestConnection()
        {
            string url = "https://testepos.ctbcbank.com/mauth/SSLAuthUI.jsp"; // 替換為文件中的目標 URL

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new
                    {
                        Success = true,
                        StatusCode = response.StatusCode,
                        Message = "Connection successful."
                    });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        Success = false,
                        StatusCode = response.StatusCode,
                        Message = "Failed to connect."
                    });
                }
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = $"Request failed: {ex.Message}"
                });
            }
        }
    }
}
