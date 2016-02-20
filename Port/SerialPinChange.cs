namespace SerialPortLib2.Port
{
    public enum SerialPinChange
    {
        CtsChanged = 8,
        DsrChanged = 16,
        CDChanged = 32,
        Break = 64,
        Ring = 256
    }
}
