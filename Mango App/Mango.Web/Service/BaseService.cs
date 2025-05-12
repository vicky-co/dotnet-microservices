using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory clientFactory, ITokenProvider tokenProvider)
        {
            _clientFactory = clientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto request, bool withBearer = true)
        {
            try
            {

                HttpClient client = _clientFactory.CreateClient("MangoAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                if (withBearer)
                {
                    message.Headers.Add("Authorization", $"Bearer {_tokenProvider.GetToken()}");
                }

                message.RequestUri = new Uri(request.Url);
                if(request.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage? apiResponse = null;

                switch (request.ApiType)
                {

                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await client.SendAsync(message);
                var rawContent = await apiResponse.Content.ReadAsStringAsync();


                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var deserializedContent = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return deserializedContent;

                }

            }
            catch (Exception ex)
            {

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Message = ex.Message.ToString()
                };

            }

        }
    }
}
