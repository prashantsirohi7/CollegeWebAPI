using System.Net;

namespace StudentWebAPIProject.Models
{
    public class ApiResponce
    {
        public bool Status { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public dynamic? Data { get; set; }
        public List<string> Errors { get; set; }
    }
}
