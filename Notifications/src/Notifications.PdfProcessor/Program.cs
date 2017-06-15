using System;
using SelectPdf;

namespace Notifications.PdfProcessor
{
	class Program
	{
		static void Main(string[] args)
		{
			RetrieveMessage();
			GeneratePdf();
		}

		public static void RetrieveMessage()
		{
			
		}

		public static void GeneratePdf()
		{
			try
			{
				// Either manually copy the dep (generator) file into your build folder or specify it this way.
				GlobalProperties.HtmlEngineFullPath = @"E:\Dan\Dev\Test Projects\SelectPdfTest\SelectPdfTest\lib\Select.Html.dep";

				var converter = new HtmlToPdf();

				var htmlToConvert = "This is a test pdf";

				PdfDocument signUpEmailDocument = converter.ConvertHtmlString(htmlToConvert);
				signUpEmailDocument.Save(@"E:\Dan\Dev\TestPdf.pdf");
				signUpEmailDocument.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			Console.ReadKey();
		}
	}
}
