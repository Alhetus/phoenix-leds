/*  http://www.pjrc.com/teensy/td_libs_OctoWS2811.html
    Copyright (c) 2013 Paul Stoffregen, PJRC.COM, LLC

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.

  Required Connections
  --------------------
    pin 2:  LED Strip #1    OctoWS2811 drives 8 LED Strips.
    pin 14: LED strip #2    All 8 are the same length.
    pin 7:  LED strip #3
    pin 8:  LED strip #4    A 100 ohm resistor should used
    pin 6:  LED strip #5    between each Teensy pin and the
    pin 20: LED strip #6    wire to the LED strip, to minimize
    pin 21: LED strip #7    high frequency ringining & noise.
    pin 5:  LED strip #8
    pin 15 & 16 - Connect together, but do not use
    pin 4 - Do not use
    pin 3 - Do not use as PWM.  Normal use is ok.
*/

#include <OctoWS2811.h>

const int ledsPerStrip = 72;

DMAMEM int displayMemory[ledsPerStrip*6];
int drawingMemory[ledsPerStrip*6];

// Led grid size of 12x12, 3 bytes per pixel, 4 panels
byte frameBuffer[12*12 * 3 * 4];
int frameByteIndex = 0;

const int config = WS2811_GRB | WS2811_800kHz;

OctoWS2811 leds(ledsPerStrip, displayMemory, drawingMemory, config);

void setup() {
  Serial.begin(12582912); // USB is always 12 Mbit/sec, no matter what is specified here
  leds.begin();
  leds.show();
}

void loop() {
  while (Serial.available()) {
    // Protect against overflows, just write the last byte again
    if (frameByteIndex >= sizeof(frameBuffer) - 1) {
      frameByteIndex -= 1;
    }
    
    byte b = Serial.read();
    
    // Received control byte -> either start or end of frame
    if (b == 0xff) {
      byte b2 = Serial.read();
      
      // End of frame
      if (b2 == 0xff) {
        // Show frame pixels
        for (int i = 0; i < leds.numPixels(); i++) {
          leds.setPixel(i, frameBuffer[i*3], frameBuffer[i*3 + 1], frameBuffer[i*3 + 2]);
        }
        
        leds.show();
      }
      // Start of frame
      else {
        // Start accumulating bytes to frame buffer
        frameByteIndex = 0;
        frameBuffer[frameByteIndex] = b2;
        frameByteIndex++;
      }
    }
    // Received data byte
    else {
      frameBuffer[frameByteIndex] = b;
      frameByteIndex++;
    }
  }
  
  delayMicroseconds(10);
}
