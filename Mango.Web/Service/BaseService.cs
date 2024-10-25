﻿using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Mango.Web.Utils.SD;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseDTO> SendAsync(RequestDTO requestDTO)
        {
            HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            //token
            message.RequestUri = new Uri(requestDTO.Url);
            if (requestDTO.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
            }



            switch (requestDTO.ApiType)
            {
                case ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;

                case ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;

                case ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            HttpResponseMessage? apiResponse = await client.SendAsync(message);

            try
            {
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new ResponseDTO { Message = "Not Found", IsSuccess = false };
                    case HttpStatusCode.Forbidden:
                        return new ResponseDTO { Message = "Access Denied", IsSuccess = false };
                    case HttpStatusCode.Unauthorized:
                        return new ResponseDTO { Message = "Unauthorized", IsSuccess = false };
                    case HttpStatusCode.InternalServerError:
                        return new ResponseDTO { Message = "Internal Server Error", IsSuccess = false };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var responseDTO = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                        return responseDTO;
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Message = ex.Message, IsSuccess = false };
            }
        }
    }
}