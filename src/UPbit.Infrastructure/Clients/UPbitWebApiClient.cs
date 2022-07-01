using Dreamrosia.Koin.Shared.Wrapper;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    public class UPbitWebApiClient<TParameter> : WebApiClient, IUPbitWebApiClient<TParameter> where TParameter : IWebApiParameter
    {
        public int PerSecondLimted { get; protected set; }

        public int PerMinuteLimted { get; protected set; }

        protected string ApiUrl => "https://api.upbit.com/v1";

        public TParameter Parameter { get; protected set; }

        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

        protected void SetAuthenticationHeader(string token)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected string AuthorizationToken(string query = null)
        {
            string queryHash = string.Empty;

            if (!string.IsNullOrEmpty(query))
            {
                SHA512 sha512 = SHA512.Create();
                byte[] queryHashByteArray = sha512.ComputeHash(Encoding.UTF8.GetBytes(query));
                queryHash = BitConverter.ToString(queryHashByteArray).Replace("-", "").ToLower();
            }

            var access_key = ExchangeClientKeys.AccessKey;
            var secret_key = ExchangeClientKeys.SecretKey;

            var payload = new JwtPayload
            {
                { "access_key", access_key },
                { "nonce", Guid.NewGuid().ToString() },
                { "query_hash", queryHash },
                { "query_hash_alg", "SHA512" }
            };

            byte[] keyBytes = Encoding.Default.GetBytes(secret_key);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
            var header = new JwtHeader(credentials);

            var securityToken = new JwtSecurityToken(header, payload);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

            //return string.Format("Bearer {0}", jwtToken);
            return jwtToken;
        }

        private string TooManyApiRequests => "Too many API requests";

        protected override T Deserialize<T>(StreamReader stream)
        {
            var body = stream.ReadToEnd();

            if (body.Contains(TooManyApiRequests))
            {
                throw new Exception(TooManyApiRequests);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(body, jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex, body);

                throw;
            }
        }

        protected override async Task<IResult<TResult>> GetResult<TResult>(HttpResponseMessage response)
        {
            using var stream = await response.Content.ReadAsStreamAsync();

            var content = response.Content;
            var charset = content.Headers.ContentType.CharSet;

            Encoding encoding = Encoding.GetEncoding(charset is null ? "UTF-8" : charset);

            StreamReader reader;

            if (content.Headers
                       .ContentEncoding
                       .SingleOrDefault(f => f.ToLower().Equals("gzip")) is null)
            {
                reader = new StreamReader(stream, encoding);
            }
            else
            {
                var gzip = new GZipStream(stream, CompressionMode.Decompress);

                reader = new StreamReader(gzip, encoding);
            }

            if (response.IsSuccessStatusCode)
            {
                return await Result<TResult>.SuccessAsync(Deserialize<TResult>(reader));

            }
            else if (400 <= (int)response.StatusCode && (int)response.StatusCode < 500)
            {
                var error = Deserialize<ErrorRoot>(reader)?.Error;

                return await Result<TResult>.FailAsync(error?.Name, error?.Message);
            }
            else
            {
                return await Result<TResult>.FailAsync(response.StatusCode.ToString(), response.StatusCode.ToString());
            }
        }

        protected override async Task<IResult<TResult>> GetAsync<TResult>()
        {
            try
            {
                Parameter = (TParameter)Activator.CreateInstance(typeof(TParameter));

                if (Parameter is not null) { SetHeadersAndURI(); }

                RequestCounter.IncrementCounter(this);

                return await base.GetAsync<TResult>();
            }
            catch (Exception ex)
            {
                return await Result<TResult>.FailAsync(ex.Message);
            }
        }

        protected override async Task<IResult<TResult>> GetAsync<TResult>(object parameter)
        {
            try
            {
                Parameter = (TParameter)parameter;

                if (Parameter is not null) { SetHeadersAndURI(); }

                RequestCounter.IncrementCounter(this);

                var response = await Client.GetAsync(URI);

                return await GetResult<TResult>(response);
            }
            catch (Exception ex)
            {
                return await Result<TResult>.FailAsync(ex.Message);
            }
        }

        protected override async Task<IResult<TResult>> PostAsync<TResult>(object parameter)
        {
            try
            {
                Parameter = (TParameter)parameter;

                if (Parameter is not null) { SetHeadersAndURI(); }

#if DEBUG
                var json = Serialize(parameter);
#endif

                var content = new StringContent(Serialize(parameter))
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                };

                RequestCounter.IncrementCounter(this);

                var response = await Client.PostAsync(URI, content);

                return await GetResult<TResult>(response);
            }
            catch (Exception ex)
            {
                return await Result<TResult>.FailAsync(ex.Message);
            }
        }

        protected override async Task<IResult<TResult>> DeleteAsync<TResult>(object parameter)
        {
            try
            {
                Parameter = (TParameter)parameter;

                if (Parameter is not null) { SetHeadersAndURI(); }

                RequestCounter.IncrementCounter(this);

                var response = await Client.DeleteAsync(URI);

                return await GetResult<TResult>(response);
            }
            catch (Exception ex)
            {
                return await Result<TResult>.FailAsync(ex.Message);
            }
        }
    }
}
