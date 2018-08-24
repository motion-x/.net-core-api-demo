using DotNetCoreTestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreTestAPILib.Models
{
    public class RequestResult<T> : IRequestResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public RequestResult(T data, Func<T, bool> isSuccess)
        {
            Data = data;
            Success = isSuccess(Data);
        }
    }
}
