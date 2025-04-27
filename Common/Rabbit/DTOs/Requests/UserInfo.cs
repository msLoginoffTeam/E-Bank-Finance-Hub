using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Rabbit.DTOs.Requests
{
	public class UserInfo
	{
		public Guid UserId { get; set; }
		public string TracerId { get; set; }
	}
}
