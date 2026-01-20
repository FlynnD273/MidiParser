using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
//using System.Windows.Forms;
using System.Threading;

namespace MidiParser
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Iterate through all files dropped onto the program
            try
            {
                switch (args.Length)
                {
                    case 1:
                        ConvertFile(args[0]);
                        break;
                    case 0:
                        while (true)
                        {
                            Console.WriteLine("Paste file path to convert here (Enter a blank line to quit):");
                            string s = Console.ReadLine();
                            if (s != "")
                                ConvertFile(s);
                            else
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine("Batch Convert function is partially supported.");
                        //Console.ReadLine();
                        ConvertAllFiles(args);
                        break;
                }
                //Post Operation.
                Console.WriteLine("Finished process.\nIf this window does not close automatically, press enter to end.");
                //Console.ReadLine(); //Leaving this here for the option to have the window not close automatically.
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception was thrown:\n{e.Message}\n\nPress enter to quit.");
                Console.ReadLine();
            }
        }

        //An Attempt to make Batch Conversion Possible
        private static void ConvertAllFiles (string[] files)
        {
            int count = 1;
            foreach (string file in files)
            {
                ConvertFile(file);
                Console.WriteLine($"Finished file {count++}");
            }
        }

        private static string ConvertFile (string file)
        {
            //Make sure the file exists
            if (File.Exists(file))
            {
                string path = file;
                string output = Path.Combine(Path.GetDirectoryName(path), "!" + Path.GetFileNameWithoutExtension(path) + ".aramidi");
                MidiFile mid;

                //If an error is thrown, notify user and continue looping
                try
                {
                    mid = new MidiFile(path);
                    int ticksPerQuarterNote = mid.DeltaTicksPerQuarterNote;
                    //MIDI TPQ is 384, though is processed as TPH (Ticks per half-note)
                    int prctmp = 120; //Formula using 120 BPM: TPQ * (prctmp/60)
                    int outTPQ = 384 * (prctmp/60); //768 TPH
                    
                    List<TempoEvent> tempoEvents = new List<TempoEvent>();
                    tempoEvents.Add(new TempoEvent(60000000/prctmp, 0)); //Assumes Tempo is 120 BPM, converts to seconds at 120 BPM

                    MidiEvent[][] midiEvents = new MidiEvent[mid.Events.Count()][];

                    for (int i = 0; i < midiEvents.Length; i++)
                    {
                        midiEvents[i] = mid.Events.ElementAt(i).ToArray();
                    }

                    List<AranaraN> notes = new List<AranaraN>();

                    foreach (MidiEvent m in midiEvents[0])
                    {
                        try
                        {
                            if (m is TempoEvent)
                            {
                                if (tempoEvents.Last().MicrosecondsPerQuarterNote != ((TempoEvent)m).MicrosecondsPerQuarterNote)
                                    tempoEvents.Add(m as TempoEvent);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error reading tempos:\n{e.Message}");
                        }
                    }

                    //Create a list containing default Expression, Volume, and Sustain values. 18 Entries in case.
                    int[] MIDIVol = Enumerable.Repeat(127, 16).ToArray();
                    int[] MIDIExp = Enumerable.Repeat(127, 16).ToArray();

                    //Iterate through every note press in the file
                    for (int i = 0; i < midiEvents.Length; i++)
                    {
                        //Track Header
                        notes.Add(new AranaraN("TR",0,0,0,0,i,outTPQ));

                        //Assume no tempo events in track 0. Default tempo is 120 (0x07A120 ms per beat)
                        if (i == 0){
                            notes.Add(new AranaraN("TE",0,0,0,0,500000,outTPQ));
                        }

                        int currentTempoIndex = 0;

                        foreach (MidiEvent midiEvent in midiEvents[i])
                        {
                            try
                            {
                                if (currentTempoIndex < tempoEvents.Count - 1)
                                {
                                    while (tempoEvents[currentTempoIndex + 1].AbsoluteTime < midiEvent.AbsoluteTime)
                                    {
                                        if (currentTempoIndex < tempoEvents.Count - 1)
                                            break;

                                        currentTempoIndex++;
                                    }
                                }

                                //Test: Switch Cases
                                double timeInSeconds;
                                double lengthInSeconds;
                                switch(midiEvent.CommandCode)
                                {
                                    case MidiCommandCode.ControlChange:
                                        ControlChangeEvent midicc = midiEvent as ControlChangeEvent;
                                        int MIDICCRawValue = short.Parse(midicc.ControllerValue.ToString());
                                        int MIDICCChannel = short.Parse((midicc.Channel%16).ToString());
                                        switch(midicc.Controller.ToString())
                                        {
                                            case "MainVolume":
                                                MIDIVol[MIDICCChannel] = MIDICCRawValue;
                                            break;
                                            case "Expression":
                                                MIDIExp[MIDICCChannel] = MIDICCRawValue;
                                            break;
                                            default:
                                            break;
                                        }
                                    break;
                                    case MidiCommandCode.PatchChange:
                                        PatchChangeEvent midipc = midiEvent as PatchChangeEvent;
                                        int MIDIPCRaw = ((short)midipc.Patch);

                                        
                                        timeInSeconds = AranaraN.ToSeconds(midipc.AbsoluteTime, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                        lengthInSeconds = AranaraN.ToSeconds(0, tempoEvents[currentTempoIndex], ticksPerQuarterNote); //Not quite needed
                                        
                                        //Add this instrument change
                                        notes.Add(new AranaraN("PC",MIDIPCRaw,0,midipc.Channel%16,timeInSeconds,0,outTPQ));
                                    break;
                                    case MidiCommandCode.NoteOn:
                                        NoteOnEvent note = midiEvent as NoteOnEvent;

                                        //If not an off note
                                        if (note.Velocity != 0)
                                        {
                                            timeInSeconds = AranaraN.ToSeconds(note.AbsoluteTime, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                            lengthInSeconds = AranaraN.ToSeconds(note.NoteLength, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                           
                                            //Add this note
                                            notes.Add(new AranaraN("N",note.NoteNumber,Convert.ToInt32(note.Velocity * (MIDIVol[note.Channel%16] * MIDIExp[note.Channel%16]) / 16129),note.Channel%16,timeInSeconds,lengthInSeconds,outTPQ));
                                        }
                                    break;
                                    default:
                                        //Tempo Event Detection
                                        if (midiEvent is TempoEvent)
                                        {
                                            TempoEvent tempo = midiEvent as TempoEvent;
                                            timeInSeconds = AranaraN.ToSeconds(tempo.AbsoluteTime, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                            lengthInSeconds = AranaraN.ToSeconds(0, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                            //Add Tempo Event
                                            notes.Add(new AranaraN("TE",0,0,0,timeInSeconds,60000000/tempo.Tempo,outTPQ));
                                        }
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Error reading note:\n{e.Message}");
                            }
                        }
                    }

                    //Aranara MIDI File Parser
                    AranaraN[] events = notes.ToArray();

                    char separateChar = '|'; //Only used for initialising file header.
                    string header = "[Aranara]█";
                    StringBuilder info = new StringBuilder($"{header}{(Path.GetFileNameWithoutExtension(path) + $"{":"}{outTPQ}").Replace(separateChar.ToString(), "")}{separateChar}");
                    

                    foreach (AranaraN n in events)
                    {
                        info.Append(n.hex_type);
                        info.Append(n.hex_note);
                        info.Append(n.hex_vel);
                        info.Append(n.hex_ch);
                        info.Append(n.hex_value);
                        info.Append(n.hex_time);
                        info.Append(n.hex_len);
                    }
                    //Write to a text file
                    File.WriteAllText(output, info.ToString());
                    
                    return info.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Could not process file {file}\n{e.Message}\nPress enter to continue");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine($"File {file} does not exist.\nPress enter to continue");
                Console.ReadLine();
            }
            return "";
        }
    }
}