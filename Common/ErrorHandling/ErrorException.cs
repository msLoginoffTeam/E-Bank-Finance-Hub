﻿using Common.Rabbit.DTOs.Responses;

namespace Common.ErrorHandling
{
    public class ErrorException : Exception
    {
        public int status { get; set; }

        public string message { get; set; }

        public ErrorException(int status, string message)
        {
            this.status = status;
            this.message = message;
        }

        public ErrorException(ErrorResponse ErrorResponse)
        {
            status = ErrorResponse.status;
            message = ErrorResponse.message;
        }

        public ErrorException(RabbitResponse ErrorResponse)
        {
            status = ErrorResponse.status;
            message = ErrorResponse.message;
        }
    }
}
