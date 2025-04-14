using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Rabbit.DTOs.Requests
{
    public class RabbitRequest
    {
        public Guid IdempotencyKey { get; set; }

        public RabbitRequest() {}
    }
}
