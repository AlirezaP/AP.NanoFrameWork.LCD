# AP.NanoFrameWork.LCD
Library To Control LCD (character LCD, text-based LCDs) In Nanoframework

(Lcd.h)

Sampel:
```
using System;
using System.Diagnostics;
using System.Threading;

namespace Sample
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            AP.NanoFrameWork.LCD.CharLCD myCharLCD = new AP.NanoFrameWork.LCD.CharLCD(27, 14, 33, 15, 4, 16, 17, 5, 18, 19, 21);
            myCharLCD.Begin(20, 4, 1);

            myCharLCD.SetCursor(0, 0);
            myCharLCD.WriteText("Hi friends");
            myCharLCD.SetCursor(0, 1);
            myCharLCD.WriteText("I love Nanoframework");
            myCharLCD.SetCursor(0, 3);
            myCharLCD.WriteText("Github.com/Alirezap");

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}
```
