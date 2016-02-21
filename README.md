# SerialPortLib2
Serial port library for .Net / Mono, that can be used with virtual usb port

Usage:
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
