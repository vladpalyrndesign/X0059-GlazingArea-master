using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlazingDataAcessLayer
{
    public interface IGlazingRepository<T>
    {
        void Add(params T[] items);
        void update(params T[] items);
    }
}
