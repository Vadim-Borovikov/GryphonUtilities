using System.Text.Json;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using RestSharp;
using RestSharp.Serializers.Json;

namespace GryphonUtilities;

[PublicAPI]
public sealed class RestManager : IDisposable
{
    public static async Task<RestResponse> GetAsync(string baseUrl, string? resource,
        IDictionary<string, string>? headerParameters = null, IDictionary<string, string?>? queryParameters = null,
        JsonSerializerOptions? options = null)
    {
        using (RestManager client =
               new(baseUrl, resource, Method.Get, headerParameters, queryParameters, options: options))
        {
            // Must await here, otherwise client will be disposed before finish
            RestResponse response = await client.RunAsync();
            return response;
        }
    }

    public static async Task<T> GetAsync<T>(string baseUrl, string? resource,
        IDictionary<string, string>? headerParameters = null, IDictionary<string, string?>? queryParameters = null,
        JsonSerializerOptions? options = null)
    {
        using (RestManager client =
               new(baseUrl, resource, Method.Get, headerParameters, queryParameters, options: options))
        {
            // Must await here, otherwise client will be disposed before finish
            T result = await client.RunAsync<T>();
            return result;
        }
    }

    public static async Task<RestResponse> PostAsync(string baseUrl, string? resource,
        IDictionary<string, string>? headerParameters = null, object? obj = null,
        JsonSerializerOptions? options = null)
    {
        using (RestManager client =
               new(baseUrl, resource, Method.Post, headerParameters, obj: obj, options: options))
        {
            // Must await here, otherwise client will be disposed before finish
            RestResponse response = await client.RunAsync();
            return response;
        }
    }

    public static async Task<T> PostAsync<T>(string baseUrl, string? resource,
        IDictionary<string, string>? headerParameters = null, object? obj = null,
        JsonSerializerOptions? options = null)
    {
        using (RestManager client =
               new(baseUrl, resource, Method.Post, headerParameters, obj: obj, options: options))
        {
            // Must await here, otherwise client will be disposed before finish
            T result = await client.RunAsync<T>();
            return result;
        }
    }

    public RestManager(string baseUrl, string? resource, Method method,
        IDictionary<string, string>? headerParameters = null, IDictionary<string, string?>? queryParameters = null,
        object? obj = null, JsonSerializerOptions? options = null)
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

    private async Task<T> RunAsync<T>()
    {
        T? result;
        switch (_request.Method)
        {
            case Method.Get:
                result = await _client.GetAsync<T>(_request);
                return result.Denull("REST GET method returned null");
            case Method.Post:
                result = await _client.PostAsync<T>(_request);
                return result.Denull("REST POST method returned null");
            default: throw new ArgumentOutOfRangeException(nameof(_request.Method), _request.Method, null);
        }
    }

    private Task<RestResponse> RunAsync()
    {
        return _request.Method switch
        {
            Method.Get  => _client.GetAsync(_request),
            Method.Post => _client.PostAsync(_request),
            _           => throw new ArgumentOutOfRangeException(nameof(_request.Method), _request.Method, null)
        };
    }

    public void Dispose() => _client.Dispose();

    private readonly RestClient _client;
    private readonly RestRequest _request;
}