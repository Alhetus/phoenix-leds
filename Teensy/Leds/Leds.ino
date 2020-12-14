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

// Maps pixel on the physical led strip positions
int pixelMappingTable[] = {
  // Strip 1
  0,   1,   2,   3,   4,   5,   6,   7,   8,   9,   10,  11,
  23,  22,  21,  20,  19,  18,  17,  16,  15,  14,  13, 12,
  24,  25,  26,  27,  28,  29,  30,  31,  32,  33,  34,  35,
  47,  46,  45,  44,  43,  42,  41,  40,  39,  38,  37,  36,
  48,  49,  50,  51,  52,  53,  54,  55,  56,  57,  58,  59,
  71,  70,  69,  68,  67,  66,  65,  64,  63,  62,  61,  60,
  // Strip 2
  72,  73,  74,  75,  76,  77,  78,  79,  80,  81,  82,  83,
  95,  94,  93,  92,  91,  90,  89,  88,  87,  86,  85,  84,
  96,  97,  98,  99,  100, 101, 102, 103, 104, 105, 106, 107,
  119, 118, 117, 116, 115, 114, 113, 112, 111, 110, 109, 108,
  120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131,
  143, 142, 141, 140, 139, 138, 137, 136, 135, 134, 133, 132,
  // Strip 3
  144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155,
  167, 166, 165, 164, 163, 162, 161, 160, 159, 158, 157, 156,
  168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179,
  191, 190, 189, 188, 187, 186, 185, 184, 183, 182, 181, 180,
  192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203,
  215, 214, 213, 212, 211, 210, 209, 208, 207, 206, 205, 204,
  // Strip 4
  216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227,
  239, 238, 237, 236, 235, 234, 233, 232, 231, 230, 229, 228,
  240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251,
  263, 262, 261, 260, 259, 258, 257, 256, 255, 254, 253, 252,
  264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275,
  287, 286, 285, 284, 283, 282, 281, 280, 279, 278, 277, 276,
  // Strip 5
  288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299,
  311, 310, 309, 308, 307, 306, 305, 304, 303, 302, 301, 300,
  312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323,
  335, 334, 333, 332, 331, 330, 329, 328, 327, 326, 325, 324,
  336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347,
  359, 358, 357, 356, 355, 354, 353, 352, 351, 350, 349, 348,
  // Strip 6
  360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371,
  383, 382, 381, 380, 379, 378, 377, 376, 375, 374, 373, 372,
  384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395,
  407, 406, 405, 404, 403, 402, 401, 400, 399, 398, 397, 396,
  408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419,
  431, 430, 429, 428, 427, 426, 425, 424, 423, 422, 421, 420,
  // Strip 7
  432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443,
  455, 454, 453, 452, 451, 450, 449, 448, 447, 446, 445, 444,
  456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467,
  479, 478, 477, 476, 475, 474, 473, 472, 471, 470, 469, 468,
  480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491,
  503, 502, 501, 500, 499, 498, 497, 496, 495, 494, 493, 492,
  // Strip 8
  504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515,
  527, 526, 525, 524, 523, 522, 521, 520, 519, 518, 517, 516,
  528, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539,
  551, 550, 549, 548, 547, 546, 545, 544, 543, 542, 541, 540,
  552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563,
  575, 574, 573, 572, 571, 570, 569, 568, 567, 566, 565, 564
};

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
    
    // Received start of frame control byte
    if (b == 0xfe) {
      // Start accumulating bytes to frame buffer
      frameByteIndex = 0;
    }
    // Received end of frame control byte
    else if (b == 0xff) {
      // Show frame pixels
      for (int i = 0; i < leds.numPixels(); i++) {
        leds.setPixel(pixelMappingTable[i], frameBuffer[i*3], frameBuffer[i*3 + 1], frameBuffer[i*3 + 2]);
      }
      
      leds.show();
    }
    // Received data byte
    else {
      frameBuffer[frameByteIndex] = b;
      frameByteIndex++;
    }
  }
  
  delayMicroseconds(100);
}
