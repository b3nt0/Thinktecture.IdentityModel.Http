using System.Collections.Concurrent;
using System.Reflection;
using System.Web.Http.Controllers;

namespace Thinktecture.IdentityModel.Http
{
    public abstract class ReflectionBasedAuthorizationManager : IAuthorizationManager
    {
        protected static ConcurrentDictionary<string, MethodInfo> _methods = new ConcurrentDictionary<string, MethodInfo>();

        protected abstract string BuildMethodName(HttpActionContext context);

        public virtual bool CheckAccess(HttpActionContext context)
        {
            var name = BuildMethodName(context);
            MethodInfo info;
            
            if (_methods.TryGetValue(name, out info))
            {
                return Invoke(info, context);
            }
        
            info = GetMethodInfo(name, context);
            if (info == null)
            {
                return false;
            }

            _methods.TryAdd(name, info);
            return Invoke(info, context);
        }

        protected virtual MethodInfo GetMethodInfo(string name, HttpActionContext context)
        {
            var info = this.GetType().GetMethod(name);
            return info;
        }

        protected virtual bool Invoke(MethodInfo info, HttpActionContext context)
        {
            try
            {
                var result = info.Invoke(this, new object[] { context });
                return (bool)result;
            }
            catch
            {
                return false;
            }
        }
    }
}
