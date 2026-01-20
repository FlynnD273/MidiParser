# MidiParser - Modded (Aranara Revision)
A program to convert MIDI files into a text file to copy originally into this Scratch project: https://scratch.mit.edu/projects/406337184/

This modded variant is intended for usage for the following Scratch projects: 
1. Pen-Based Aranara MIDI Players 
2. Pen-Based Aranara MIDI Render Toolkit

You can access them [here](https://daniferous.github.io/aranara-midi-player-sb3).

## About Mod
Initially, this program intended to add support for tempo events as the original code, made by [K9ShyGuy](https://scratch.mit.edu/users/K9shyguy/), did not include tempo changes. 

After multiple iterations in the past few years, the AraMIDI format, [which you can learn more about by clicking here](https://daniferous.github.io/aranara-midi-player-sb3/versions), now includes limited support for Control Change Events (such as Volume and Expression events), and full support for Program Change Events (or Instruments). However, do know that the Instruments only involve the standard Scratch library.

An "upgraded" version of this format, [known as the Faelei MIDI Format](https://github.com/Daniferous/MidiParser/tree/Faelei), adds full Control Change and Pitch Bend Support, by using a plug-in made by [hevean](https://scratch.mit.edu/users/hevean_3/) which allows Scratch Projects to have MIDI Synth Support.

This program has been modded to allow tempo events to be included in the file. This will make the code incompatible with the original Scratch project made by K9ShyGuy. This converter is only usable for Aranara MIDI Player or MIDI Players using Aranara MIDI Format.

## How to use
The compiled executable can be found [here](../MidiParser/bin/Release/netcoreapp3.1/publish):

There's a green button in the upper left area of the page. Click on that, then click "Download ZIP" Extract the zip file, then go to MidiParser >> bin >> Debug >> netcoreapp3.1 You can either run the exe file and paste in the MIDI file path, or you can drag MIDI files onto the exe, and convert them that way. I find the 2nd method easier.

Once finished, the converted MIDI will show up as `!(name) - converted.aramidi`. This file can now be imported to any Aranara MIDI Compatible Program in two methods.

### Method 1 (File Drag and Drop)
This is by far the easiest method and is seen in the latest versions for [Aranara MIDI Player 2.3.8](https://daniferous.github.io/aranara-midi-player-sb3/amp/Aranara%20MIDI%20Player%202.3.8.html) and [Aranara MIDI Render Toolkit](https://daniferous.github.io/aranara-midi-player-sb3/amrt/Aranara%20MIDI%20Render%20Toolkit%201.6.html).

Simply click the green `Import` button and a file selector will open. You can then navigate to the converted AraMIDI file and open it and have it loaded. If the file selector does not show, simply drag the converted AraMIDI file to the center of the program and it will open and load the AraMIDI file.

### Method 2 (Open File)
For older Aranara MIDI Programs, it may rely on a text prompt to import your MIDIs. This is inefficient, as loading large MIDIs would be restrictive and lead to memory issues. Additionally, it lead to the reason for developing a [MIDI Splitter Tool](https://github.com/daniferous/PythonConvertedMIDISplitter) which made "slices" of each AraMIDI.

Before opening the MIDI Program, navigate to the converted AraMIDI file and open it as if it were a text file. Copy everything in that text file.

Once again, click the green `Import` button and a text prompt will open, requesting you enter or paste the AraMIDI file you wish to open. You then paste everything into the dialog box then press `Enter`. 

Alternatively, you can drag and drop the converted AraMIDI file to the dialog box and then press `Enter`. 

### Batch Conversion

This modded version supports Batch Modification. Dropping multiple files will output multiple converted files.

You may need to install DotNet Core Runtime: https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.10-windows-x64-installer

Also, if you have any inquiries regarding this modification, feel free to contact me through my email, my [Scratch Profile](https://scratch.mit.edu/users/OjasnGamer101/), and the [Daniferousity Discord Server](https://discord.gg/kTD8y6YDjJ). 

## Credits
[FlynnD273](https://github.com/FlynnD273/MidiParser) for the original code.

Daniferous for the tempo events, MIDI channel,  Volume and Expresion Control Change implementation, and Instrument Integration.