using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADXService
{
    public interface IADXAccess
    {
        Task CreateTable();

        Task IngestionMapping();

        Task Batching();

        bool CheckIfTableExist();

        long RowCount();
    }

}
