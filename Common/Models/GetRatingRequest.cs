﻿using Common.Rabbit.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class GetRatingRequest
    {
        public Guid ClientId { get; set; }
        public string TraceId { get; set; }

        public GetRatingRequest() {}

        public GetRatingRequest(Guid ClientId, string TraceId)
        {
            this.ClientId = ClientId;
            this.TraceId = TraceId;
        }
    }
}
