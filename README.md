[![nuget](https://img.shields.io/nuget/v/MonoSerialPort.svg)](https://www.nuget.org/packages/MonoSerialPort/)

# MonoSerialPort
Serial port library for .Net / Mono, that can be used with virtual usb port

# Notice
The project has been renamed from SerialPortLib2 TO MonoSerialPort

# Prerequisites
The library is built against .NET Standard 2.0, which means it's supported by different Target Platforms like .NET Framework, .NET Core, Mono and more.  
You can find a full list of the supported frameworks (and which version you need) [here](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).  

## Dependencies
The library depends on two nuget packages:  
- [Microsoft.Windows.Compatibility](https://www.nuget.org/packages/Microsoft.Windows.Compatibility)  
- [Mono.Posix.NETStandard](https://www.nuget.org/packages/Mono.Posix.NETStandard/)  

Both of them are built against .NET Standard. If you don't use the nuget package but instead build the dll's on your own make sure to include those two libraries in your integrating project, otherwise it won't run!

# Usage
Just create an instance, if the port is virtual usb, create the object with the second constructor SerialPortInput(true).

    var _serialPort = new SerialPortInput(isVirtualPort);
    _serialPort.SetPort(port, baudRate);
    _serialPort.ConnectionStatusChanged += SerialPort_ConnectionStatusChanged;
    _serialPort.MessageReceived += SerialPort_MessageReceived;
   
      public bool OpenConnection()
      {
          if (!_serialPort.IsConnected)
          {
              return _serialPort.Connect();
          }
          else
              return false;
      }
      
      public void CloseConnection()
      {
          if (_serialPort != null)
          {
              _serialPort.Disconnect();
          }
          _buffer = string.Empty;
      }      
      
      public bool write(byte[] packet)
      {
          return _serialPort.SendMessage(packet);
      }
      
      private void SerialPort_MessageReceived(object sender, MessageReceivedEventArgs args)
      {
          _buffer += Encoding.ASCII.GetString(args.Data);
          *Do something*
      }
      private void SerialPort_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs args)
      {
          if (OnConnecting != null)
              OnConnecting(this, new StateDeviceEventArgs(args.Connected));
      }


MIT License
Copyright (c) 2018 Gerardo Trotta

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
