﻿namespace CustomerServiceApi.Core.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        int Complete();
    }
}
