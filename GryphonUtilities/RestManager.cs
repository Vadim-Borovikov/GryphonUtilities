using System.Text.Json;
using JetBrains.Annotations;
using RestSharp;
using RestSharp.Serializers.Json;

namespace GryphonUtilities;

[PublicAPI]
public class RestManager<TRequest, TResponse> : IDisposable where TRequest : class
{
    public static Task<TResponse> GetAsync(string baseUrl, string? resource,
        IDictionary<string, string>? headerParameters = null, IDictionary<string, string?>? queryParameters = null,
        JsonSerializerOptions? options = null)
    {
        using (RestManager<TRequest, TResponse> client =
               new(baseUrl, resource, Method.Get, headerParameters, queryParameters, options:options))
        {
            return client.RunAsync();
        }
    }

    public static Task<TResponse> PostAsync(string baseUrl, string? resource,
        IDictionary<string, string>? headerParameters = null, TRequest? obj = null,
        JsonSerializerOptions? options = null)
    {
        using (RestManager<TRequest, TResponse> client =
               new(baseUrl, resource, Method.Post, headerParameters, obj: obj, options:options))
        {
            return client.RunAsync();
        }
    }

    public RestManager(string baseUrl, string? resource, Method method,
        IDictionary<string, string>? headerParameters = null, IDictionary<string, string?>? queryParameters = null,
        TRequest? obj = null, JsonSerializerOptions? options = null)
    {
        _client = new RestClient(baseUrl);
        if (options is not null)
        {
            _client.UseSystemTextJson(options);
        }
        _request = new RestRequest(resource, method);
        if (headerParameters is not null)
        {
            foreach (string name in headerParameters.Keys)
            {
                _request.AddHeader(name, headerParameters[name]);
            }
        }
        if (queryParameters is not null)
        {
            foreach (string name in queryParameters.Keys)
            {
                _request.AddQueryParameter(name, queryParameters[name]);
            }
        }
        if (obj is not null)
        {
            _request.AddJsonBody(obj);
        }
    }

    private async Task<TResponse> RunAsync()
    {
        TResponse? result;
        switch (_request.Method)
        {
            case Method.Get:
                result = await _client.GetAsync<TResponse>(_request);
                return result.GetValue("REST GET method returned null");
            case Method.Post:
                result = await _client.PostAsync<TResponse>(_request);
                return result.GetValue("REST POST method returned null");
            default: throw new ArgumentOutOfRangeException(nameof(_request.Method), _request.Method, null);
        }
    }

    public void Dispose() => _client.Dispose();

    private readonly RestClient _client;
    private readonly RestRequest _request;
}