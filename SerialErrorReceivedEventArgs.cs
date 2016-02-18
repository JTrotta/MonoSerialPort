using System;

namespace SerialPortLib2
{
    public class SerialErrorReceivedEventArgs : EventArgs
    {

        internal SerialErrorReceivedEventArgs(SerialError eventType)
        {
            this.eventType = eventType;
        }

        // properties

        public SerialError EventType
        {
            get
            {
                return eventType;
            }
        }

        SerialError eventType;
    }
}
