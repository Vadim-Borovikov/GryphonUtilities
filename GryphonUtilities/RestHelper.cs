using JetBrains.Annotations;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace GryphonUtilities;

[PublicAPI]
public static class RestHelper
{
    public static Task<TResponse> CallGetMethodAsync<TResponse>(string baseUrl, string? resource,
        IDictionary<string, string>? headerParameters = null, IDictionary<string, string?>? queryParameters = null,
        JsonSerializerSettings? settings = null)
    {
        return CallMethodAsync<string, TResponse>(baseUrl, resource, headerParameters, queryParameters,
            settings: settings);
    }

    public static Task<TResponse> CallPostMethodAsync<TRequest, TResponse>(string baseUrl, string? resource,
        IDictionary<string, string>? headerParameters = null, TRequest? obj = null,
        JsonSerializerSettings? settings = null)
        where TRequest : class
    {
        return CallMethodAsync<TRequest, TResponse>(baseUrl, resource, headerParameters, null, obj,
            Method.Post, settings);
    }

    private static async Task<TResponse> CallMethodAsync<TRequest, TResponse>(string baseUrl, string? resource,
        IDictionary<string, string>? headerParameters = null, IDictionary<string, string?>? queryParameters = null,
        TRequest? obj = null, Method method = Method.Get, JsonSerializerSettings? settings = null)
        where TRequest : class
    {
        using (RestClient client = new(baseUrl))
        {
            if (settings is not null)
            {
                client.UseNewtonsoftJson(settings);
            }
            RestRequest request = new(resource, method);
            if (headerParameters is not null)
            {
                foreach (string name in headerParameters.Keys)
                {
                    request.AddHeader(name, headerParameters[name]);
                }
            }
            if (queryParameters is not null)
            {
                foreach (string name in queryParameters.Keys)
                {
                    request.AddQueryParameter(name, queryParameters[name]);
                }
            }
            if (obj is not null)
            {
                request.AddJsonBody(obj);
            }

            TResponse? result;
            switch (method)
            {
                case Method.Get:
                    result = await client.GetAsync<TResponse>(request);
                    return result.GetValue("REST GET method returned null");
                case Method.Post:
                    result = await client.PostAsync<TResponse>(request);
                    return result.GetValue("REST POST method returned null");
                default: throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }
    }
}