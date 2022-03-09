using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                        ConvertAllFiles(args);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception was thrown:\n{e.Message}\n\nPress enter to quit.");
                Console.ReadLine();
            }
        }

        private static void ConvertAllFiles (string[] files)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string file in files)
            {
                sb.AppendLine(ConvertFile(file));
            }
            sb.Remove(sb.Length - 1, 1);

            File.WriteAllText(Path.Combine(Path.GetDirectoryName(files[0]), "__AllSongs.txt"), sb.ToString());
        }

        private static string ConvertFile (string file)
        {
            //Make sure the file exists
            if (File.Exists(file))
            {
                string path = file;
                string output = Path.Combine(Path.GetDirectoryName(path), "_" + Path.GetFileNameWithoutExtension(path) + " - Converted.txt");
                MidiFile mid;

                //If an error is thrown, notify user and continue looping
                try
                {
                    mid = new MidiFile(path);
                    int ticksPerQuarterNote = mid.DeltaTicksPerQuarterNote;
                    int exportTPQN = 1; //Currently has no definite usage, can use MIDI PPQ as long as the numbers are small

                    List<TempoEvent> tempoEvents = new List<TempoEvent>();
                    tempoEvents.Add(new TempoEvent(500000, 0));

                    MidiEvent[][] midiEvents = new MidiEvent[mid.Events.Count()][];

                    for (int i = 0; i < midiEvents.Length; i++)
                    {
                        midiEvents[i] = mid.Events.ElementAt(i).ToArray();
                    }

                    List<Note> notes = new List<Note>();

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

                    //Iterate through every note press in the file
                    for (int i = 0; i < midiEvents.Length; i++)
                    {
                        //Skip drums - This should be skipping channels, not tracks
                        //if (i == 9)
                        //{
                        //    continue;
                        //}

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

                                //Only if a note press
                                if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                                {
                                    NoteOnEvent note = midiEvent as NoteOnEvent;

                                    //If not an off note
                                    if (note.Velocity != 0)
                                    {
                                        int modTrack = i % 16;
                                        int noteVel = note.Velocity;
                                        //This code is made to simply add color to Eden's MIDI Player without implementing a track data variable.
                                        if (noteVel < 16){
                                            noteVel += modTrack;
                                        }
                                        double timeInSeconds = Note.ToSeconds(note.AbsoluteTime, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                        double lengthInSeconds = Note.ToSeconds(note.NoteLength, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                        //Add this note
                                        notes.Add(new Note(note.NoteNumber, timeInSeconds, lengthInSeconds, noteVel, exportTPQN));
                                    }
                                }
                                //If there is a tempo event
                                if (midiEvent is TempoEvent){
                                    TempoEvent tempo = midiEvent as TempoEvent;
                                    double timeInSeconds = Note.ToSeconds(tempo.AbsoluteTime, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                    double lengthInSeconds = Note.ToSeconds(0, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                    //Add tempo event, since the MIDI standard allows 0 to 127 pitch values, 128 to 255 can be used as control macros.
                                    //Tempo event is "note" 128, all tempo events have length of 0 but channel value of the tempo. Still can be optimised
                                    notes.Add(new Note(128, timeInSeconds, lengthInSeconds, Convert.ToInt32(60000000/tempo.Tempo), exportTPQN));
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Error reading note:\n{e.Message}");
                            }
                        }
                    }

                    //Sort by time in ascending order
                    Note[] sortedNotes = notes.OrderBy(o => o.TimeStart).ToArray();

                    char separateChar = '\\';
                    //String to write to the text file
                    //Start with the file name without commas as the song title
                    StringBuilder info = new StringBuilder($"{Path.GetFileNameWithoutExtension(path).Replace(separateChar.ToString(), "")}{separateChar}1{separateChar}");

                    foreach (Note n in sortedNotes)
                    {
                        //Every 4 items contains all the info for a note
                        info.Append(n.TimeStart.ToString()).Append(separateChar);
                        info.Append(n.NotePitch.ToString()).Append(separateChar);
                        info.Append(n.Length.ToString()).Append(separateChar);
                        info.Append(n.Velocity.ToString()).Append(separateChar);
                    }

                    //Remove end comma
                    info.Remove(info.Length - 1, 1);

                    //Write to a text file
                    File.WriteAllText(output, info.ToString());

                    //Console.WriteLine("\n\n****************\n\n" + info.ToString());
                    //Clipboard.SetText(info.ToString());
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
