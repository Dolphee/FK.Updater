using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FKUpdater.Assets.Structs
{
    public interface IControl<T>
    {
        string Title { get; set; }
        T Control { get; set; }
    }
}
