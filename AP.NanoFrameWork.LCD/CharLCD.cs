using System;
using System.Threading;
using Windows.Devices.Gpio;

namespace AP.NanoFrameWork.LCD
{
    public class CharLCD
    {
        public enum Commands
        {
            // commands
            LCD_CLEARDISPLAY = 0x01,
            LCD_RETURNHOME = 0x02,
            LCD_ENTRYMODESET = 0x04,
            LCD_DISPLAYCONTROL = 0x08,
            LCD_CURSORSHIFT = 0x10,
            LCD_FUNCTIONSET = 0x20,
            LCD_SETCGRAMADDR = 0x40,
            LCD_SETDDRAMADDR = 0x80,

            // flags for display entry mode
            LCD_ENTRYRIGHT = 0x00,
            LCD_ENTRYLEFT = 0x02,
            LCD_ENTRYSHIFTINCREMENT = 0x01,
            LCD_ENTRYSHIFTDECREMENT = 0x00,

            // flags for display on/off control
            LCD_DISPLAYON = 0x04,
            LCD_DISPLAYOFF = 0x00,
            LCD_CURSORON = 0x02,
            LCD_CURSOROFF = 0x00,
            LCD_BLINKON = 0x01,
            LCD_BLINKOFF = 0x00,



            // flags for function set
            LCD_8BITMODE = 0x10,
            LCD_4BITMODE = 0x00,
            LCD_2LINE = 0x08,
            LCD_1LINE = 0x00,
            LCD_5x10DOTS = 0x04,
            LCD_5x8DOTS = 0x00,
        }

        //.................................

        static byte _displayfunction = 0;
        static byte _displaycontrol = 0;
        static byte _displaymode = 0;

        private static int _numlines = 1;

        static int[] _row_offsets = new int[4];
        static GpioPin[] lcdPins = new GpioPin[8];

        //..................................

        private static GpioPin rsPin = null;
        private static GpioPin rwPin = null;
        private static GpioPin ePin = null;


        private static GpioPin db0Pin = null;
        private static GpioPin db1Pin = null;
        private static GpioPin db2Pin = null;
        private static GpioPin db3Pin = null;
        private static GpioPin db4Pin = null;
        private static GpioPin db5Pin = null;
        private static GpioPin db6Pin = null;
        private static GpioPin db7Pin = null;

        //..................................

        public CharLCD(int rs, int rw, int e, int d0, int d1, int d2, int d3, int d4, int d5, int d6, int d7)
        {
            Initial(rs, rw, e, d0, d1, d2, d3, d4, d5, d6, d7);
        }


        private void Initial(int rs, int rw, int e, int d0, int d1, int d2, int d3, int d4, int d5, int d6, int d7)
        {
            InitialGPIO(rs, rw, e, d0, d1, d2, d3, d4, d5, d6, d7);


            _displayfunction = ((byte)Commands.LCD_8BITMODE) | ((byte)Commands.LCD_1LINE) | ((byte)Commands.LCD_5x8DOTS);

        }

        private void InitialGPIO(int rs, int rw, int e, int d0, int d1, int d2, int d3, int d4, int d5, int d6, int d7)
        {
            var s_GpioController = new GpioController();

            rsPin = s_GpioController.OpenPin(rs);
            rsPin.SetDriveMode(GpioPinDriveMode.Output);

            rwPin = s_GpioController.OpenPin(rw);
            rwPin.SetDriveMode(GpioPinDriveMode.Output);

            ePin = s_GpioController.OpenPin(e);
            ePin.SetDriveMode(GpioPinDriveMode.Output);
            ePin.Write(GpioPinValue.High);


            db0Pin = s_GpioController.OpenPin(d0);
            db0Pin.SetDriveMode(GpioPinDriveMode.Output);

            db1Pin = s_GpioController.OpenPin(d1);
            db1Pin.SetDriveMode(GpioPinDriveMode.Output);

            db2Pin = s_GpioController.OpenPin(d2);
            db2Pin.SetDriveMode(GpioPinDriveMode.Output);

            db3Pin = s_GpioController.OpenPin(d3);
            db3Pin.SetDriveMode(GpioPinDriveMode.Output);

            db4Pin = s_GpioController.OpenPin(d4);
            db4Pin.SetDriveMode(GpioPinDriveMode.Output);

            db5Pin = s_GpioController.OpenPin(d5);
            db5Pin.SetDriveMode(GpioPinDriveMode.Output);

            db6Pin = s_GpioController.OpenPin(d6);
            db6Pin.SetDriveMode(GpioPinDriveMode.Output);

            db7Pin = s_GpioController.OpenPin(d7);
            db7Pin.SetDriveMode(GpioPinDriveMode.Output);


            lcdPins[0] = db0Pin;
            lcdPins[1] = db1Pin;
            lcdPins[2] = db2Pin;
            lcdPins[3] = db3Pin;
            lcdPins[4] = db4Pin;
            lcdPins[5] = db5Pin;
            lcdPins[6] = db6Pin;
            lcdPins[7] = db7Pin;



        }

        private void SendCommand(byte value)
        {
            SendData(value, GpioPinValue.Low);

        }

        // write either command or data, with automatic 4/8-bit selection
        private void SendData(byte value, GpioPinValue mode)
        {
            ePin.Write(GpioPinValue.High);

            rsPin.Write(mode);
            Thread.Sleep(1);

            rwPin.Write(GpioPinValue.Low);

            Write8bits(value);

            Thread.Sleep(10);

            ePin.Write(GpioPinValue.Low);
        }

        private void WriteData(byte value)
        {
            SendData(value, GpioPinValue.High);
        }

        private void Write8bits(byte value)
        {
            for (int i = 0; i < 8; i++)
            {
                var val = (value >> i) & 0x01;

                if (val == 0)
                {
                    lcdPins[i].Write(GpioPinValue.Low);
                }

                if (val == 1)
                {
                    lcdPins[i].Write(GpioPinValue.High);
                }
                //digitalWrite(_data_pins[i], (value >> i) & 0x01);
            }

        }

        private void SetRowOffsets(int row0, int row1, int row2, int row3)
        {
            _row_offsets[0] = row0;
            _row_offsets[1] = row1;
            _row_offsets[2] = row2;
            _row_offsets[3] = row3;
        }


        public void Begin(byte cols, byte lines, byte dotsize)
        {
            if (lines > 1)
            {
                _displayfunction |= (byte)Commands.LCD_2LINE;
            }

            _numlines = lines;

            SetRowOffsets(0x00, 0x40, 0x00 + cols, 0x40 + cols);

            //// for some 1 line displays you can select a 10 pixel high font
            //if ((dotsize != LCD_5x8DOTS) && (lines == 1))
            //{
            //    _displayfunction |= LCD_5x10DOTS;
            //}

            SetRowOffsets(0x00, 0x40, 0x00 + cols, 0x40 + cols);

            // for some 1 line displays you can select a 10 pixel high font
            //// if ((dotsize != LCD_5x8DOTS) && (lines == 1))
            // {
            //     _displayfunction |= LCD_5x10DOTS;
            // }



            Thread.Sleep(50);


            rsPin.Write(GpioPinValue.Low);
            ePin.Write(GpioPinValue.Low);
            rwPin.Write(GpioPinValue.Low);


            // Send function set command sequence
            SendCommand((byte)(((byte)Commands.LCD_FUNCTIONSET) | _displayfunction));
            Thread.Sleep(4500);  // wait more than 4.1 ms

            SendCommand((byte)(((byte)Commands.LCD_FUNCTIONSET) | _displayfunction));
            Thread.Sleep(1);  // wait more than 4.1 ms
                              //  delayMicroseconds(150);

            SendCommand((byte)(((byte)Commands.LCD_FUNCTIONSET) | _displayfunction));
            Thread.Sleep(1);


            // finally, set # lines, font size, etc.
            SendCommand((byte)((byte)Commands.LCD_FUNCTIONSET | _displayfunction));

            // turn the display on with no cursor or blinking default
            _displaycontrol = (byte)Commands.LCD_DISPLAYON | (byte)Commands.LCD_CURSOROFF | (byte)Commands.LCD_BLINKOFF;
            Display();

            // clear it off
            Clear();


            // Initialize to default text direction (for romance languages)
            _displaymode = (byte)Commands.LCD_ENTRYLEFT | (byte)Commands.LCD_ENTRYSHIFTDECREMENT;
            // set the entry mode
            SendCommand((byte)((byte)Commands.LCD_ENTRYMODESET | _displaymode));
        }

        public void SetCursor(int col, int row)
        {
            //const int max_lines = (_row_offsets.Length) / (_row_offsets.Length);
            //if (row >= max_lines)
            //{
            //    row = max_lines - 1;    // we count rows starting w/ 0
            //}
            //if (row >= _numlines)
            //{
            //    row = _numlines - 1;    // we count rows starting w/ 0
            //}

            SendCommand((byte)((byte)Commands.LCD_SETDDRAMADDR | (col + _row_offsets[row])));
        }

        public void Display()
        {
            _displaycontrol |= (byte)Commands.LCD_DISPLAYON;
            SendCommand((byte)((byte)Commands.LCD_DISPLAYCONTROL | _displaycontrol));
        }

        public void Clear()
        {
            SendCommand((byte)Commands.LCD_CLEARDISPLAY);  // clear display, set cursor position to zero
            Thread.Sleep(2);  // this command takes a long time!
        }

        public void WriteText(string txt)
        {
            for (int i = 0; i < txt.Length; i++)
            {
                WriteData((byte)txt[i]);
                Thread.Sleep(10);
            }
        }

        public void WriteText(string txt,int startCol,int startRow)
        {
            SetCursor(startCol, startRow);
            Thread.Sleep(10);
            WriteText(txt);
        }
    }
}
