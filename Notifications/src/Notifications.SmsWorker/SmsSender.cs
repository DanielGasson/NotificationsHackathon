using System;
using System.Configuration;
using System.Net;
using RestSharp;

namespace Notifications.SmsWorker
{
	public class SmsSender
	{
		private readonly RestClient _restClient;

		public SmsSender()
		{
			var baseUrl = ConfigurationManager.AppSettings["TextLocalBaseUrl"];
			_restClient = new RestClient(baseUrl);
		}

		public void SendSms(string number, string content)
		{
			try
			{
				var apiKey = ConfigurationManager.AppSettings["TextLocalApiKey"];

				var request = new RestRequest("send", Method.POST);
				request.AddParameter("sender", "Notifications Hackathon");
				request.AddParameter("message", content);
				request.AddParameter("apiKey", apiKey);
				request.AddParameter("numbers", number);

				var response = _restClient.Execute(request);
				if (response.StatusCode != HttpStatusCode.OK)
				{
					throw new Exception($"Could not send message to {number}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to send text message. Details: {ex.Message}");
			}
		}
	}
}
