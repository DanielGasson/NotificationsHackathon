using System.Collections.Generic;
using Common.DataAccess.Models;

namespace Common.DataAccess
{
	public class MockDatabase
	{
		public static IEnumerable<Customer> Customers => new List<Customer>
		{
			new Customer
			{
				Id = 1,
				FirstName = "Sunit",
				LastName = "Malkan",
				PhoneNumber = "00447507484850"
			},
			new Customer
			{
				Id = 2,
				FirstName = "Daniel",
				LastName = "Gasson",
				PhoneNumber = "00447949863879"
			}
		};
	}
}
