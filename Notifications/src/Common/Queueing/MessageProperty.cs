namespace Common.Queueing
{
	public class MessageProperty
	{
		public string Key { get; set; }
		public object Value { get; set; }

		public MessageProperty(string key, object value)
		{
			Key = key;
			Value = value;
		}
	}
}
