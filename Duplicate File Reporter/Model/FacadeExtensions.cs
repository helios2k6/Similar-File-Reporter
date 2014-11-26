using PureMVC.Interfaces;
using System;
namespace DuplicateFileReporter.Model
{
    public static class FacadeExtensions
    {
        public static T RetrieveProxy<T>(this IFacade @this, string proxyName)
        {
            var proxy = @this.RetrieveProxy(proxyName);
            if (proxy == null || proxy is T == false)
            {
                throw new InvalidOperationException("Unable to retrieve proxy of type: " + typeof(T));
            }

            return (T)proxy;
        }
    }
}
