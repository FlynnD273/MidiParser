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
            for (int i = 0; i < args.Length; i++)
            {
                //Make sure the file exists
                if (File.Exists(args[i]))
                {
                    string path = args[i];
                    string output = Path.Combine(Path.GetDirectoryName(path), "_" + Path.GetFileNameWithoutExtension(path) + " - Converted.txt");
                    MidiFile mid;

                    //If an error is thrown, notify user and continue looping
                    try
                    {
                        mid = new MidiFile(path);

                        //Combine all MIDI layers
                        mid.Events.MidiFileType = 0;

                        List<Note> notes = new List<Note>();

                        //Iterate through every note press in the file
                        foreach (MidiEvent midiEvent in mid.Events[0])
                        {
                            //Only if a note press
                            if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                            {
                                NoteOnEvent note = (NoteOnEvent)midiEvent;

                                //If not an off note
                                if (note.Velocity != 0)
                                {
                                    //Add this note
                                    notes.Add(new Note(note.NoteNumber, note.AbsoluteTime, note.NoteLength, note.Velocity));
                                }
                            }
                        }

                        //Sort by time in ascending order
                        Note[] sortedNotes = notes.OrderBy(o => o.TimeStart).ToArray();

                        //String to write to the text file
                        StringBuilder info = new StringBuilder($"{Path.GetFileNameWithoutExtension(path)},1,");

                        foreach (Note n in sortedNotes)
                        {
                            //Every 4 items contains all the info for a note
                            info.Append(((double)n.TimeStart / 32).ToString()).Append(",");
                            info.Append(n.NotePitch.ToString()).Append(",");
                            info.Append(n.Length.ToString()).Append(",");
                            info.Append(n.Velocity.ToString()).Append(",");
                        }

                        //Remove end comma
                        info.Remove(info.Length - 1, 1);

                        //Write to a text file
                        File.WriteAllText(output, info.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Could not process file {args[i]}\n{e.Message}\nPress enter to continue");
                        Console.ReadLine();
                    }
                }
            }
        }
    }
}
