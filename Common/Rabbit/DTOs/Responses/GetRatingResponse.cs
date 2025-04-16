using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Rabbit.DTOs.Responses
{
    public class GetRatingResponse : RabbitResponse
    {
        public int Rating { get; set; }

        public GetRatingResponse() : base() {}

        public GetRatingResponse(int Rating)
        {
            this.Rating = Rating;
        }
    }
}
