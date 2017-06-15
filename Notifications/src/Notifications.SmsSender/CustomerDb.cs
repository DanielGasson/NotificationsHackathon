using System.Collections.Generic;

namespace Notifications.SmsSender
{
	public class CustomerDb
	{
		public static List<CustomerRecord> Customers
		{
			get
			{
				return
					new List<CustomerRecord>
					{
						new CustomerRecord
						{
							Id = 1,
							FirstName = "Sunit",
							LastName = "Malkan",
							PhoneNumber = "00447507484850"
						},
						new CustomerRecord
						{
							Id = 1,
							FirstName = "Daniel",
							LastName = "Gasson",
							PhoneNumber = "00447949863879"
						},
						new CustomerRecord
						{
							Id = 1,
							FirstName = "Jamie",
							LastName = "Howard",
							PhoneNumber = "00447507484850"
						},
						new CustomerRecord
						{
							Id = 1,
							FirstName = "Morgan",
							LastName = "Faget",
							PhoneNumber = "00447949863879"
						},
						new CustomerRecord
						{
							Id = 1,
							FirstName = "Katerina",
							LastName = "Gerykova",
							PhoneNumber = "00447507484850"
						}
					};
			}
		}

		public class CustomerRecord
		{
			public int Id { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public string PhoneNumber { get; set; }
		}
	}
}
