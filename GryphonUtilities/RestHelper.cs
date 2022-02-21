using JetBrains.Annotations;
using RestSharp;

namespace GryphonUtilities;

[PublicAPI]
public static class RestHelper
{
    public static Task<TResponse> CallGetMethodAsync<TResponse>(string baseUrl, string? resource,
        string? headerName = null, string? headerValue = null, IDictionary<string, string?>? queryParameters = null)
    {
        return CallMethodAsync<string, TResponse>(baseUrl, resource, headerName, headerValue, queryParameters);
    }

    public static Task<TResponse> CallPostMethodAsync<TRequest, TResponse>(string baseUrl, string? resource,
        string? headerName = null, string? headerValue = null, TRequest? obj = null)
        where TRequest : class
    {
        return CallMethodAsync<TRequest, TResponse>(baseUrl, resource, headerName, headerValue, null, obj,
            Method.Post);
    }

    private static async Task<TResponse> CallMethodAsync<TRequest, TResponse>(string baseUrl, string? resource,
        string? headerName = null, string? headerValue = null, IDictionary<string, string?>? queryParameters = null,
        TRequest? obj = null, Method method = Method.Get)
        where TRequest : class
    {
        using (RestClient client = new(baseUrl))
        {
            RestRequest request = new(resource, method);
            if (headerName is not null && headerValue is not null)
            {
                request.AddHeader(headerName, headerValue);
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