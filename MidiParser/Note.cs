using System;
using System.Collections.Generic;
using System.Text;

namespace MidiParser
{
    class Note
    {
        public int NotePitch { get; private set; }
        public long TimeStart { get; private set; }
        public int Length { get; private set; }
        public int Velocity { get; private set; }

        public Note (int note, long timeStart, int length, int velocity)
        {
            NotePitch = note;
            TimeStart = timeStart;
            Length = length;
            Velocity = velocity;
        }
    }
}
