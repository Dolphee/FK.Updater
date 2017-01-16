using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FKUpdater.Assets.Factory
{
    public abstract class Singleton<T>
    {
        public static T _Instance { get; set; }
        public static T Instance()
        {
            if(_Instance == null)
                throw new NullReferenceException();

            return _Instance;
        }

        public static void Set(T value)
        {
            if(_Instance == null)
                _Instance = value;
        }
    }
}
