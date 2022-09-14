using ei8.Cortex.Diary.Domain.Model;
using ei8.Cortex.Diary.Port.Adapter.Common;
using neurUL.Common.Domain.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.IO.Persistence.SQLite
{
    public class ViewRepository : IViewRepository
    {
        private SQLiteAsyncConnection connection;

        public async Task<IEnumerable<View>> GetAll()
        {
            var results = this.connection.Table<View>();
            return (await results.OrderBy(v => v.Sequence).ToArrayAsync());
        }

        public async Task Initialize()
        {
            this.connection = await ViewRepository.CreateConnection<View>();

            //sample data creator - call Initialize from CustomBootstrapper to invoke
            //await this.connection.InsertAsync(new View()
            //{
            //    Url = "http",
            //    Name = "Home",
            //    IsDefault = true,
            //    Sequence = 1
            //});
        }

        internal static async Task<SQLiteAsyncConnection> CreateConnection<TTable>() where TTable : new()
        {
            SQLiteAsyncConnection result = null;
            string databasePath = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.DatabasePath);
            
            if (!databasePath.Contains(":memory:"))
                AssertionConcern.AssertPathValid(databasePath, nameof(databasePath));

            result = new SQLiteAsyncConnection(databasePath);
            await result.CreateTableAsync<TTable>();
            return result;
        }
    }
}
