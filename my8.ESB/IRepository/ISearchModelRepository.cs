using my8.ESB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB.IRepository
{
    public interface ISearchModelRepository
    {
        Task<string> Create(SearchModel model);
    }
}
