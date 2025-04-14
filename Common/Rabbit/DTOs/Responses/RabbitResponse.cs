using Common.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Rabbit.DTOs.Responses
{
    public class RabbitResponse
    {
        public int status { get; set; }
        public string message { get; set; }

        public RabbitResponse() { }

        public RabbitResponse(int status, string message)
        {
            this.status = status;
            this.message = message;
        }

        public RabbitResponse(ErrorException ErrorException)
        {
            status = ErrorException.status;
            message = ErrorException.message;
        }
    }
}
