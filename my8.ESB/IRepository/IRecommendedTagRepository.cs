using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my8.ESB.IRepository
{
    public interface IRecommendedTagRepository
    {
        Task<bool> UpdateCountUsed(string tag, int value);
        Task<bool> UpdateCountUsed(string[] tags, int value);
    }
}
