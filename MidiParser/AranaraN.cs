using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidiParser
{
    class AranaraN
    {
        private string ahex_note;
        public string hex_note {get => ahex_note; private set => ahex_note = value;}
        private string ahex_vel;
        public string hex_vel {get => ahex_vel; private set => ahex_vel = value;}
        private string ahex_ch;
        public string hex_ch {get => ahex_ch; private set => ahex_ch = value;}
        private string ahex_value;
        public string hex_value {get => ahex_value; private set => ahex_value = value;}
        private string ahex_time;
        public string hex_time {get => ahex_time; private set => ahex_time = value;}
        private string ahex_len;
        public string hex_len {get => ahex_len; private set => ahex_len = value;}
        private string ahex_type;
        public string hex_type {get => ahex_type; private set => ahex_type = value;}

        public AranaraN (string htype, int hnote, int hvel, int hch, double htime, double hlen, int htpqn)
        {
            switch (htype){ //Len parameter for function AranaraN used for values for non-note events.
                case "TR": //Track
                    hex_type = "F";
                    hex_note = ""; //Unused Parameter for Track Headers
                    hex_vel = "";
                    hex_ch = "";
                    hex_value = Convert.ToInt32(hlen).ToString("X") + "|"; 
                    hex_time = "";
                    hex_len = "";
                    break;

                case "TE": //Tempo
                    hex_type = "E";
                    hex_note = ""; //Unused Parameter for Tempo Events
                    hex_vel = "";
                    hex_ch = "";
                    hex_value = Convert.ToInt32(hlen).ToString("X") + "|"; 
                    hex_time = Convert.ToInt32(Math.Round(htime*htpqn,0)).ToString("X") + "|";
                    hex_len = "";
                    break;

                default: //Assume Note
                    hex_type = "";
                    hex_note = hnote.ToString("X2"); 
                    hex_vel = hvel.ToString("X2");
                    hex_ch = hch.ToString("X");
                    hex_value = ""; //Unused Parameter for Notes
                    hex_time = Convert.ToInt32(Math.Round(htime*htpqn,0)).ToString("X") + "|";
                    hex_len = Convert.ToInt32(Math.Round(hlen*htpqn,0)).ToString("X") + "|";
                    break;
            }
        }
    }
}