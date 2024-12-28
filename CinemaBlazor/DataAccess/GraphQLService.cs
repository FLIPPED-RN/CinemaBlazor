using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;

namespace CinemaBlazor.DataAccess
{
    public class GraphQLService
    {
        private readonly GraphQLHttpClient _client;

        public GraphQLService()
        {
            var uri = new Uri("http://localhost:5000/graphql");
            var graphQLOptions = new GraphQLHttpClientOptions
            {
                EndPoint = uri,
                HttpMessageHandler = new HttpClientHandler(),
            };
            _client = new GraphQLHttpClient(graphQLOptions, new NewtonsoftJsonSerializer());
        }

        public async Task<List<T>> ExecuteQueryReturnListAsync<T>(string graphQLQueryType, string completeQueryString)
        {
            var request = new GraphQLRequest
            {
                Query = completeQueryString
            };
            var response = await _client.SendQueryAsync<GraphQLResponse<List<T>>>(request);
            return response.Data.Data;
        }

        public class GraphQLResponse<T>
        {
            public T Data { get; set; }
        }

        public async Task<T> ExecuteQueryAsync<T>(string graphQLQueryType, string completeQueryString)
        {
            try
            {
                var request = new GraphQLRequest
                {
                    Query = completeQueryString
                };
                var response = await _client.SendQueryAsync<object>(request);
                var stringResult = response.Data.ToString();
                stringResult = stringResult!.Replace($"\"{graphQLQueryType}\":", string.Empty);
                stringResult = stringResult.Remove(0, 1);
                stringResult = stringResult.Remove(stringResult.Length - 1, 1);
                var result = JsonConvert.DeserializeObject<T>(stringResult);
                return result!;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<T> ExecuteMutationAsync<T>(string graphQLMutationType, string completeMutationString)
        {
            try
            {
                var request = new GraphQLRequest
                {
                    Query = completeMutationString
                };
                var response = await _client.SendMutationAsync<object>(request);
                var stringResult = response.Data.ToString();
                stringResult = stringResult!.Replace($"\"{graphQLMutationType}\":", string.Empty);
                stringResult = stringResult.Remove(0, 1);
                stringResult = stringResult.Remove(stringResult.Length - 1, 1);
                var result = JsonConvert.DeserializeObject<T>(stringResult);
                return result!;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
