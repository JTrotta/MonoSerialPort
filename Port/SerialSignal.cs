namespace SerialPortLib2.Port
{
    enum SerialSignal
    {
        None = 0,
        Cd = 1, // Carrier detect 
        Cts = 2, // Clear to send
        Dsr = 4, // Data set ready
        Dtr = 8, // Data terminal ready
        Rts = 16 // Request to send
    }
}
