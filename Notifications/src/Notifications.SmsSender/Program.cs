using System;
using System.Configuration;
using RestSharp;

namespace Notifications.SmsSender
{
	class Program
	{
		static void Main(string[] args)
		{
			SendSms("", "This is a trial run");
		}

		private static void SendSms(string number, string content)
		{
			try
			{
				var apiKey = ConfigurationManager.AppSettings["TextLocalApiKey"];
				var request = new RestRequest("send", Method.POST);
				request.AddParameter("sender", "Dan and Sunit");
				request.AddParameter("message", content);
				request.AddParameter("apiKey", apiKey);
				request.AddParameter("numbers", number);

				var client = new RestClient("https://api.txtlocal.com");
				var response = client.Execute(request);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to send text message. Details: {ex.Message}");
			}
		}
	}
}
