using System;
using System.Net;

namespace Abp.Swagger.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SwaggerResponseRemoveDefaultsAttribute : Attribute
    {
    }
}