using System;

namespace SerialPortLib2.Port
{
    internal class SerialDataReceivedEventArgs : EventArgs
    {
        internal SerialDataReceivedEventArgs(SerialData eventType)
        {
            this.eventType = eventType;
        }

        // properties

        internal SerialData EventType
        {
            get
            {
                return eventType;
            }
        }

        SerialData eventType;
    }
}
