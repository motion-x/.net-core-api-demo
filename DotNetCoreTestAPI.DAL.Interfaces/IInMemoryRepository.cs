using LiteDB;
using System;

namespace DotNetCoreTestAPI.DAL.Interfaces
{
    public interface IInMemoryRepository : IDisposable
    {
        LiteDatabase Client { get; set; }
        string DBName { get; }

        void PersistData();
    }
}