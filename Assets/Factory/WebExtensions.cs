using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FKUpdater
{

    public static class WebExtensions
    {
        public static void UIThread(Action action)
        {
            var dispatcher = System.Windows.Application.Current.Dispatcher;
            if (dispatcher.CheckAccess())
                action.Invoke();
            else
                dispatcher.Invoke(action);
        }


        public static double BytesToMegaBytes(this long bytes)
        {
            return Math.Round((double)(bytes / 1024) / 1024, 2);
        }
    }
}
