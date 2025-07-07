using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;


using System.IO;

namespace GroupCheck.ServerProxy
{
	public static class HTTP
	{
		public static HttpResponse MakeRequest(string method, string url, string contentType, Dictionary<string, string> headers, string body)
		{
			var result = new HttpResponse();

			var request = WebRequest.Create(url);
			request.Method = method;
			request.ContentType = contentType;

			if (headers != null)
			{
				foreach (var header in headers)
				{
					request.Headers.Add(header.Key, header.Value);
				}
			}

			if (body != null)
			{
				var data = Encoding.UTF8.GetBytes(body);
				request.ContentLength = data.Length;
				using (var writer = request.GetRequestStream())
				{
					writer.Write(data, 0, data.Length);
					writer.Close();
				}
			}

			try
			{
				using (var response = (HttpWebResponse)request.GetResponse())
				{
					result.ResponseCode = response.StatusCode;
					result.Headers = GetResponseHeaders(response);
					result.Content = GetResponseBody(response);
				}
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
				{
					var response = (HttpWebResponse)ex.Response;
					result.ResponseCode = response.StatusCode;
					result.Headers = GetResponseHeaders(response);
					result.Content = GetResponseBody(response);
					result.ErrorMessage = ex.Message;
				}
				else
				{
					throw;
				}
			}

			return result;
		}
		private static Dictionary<string, string> GetResponseHeaders(HttpWebResponse response)
		{
			var headers = new Dictionary<string, string>();
			if (response?.Headers != null)
			{
				for (int i = 0; i < response.Headers.Count; i++)
				{
					headers.Add(response.Headers.Keys[i], response.Headers[i]);
				}
			}
			return headers;
		}
		private static string GetResponseBody(HttpWebResponse response)
		{
			using (var responseStream = response.GetResponseStream())
			{
				using (var reader = new StreamReader(responseStream))
				{
					return reader.ReadToEnd().Trim();
				}
			}
		}
	}

	public class HttpResponse
	{
		public HttpStatusCode ResponseCode { get; set; }
		public Dictionary<string, string> Headers { get; set; }
		public string Content { get; set; }
		public string ErrorMessage { get; set; }
	}
}
