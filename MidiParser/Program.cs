using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
//using System.Threading;

namespace MidiParser
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
        
            //Credits
            Console.WriteLine("MIDIParser by K9ShyGuy - Modified by Eden/Danify/OjasnGamer101\nRevision 5 - 27 August 2022\n\nType the number corresponding to an Import Option then press Enter to initialize conversion.\n\n\t[1] - K9ShyGuy's MIDI Player\n\t[2] - Danify's MIDI Players\n\t[3] - Danify's MIDI Players [HEX] (BETA)\n\t[4] - Aranara MIDI (BETA)\n\n");
            double importMode = Convert.ToDouble(Console.ReadLine());
            if (importMode % 1 !=0 || Math.Ceiling(importMode/4) != 1)
            {
                Console.WriteLine($"\nInvalid input.\n\nPress enter to quit.");
                Console.ReadLine();
                return;
            }
            
            //Pending Code Rewrite
            
            importMode -= 1;
            importMode *= 2;
            importMode += 1;
            
            //Iterate through all files dropped onto the program
            try
            {
                switch (args.Length)
                {
                    case 1:
                        ConvertFile(args[0],importMode);
                        break;
                    case 0:
                        while (true)
                        {
                            Console.WriteLine("Paste file path to convert here (Enter a blank line to quit):");
                            string s = Console.ReadLine();
                            if (s != "")
                                ConvertFile(s,importMode);
                            else
                                break;
                        }
                        break;
                    default:
                        ConvertAllFiles(args,importMode);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception was thrown:\n{e.Message}\n\nPress enter to quit.");
                Console.ReadLine();
            }
        }

        private static void ConvertAllFiles (string[] files, double importMode)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string file in files)
            {
                sb.AppendLine(ConvertFile(file,importMode));
            }
            sb.Remove(sb.Length - 1, 1);

            File.WriteAllText(Path.Combine(Path.GetDirectoryName(files[0]), "__AllSongs.txt"), sb.ToString());
        }

        private static string ConvertFile (string file, double importMode) 
        {
            //Make sure the file exists
            if (File.Exists(file))
            {
                string path = file;
                string output = "";

                switch(importMode){
                    case 1:
                        output = Path.Combine(Path.GetDirectoryName(path), "_" + Path.GetFileNameWithoutExtension(path) + " - Converted.txt");
                        break;
                    case 3:
                        output = Path.Combine(Path.GetDirectoryName(path), "_" + Path.GetFileNameWithoutExtension(path) + " - Converted.spfa");
                        break;
                    case 5:
                        output = Path.Combine(Path.GetDirectoryName(path), "_" + Path.GetFileNameWithoutExtension(path) + " - Converted.shmd");
                        break;
                    case 7:
                        output = Path.Combine(Path.GetDirectoryName(path), "!" + Path.GetFileNameWithoutExtension(path) + ".aramidi");
                        break;
                }

                MidiFile mid;

                //If an error is thrown, notify user and continue looping
                try
                {
                    mid = new MidiFile(path);
                    int ticksPerQuarterNote = mid.DeltaTicksPerQuarterNote;
                    int exportTPQN = Convert.ToInt32(importMode==1) + Convert.ToInt32(importMode!=1) * 768;
                    int channelGet = 0;

                    List<TempoEvent> tempoEvents = new List<TempoEvent>();
                    tempoEvents.Add(new TempoEvent(500000, 0));

                    MidiEvent[][] midiEvents = new MidiEvent[mid.Events.Count()][];

                    for (int i = 0; i < midiEvents.Length; i++)
                    {
                        midiEvents[i] = mid.Events.ElementAt(i).ToArray();
                    }

                    
                    List<Note> notes = new List<Note>(); //somehow nesting this in an if-else statement breaks everything
                    List<AranaraN>aran = new List<AranaraN>();
                   
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
                    /*
                    int[] MIDIVol = new int[] {127,127,127,127,127,127,127,127,127,127,127,127,127,127,127,127,127,127};
                    int[] MIDIExp = new int[] {127,127,127,127,127,127,127,127,127,127,127,127,127,127,127,127,127,127};
                    int[] MIDISus = new int[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
                    */

                    int[] MIDIVol = Enumerable.Repeat(127, 18).ToArray();
                    int[] MIDIExp = Enumerable.Repeat(127, 18).ToArray();
                    int[] MIDISus = Enumerable.Repeat(0, 18).ToArray();

                    //Iterate through every note press in the file
                    for (int i = 0; i < midiEvents.Length; i++)
                    {
                        
                        //Track Header - Separates notes into its own separate tracks

                        if (importMode != 1)
                        {
                            if (importMode == 7)
                            {
                                aran.Add(new AranaraN("TR",0,0,0,0,i,exportTPQN));
                            }
                            else
                            {
                                notes.Add(new Note(i, -1, 0, 0, exportTPQN));
                            }
                        }
                        
                        
                        int currentTempoIndex = 0;
                        channelGet = 1;

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

                                //If Control Change - WIP (Not yet implemented)
                                
                                if (midiEvent.CommandCode == MidiCommandCode.ControlChange)
                                {
                                    ControlChangeEvent midicc = midiEvent as ControlChangeEvent;
                                    //Console.WriteLine("Controller: "+ midicc.Controller + "\nValue: "+ midicc.ControllerValue);
                                    string MIDICCRaw = midicc.Controller.ToString();
                                    int MIDICCRawValue = Int16.Parse(midicc.ControllerValue.ToString());
                                    int MIDICCChannel = Int16.Parse(midicc.Channel.ToString());
                                    if (MIDICCRaw == "MainVolume"){
                                        MIDIVol[MIDICCChannel] = MIDICCRawValue;
                                    }
                                    if (MIDICCRaw == "Expression"){
                                        MIDIExp[MIDICCChannel] = MIDICCRawValue;
                                    }
                                }

                                //If Program Change (Only for Aranara MIDI)
                                if (importMode == 7)
                                {
                                    if (midiEvent.CommandCode == MidiCommandCode.PatchChange)
                                    {
                                        PatchChangeEvent midipc = midiEvent as PatchChangeEvent;
                                        int MIDIPCRaw = ((short)midipc.Patch);
                                        Console.WriteLine("MIDI Program Change: " + MIDIPCRaw.ToString());

                                        //Instrument Time
                                        double timeInSeconds = Note.ToSeconds(midipc.AbsoluteTime, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                        double lengthInSeconds = Note.ToSeconds(0, tempoEvents[currentTempoIndex], ticksPerQuarterNote); //Not quite needed

                                        aran.Add(new AranaraN("PC",MIDIPCRaw,0,midipc.Channel,timeInSeconds,0,exportTPQN));
                                    }
                                }

                                //Only if a note press
                                if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                                {
                                    NoteOnEvent note = midiEvent as NoteOnEvent;

                                    //If not an off note
                                    if (note.Velocity != 0)
                                    {
                                        //Adjusts volumes based on MIDI CC.
                                        double noteVelCC = (MIDIVol[note.Channel]) * (MIDIExp[note.Channel]);
                                        double noteVel = note.Velocity * noteVelCC/16129;

                                        

                                        //Gets the channel data 
                                        if (channelGet == 1 && importMode != 1 && importMode != 7) //Not needed for K9 and Aranara formats
                                        {
                                            notes.Add(new Note(note.Channel, -1, 1, 0, exportTPQN));
                                            channelGet = 0;
                                        }

                                        //Calculates length and start times

                                        double timeInSeconds = Note.ToSeconds(note.AbsoluteTime, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                        double lengthInSeconds = Note.ToSeconds(note.NoteLength, tempoEvents[currentTempoIndex], ticksPerQuarterNote);

                                        //Add this note
                                        if (importMode != 7)
                                        {
                                            notes.Add(new Note(note.NoteNumber, timeInSeconds, lengthInSeconds, noteVel, exportTPQN));
                                        }
                                        else
                                        {
                                            aran.Add(new AranaraN("N",note.NoteNumber,Convert.ToInt32(noteVel),note.Channel,timeInSeconds,lengthInSeconds,exportTPQN));
                                        }

                                    }
                                }
                                //If there is a tempo event
                                if (midiEvent is TempoEvent){
                                    TempoEvent tempo = midiEvent as TempoEvent;
                                    double timeInSeconds = Note.ToSeconds(tempo.AbsoluteTime, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                    double lengthInSeconds = Note.ToSeconds(0, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                    //Add tempo event, since the MIDI standard allows 0 to 127 pitch values, 128 to 255 can be used as control macros.
                                    //Tempo event is "note" 128, all tempo events have length of 0 but channel value of the tempo. Still can be optimised
                                    if (importMode != 7)
                                    {
                                        notes.Add(new Note(128, timeInSeconds, lengthInSeconds, Convert.ToInt32(60000000/tempo.Tempo), exportTPQN));
                                    }
                                    else
                                    {
                                        aran.Add(new AranaraN("TE",0,0,0,timeInSeconds,60000000/tempo.Tempo,exportTPQN));
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Error reading note:\n{e.Message}");
                            }
                        }
                    }

                    
                    // Parsing Data
                    
                    if(importMode != 7)
                    {
                        //Sort by time in ascending order - Not needed for track header separation (MK9S 4+)
                        //WARNING: Disabling sortedNotes will render this program unusable to the original MIDI Player of K9ShyGuy
                        Note[] sortedNotes;
                        if (importMode == 1)
                        {
                            sortedNotes = notes.OrderBy(o => o.TimeStart).ToArray();
                        } 
                        else 
                        {
                            sortedNotes = notes.ToArray();
                        }
                        
                        char separateChar = '\\';
                        //String to write to the text file
                        //Start with the file name without commas as the song title
                        StringBuilder info = new StringBuilder($"{Path.GetFileNameWithoutExtension(path).Replace(separateChar.ToString(), "")}{separateChar}{importMode}{separateChar}");
                        
                        foreach (Note n in sortedNotes)
                        {
                            //Every 4 items contains all the info for a note
                            //Is it possible to split?
                            switch(importMode){
                                case 1: //K9ShyGuy's MIDI Player
                                    info.Append(n.TimeStart.ToString()).Append(separateChar);
                                    info.Append(n.NotePitch.ToString()).Append(separateChar);
                                    info.Append(n.Length.ToString()).Append(separateChar);
                                    info.Append(n.Velocity.ToString()).Append(separateChar);
                                    break;
                                case 3: //Danify's MIDI Player
                                    info.Append(n.TimeStart.ToString()).Append(separateChar);
                                    info.Append(n.NotePitch.ToString()).Append(separateChar);
                                    info.Append(n.Length.ToString()).Append(separateChar);
                                    info.Append(n.Velocity.ToString()).Append(separateChar);
                                    break;
                                case 5: //Experimental (HEX)
                                    info.Append(Convert.ToInt32(n.TimeStart).ToString("X")).Append(separateChar);
                                    info.Append(n.NotePitch.ToString("X")).Append(separateChar);
                                    info.Append(Convert.ToInt32(n.Length).ToString("X")).Append(separateChar);
                                    info.Append(Convert.ToInt32(n.Velocity).ToString("X")).Append(separateChar);
                                    break;
                            }
                        }

                        //Remove end comma
                        info.Remove(info.Length - 1, 1);

                        //Write to a text file
                        File.WriteAllText(output, info.ToString());

                        //Console.WriteLine("\n\n****************\n\n" + info.ToString());
                        //Clipboard.SetText(info.ToString());
                        
                        //Below is debug only.
                        Console.WriteLine("Finished process.\nIf this window does not close automatically, press enter to end.");
                        //Console.ReadLine();
                        return info.ToString();
                    }
                    else
                    {
                        //Aranara MIDI File Parser
                        AranaraN[] events = aran.ToArray();

                        char separateChar = '|'; //Only used for initialising file header.
                        StringBuilder info = new StringBuilder($"{Path.GetFileNameWithoutExtension(path).Replace(separateChar.ToString(), "")}{separateChar}");

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

                        //Console.WriteLine("\n\n****************\n\n" + info.ToString());
                        //Clipboard.SetText(info.ToString());
                        
                        //Below is debug only.
                        Console.WriteLine("Finished process.\nIf this window does not close automatically, press enter to end.");
                        //Console.ReadLine();
                        return info.ToString();
                    }
                    
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
