# MidiParser - Modded (Aranara Revision)
A program to convert MIDI files into a text file to copy originally into this Scratch project: https://scratch.mit.edu/projects/406337184/

This modded variant is intended for usage for the following Scratch projects: 
1. Pen-Based Aranara MIDI Players (Both Lite and Regular Versions)

## About Mod
This program has been modded to allow tempo events to be included in the file. This will make the code incompatible with the original Scratch project made by K9ShyGuy. This converter is only usable for Aranara MIDI Player or MIDI Players using Aranara MIDI Format.

## How to use
The compiled executable can be found [here](../MidiParser/bin/Release/netcoreapp3.1/publish):

There's a green button in the upper left area of the page. Click on that, then click "Download ZIP" Extract the zip file, then go to MidiParser >> bin >> Debug >> netcoreapp3.1 You can either run the exe file and paste in the MIDI file path, or you can drag MIDI files onto the exe, and convert them that way. I find the 2nd method easier.
A text file will appear, and you want to open the text file and copy everything in that text file. 

In Scratch, since this is a modded program, the original Scratch project wouldn't be able to read the contents of the text file properly. Instead, make sure you are Aranara MIDI Player or any MIDI Player that accepts Aranara MIDI File format since it accepts the modded code. Simply start the Scratch Project and choose the Import function. Then, simply paste everything from the text file into the text box and press enter. Some variations can support file drag-and-drop methods.

This modded version no longer supports Batch Conversion (Convert multiple songs and merge all into a single list) for now.

You may need to install DotNet Core Runtime: https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.10-windows-x64-installer

## Credits
FlynnD273 for the original code

Eden for the tempo events, MIDI channel,  Volume and Expresion Control Change implementation, and Instrument Integration