using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace Thinktecture.IdentityModel.Http.Client
{
    public static class HttpMethods
    {
        public static readonly string Get = HttpMethod.Get.Method;
        public static readonly string Put = HttpMethod.Put.Method;
        public static readonly string Post = HttpMethod.Post.Method;
        public static readonly string Delete = HttpMethod.Delete.Method;
    }
}
