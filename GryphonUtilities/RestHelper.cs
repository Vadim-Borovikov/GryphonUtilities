using JetBrains.Annotations;
using RestSharp;

namespace GryphonUtilities;

[PublicAPI]
public static class RestHelper
{
    public static Task<T> CallGetMethodAsync<T>(string apiProvider, string method)
    {
        return CallMethodAsync<T>(apiProvider, method);
    }

    public static Task<T> CallPostMethodAsync<T>(string apiProvider, string method)
    {
        return CallMethodAsync<T>(apiProvider, method, true);
    }

    private static async Task<T> CallMethodAsync<T>(string apiProvider, string method, bool post = false)
    {
        using (RestClient client = new(apiProvider))
        {
            RestRequest request = new(method);
            T? result = post ? await client.PostAsync<T>(request) : await client.GetAsync<T>(request);
            string methodType = post ? "POST" : "GET";
            return result.GetValue($"REST {methodType} method returned null");
        }
    }
}