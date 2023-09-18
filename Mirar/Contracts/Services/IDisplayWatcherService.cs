using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Display;
using Windows.Foundation;

namespace Mirar.Contracts.Services;
public interface IDisplayWatcherService
{
    event TypedEventHandler<object, DisplayMonitor> DisplayAdapterChanged;

    Task StartWatcherAsync();
    Task StopWatcherAsync();
}
