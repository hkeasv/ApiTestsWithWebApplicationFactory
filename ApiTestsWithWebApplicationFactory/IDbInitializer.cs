using System;
using TodoApi.Data;

namespace ApiTestsWithWebApplicationFactory
{
    public interface IDbInitializer
    {
        void Initialize(TodoContext context);
    }
}
