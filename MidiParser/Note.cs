using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidiParser
{
    class Note
    {
        private int _notePitch;
        public int NotePitch { get => _notePitch; private set => _notePitch = value; }
        private double _timeStart;
        public double TimeStart { get => _timeStart; private set => _timeStart = value; }
        private double _length;
        public double Length { get => _length; private set => _length = value; }
        private int _velocity;
        public int Velocity { get => _velocity; private set => _velocity = value; }

        public Note (int note, double timeStart, double length, int velocity, int tpqn)
        {
            NotePitch = note;
            TimeStart = Math.Round(timeStart*tpqn, 4);
            Length = Math.Round(length*tpqn, 4);
            Velocity = velocity;
        }

        public static double ToSeconds (long time, TempoEvent lastTempoEvent, int ticksPerQuarterNote)
        {
            return (double)(((double)(time - lastTempoEvent.AbsoluteTime) / ticksPerQuarterNote) * lastTempoEvent.MicrosecondsPerQuarterNote + lastTempoEvent.AbsoluteTime) / 1000000;
        }
    }
}
