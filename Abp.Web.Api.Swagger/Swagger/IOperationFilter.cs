﻿using System.Web.Http.Description;

namespace Abp.Swagger
{
    public interface IOperationFilter
    {
        void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription);
    }
}
