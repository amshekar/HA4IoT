﻿using System;
using System.Collections.Generic;
using Windows.Devices.Gpio;
using HA4IoT.Contracts.Hardware;
using HA4IoT.Contracts.Hardware.Services;
using HA4IoT.Contracts.Services;
using HA4IoT.Contracts.Services.System;

namespace HA4IoT.Hardware.RaspberryPi
{
    public class GpioService : ServiceBase, IGpioService
    {
        private readonly GpioController _gpioController = GpioController.GetDefault();
        private readonly Dictionary<int, GpioPort> _openPorts = new Dictionary<int, GpioPort>();

        public GpioService(ITimerService timerService)
        {
            if (timerService == null) throw new ArgumentNullException(nameof(timerService));

            timerService.Tick += (s, e) => PollOpenInputPorts();
        }

        public IBinaryInput GetInput(int number)
        {
            return OpenPort(number, GpioPinDriveMode.Input);
        }

        public IBinaryOutput GetOutput(int number)
        {
            return OpenPort(number, GpioPinDriveMode.Output);
        }

        private void PollOpenInputPorts()
        {
            foreach (GpioPort port in _openPorts.Values)
            {
                if (port.Pin.GetDriveMode() == GpioPinDriveMode.Input)
                {
                    ((IBinaryInput)port).Read();
                }
            }
        }

        private GpioPort OpenPort(int number, GpioPinDriveMode mode)
        {
            GpioPort port;
            if (_openPorts.TryGetValue(number, out port))
            {
                return port;
            }

            var pin = _gpioController.OpenPin(number, GpioSharingMode.Exclusive);
            pin.SetDriveMode(mode);
            
            port = new GpioPort(pin);
            _openPorts.Add(number, port);

            return port;
        }
    }
}
