# MidiParser - Modded
A program to convert MIDI files into a text file to copy originally into this Scratch project: https://scratch.mit.edu/projects/406337184/

This modded variant is intended for usage for the following Scratch projects: 
1. Pen-Based (New Eden MIDI Player, Slow, Outdated, but has more settings) https://scratch.mit.edu/projects/722655492/
2. Pen-Based (KazuMIDI Player, Faster, Newest, but very basic in design) (Link coming soon!)
3. Pen-Based (Aranara MIDI Player, currently WIP and uses the Aranara MIDI File Format)

## About Mod
This program has been modded to allow tempo events to be included in the text file. This will make the code incompatible with the original Scratch project made by K9ShyGuy. However, this program can still allow conversion to the original MIDI program by K9ShyGuy and current and future MIDI programs either finished or in development by OjasnRCGamer101/Eden/Danify.

## Available File Formats
1. Original K9ShyGuy Format (Intended only for the original project.)
2. Modified Format for Eden/Ojasn MIDI Players 
3. Modified Format for Eden/Ojasn MIDI Players \[Hex Values\] (Only compatible with KazuMIDI 1.2.x, only benefit is each file takes lesser space.)
4. Aranara MIDI File Format for Aranara MIDI Player (MIDI Player is currently WIP, Only compatible with said MIDI Player.)

## How to use
The compiled executable can be found [here](../Aranara/MidiParser/bin/Debug/netcoreapp3.1/publish):

There's a green button in the upper left area of the page. Click on that, then click "Download ZIP" Extract the zip file, then go to MidiParser >> bin >> Debug >> netcoreapp3.1 You can either run the exe file and paste in the MIDI file path, or you can drag MIDI files onto the exe, and convert them that way. I find the 2nd method easier.
A text file will appear, and you want to open the text file and copy everything in that text file. 

In Scratch, since this is a modded program, the original Scratch project wouldn't be able to read the contents of the text file properly. Instead, make sure you are in ScratchPFA/KazuMIDI/New Eden MIDI Player since it accepts the modded code. Simply start the Scratch Project and choose the Import function. Then, simply paste everything from the text file into the text box and press enter.

If you drag multiple files onto the exe file, you'll get a file called \_\_AllSongs.txt. This feature only works for K9ShyGuy's project.

You may need to install DotNet Core Runtime: https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.10-windows-x64-installer

## Credits
FlynnD273 for the original code

Eden for the tempo events, MIDI channel,  Volume and Expresion Control Change implementation, and Instrument Integration