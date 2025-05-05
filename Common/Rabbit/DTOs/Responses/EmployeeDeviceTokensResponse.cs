using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Rabbit.DTOs.Responses
{
    public class EmployeeDeviceTokensResponse : RabbitResponse
    {
        public List<string> DeviceTokens { get; set; }

        public EmployeeDeviceTokensResponse() : base() { }

        public EmployeeDeviceTokensResponse(List<string> DeviceTokens)
        {
            this.DeviceTokens = DeviceTokens;
        }
    }
}
