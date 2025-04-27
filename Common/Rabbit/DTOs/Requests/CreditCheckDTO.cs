using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Rabbit.DTOs.Requests
{
	public class CreditCheckDTO
	{
		public Guid AccountId { get; set; }
		public string TraceId { get; set; }
	}
}
