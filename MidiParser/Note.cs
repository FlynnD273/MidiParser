using System;
using System.Collections.Generic;
using System.Text;

namespace MidiParser
{
    class Note
    {
        private int _notePitch;
        public int NotePitch { get => _notePitch; private set => _notePitch = value; }
        private long _timeStart;
        public long TimeStart { get => _timeStart; private set => _timeStart = value; }
        private int _length;
        public int Length { get => _length; private set => _length = value; }
        private int _velocity;
        public int Velocity { get => _velocity; private set => _velocity = value; }

        public Note (int note, long timeStart, int length, int velocity)
        {
            NotePitch = note;
            TimeStart = timeStart;
            Length = length;
            Velocity = velocity;
        }
    }
}
