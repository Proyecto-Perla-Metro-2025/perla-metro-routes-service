using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutesService.Src.Responses
{
    public class ApiResponse<T>(T data, string message, bool success)
    {
        public T Data { get; set; } = data;
        public string Message { get; set; } = message;
        public bool Success { get; set; } = success;
    }
}