using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealabsDatabase
{
    interface IDealabsData<T>
    {
        T get(int Id);
        List<T> getAll();
        void insert(T Item);
        void delete(T Item);
        void update(T Item);
    }
}
