using Dreamrosia.Koin.Shared.Wrapper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    public class WebApiClient : IWebApiClient
    {
        protected string URL { get; set; }
        protected string URI { get; set; }

        protected readonly HttpClient Client;

        public WebApiClient()
        {
            Client = new HttpClient();

            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            Client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("ko-KR"));
        }

        protected virtual List<KeyValuePair<string, object>> QueryParameters()
        {
            return new List<KeyValuePair<string, object>>();
        }

        protected virtual string QueryString(object parameter)
        {
            List<string> values = new List<string>();

            List<KeyValuePair<string, object>> keyValues = new List<KeyValuePair<string, object>>();

            foreach (var f in parameter.GetType()
                                       .GetProperties(BindingFlags.GetProperty |
                                                      BindingFlags.Instance |
                                                      BindingFlags.Public))
            {
                var jsonIgnore = f.GetCustomAttributes().OfType<JsonIgnoreAttribute>().SingleOrDefault();

                if (jsonIgnore is not null) { continue; }

                if (Nullable.GetUnderlyingType(f.PropertyType) != null)
                {
                    if (f.GetValue(parameter) is null) { continue; }
                }

                if (f.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(f.PropertyType))
                {
                    var fields = f.GetValue(parameter) as IEnumerable;

                    if (fields is null) { continue; }

                    foreach (var field in fields)
                    {
                        keyValues.Add(new KeyValuePair<string, object>($"{f.Name}[]", field));
                    }
                }
                else
                {
                    string value = Convert.ToString(f.GetValue(parameter));

                    if (!string.IsNullOrEmpty(value))
                    {
                        keyValues.Add(new KeyValuePair<string, object>(f.Name, value));
                    }
                }
            }

            return QueryString(keyValues);
        }

        protected virtual string QueryString(List<KeyValuePair<string, object>> parameters)
        {
            List<string> values = new List<string>();

            foreach (var parameter in parameters)
            {
                values.Add(string.Format("{0}={1}", parameter.Key, parameter.Value));
            }

            return string.Join("&", values.ToArray());
        }

        protected virtual void SetHeadersAndURI() { }

        protected virtual T Deserialize<T>(StreamReader stream)
        {
            var body = stream.ReadToEnd();

            try
            {
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex, body);

                throw;
            }
        }

        protected string Serialize(object parameter)
        {
            return JsonConvert.SerializeObject(parameter);
        }

        protected virtual async Task<IResult<TResult>> GetResult<TResult>(HttpResponseMessage response)
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
            else
            {
                return await Result<TResult>.FailAsync(response.StatusCode.ToString(), response.StatusCode.ToString());
            }
        }

        protected virtual async Task<IResult<TResult>> GetAsync<TResult>()
        {
            try
            {
                var response = await Client.GetAsync(URI);

                return await GetResult<TResult>(response);
            }
            catch (Exception ex)
            {
                return await Result<TResult>.FailAsync(ex.Message);
            }
        }

        protected virtual async Task<IResult<TResult>> GetAsync<TResult>(object parameter)
        {

            try
            {
                URI = string.Format("{0}/?{1}", URL, QueryString(parameter));

                var response = await Client.GetAsync(URI);

                return await GetResult<TResult>(response);
            }
            catch (Exception ex)
            {
                return await Result<TResult>.FailAsync(ex.Message);
            }
        }

        protected virtual async Task<IResult<TResult>> PostAsync<TResult>(object parameter)
        {
            try
            {
                var content = new StringContent(Serialize(parameter))
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                };

                var response = await Client.PostAsync(URI, content);

                return await GetResult<TResult>(response);
            }
            catch (Exception ex)
            {
                return await Result<TResult>.FailAsync(ex.Message);
            }
        }

        protected async virtual Task<IResult<TResult>> DeleteAsync<TResult>(object parameter)
        {
            try
            {
                URI = string.Format("{0}/?{1}", URL, QueryString(parameter));

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
