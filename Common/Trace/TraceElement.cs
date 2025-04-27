using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Trace
{
	public class TraceElement
	{
		public required string TraceId { get; set; }
		public required string DictionaryId { get; set; }
	}
}
