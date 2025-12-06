using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.DAL.Wapper
{
    public class Response<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
        public bool IsSucceded { get; set; }
        public DateTime DateTime { get; set; }


        public Response()
        {
            DateTime = DateTime.Now;
        }

        public Response(T data , string massage) : this()
        {
            Data = data;
            Message = massage;
            IsSucceded = true;
        }

        public Response(string massage) : this()
        {
            Message = massage;
            IsSucceded = false;
        }

        // Static factory methods for cleaner usage
        public static Response<T> Success(T data, string message = "") => new Response<T>(data, message);
        public static Response<T> Fail(string message) => new Response<T>(message);
        public static Response<T> Success(T data, string message = "") => new Response<T>(data, message);
        public static Response<T> Fail(string message) => new Response<T>(message);


    }
}
