using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MidiParser
{
    class Program
    {
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
                        if (m is TempoEvent)
                        {
                            if (tempoEvents.Last().MicrosecondsPerQuarterNote != ((TempoEvent)m).MicrosecondsPerQuarterNote)
                                tempoEvents.Add(m as TempoEvent);
                        }
                    }

                    //Iterate through every note press in the file
                    for (int i = 0; i < midiEvents.Length; i++)
                    {
                        //Skip drums
                        if (i == 9)
                        {
                            continue;
                        }

                        int currentTempoIndex = 0;

                        foreach (MidiEvent midiEvent in midiEvents[i])
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
                                    double timeInSeconds = Note.ToSeconds(note.AbsoluteTime, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                    double lengthInSeconds = Note.ToSeconds(note.NoteLength, tempoEvents[currentTempoIndex], ticksPerQuarterNote);
                                    //Add this note
                                    notes.Add(new Note(note.NoteNumber, timeInSeconds, lengthInSeconds, note.Velocity));
                                }
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
            return null;
        }
    }
}
