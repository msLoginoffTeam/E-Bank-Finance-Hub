using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Rabbit.DTOs.Requests
{
	public class CreatedUserIdMessage
	{
		public Guid ClientId { get; set; }
		public string TraceId { get; set; }
	}
}
