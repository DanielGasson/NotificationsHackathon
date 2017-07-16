using System.ComponentModel;

namespace Common.DataAccess.Models
{
	public static class ContentTypeExtensions
	{
		public static string Description(this ContentType contentType)
		{
			var attributes = (DescriptionAttribute[]) contentType.GetType().GetField(contentType.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

			return attributes.Length > 0
				? attributes[0].Description
				: string.Empty;
		}
	}
}
