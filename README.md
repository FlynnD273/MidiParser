# MidiParser - Modded
A program to convert MIDI files into a text file to copy originally into this Scratch project: https://scratch.mit.edu/projects/406337184/
This modded variant is intended for usage for this Scratch project: https://scratch.mit.edu/projects/633610677/

## About Mod
This program has been modded to allow tempo events to be included in the text file. This will make the code incompatible with the Scratch project listed above. This is intended for ScratchPFA, another Scratch Project which uses the same text file.

## How to use
The compiled executable can be found [here](../master/MidiParser/bin/Debug/netcoreapp3.1):

There's a green button in the upper left area of the page. Click on that, then click "Download ZIP" Extract the zip file, then go to MidiParser >> bin >> Debug >> netcoreapp3.1 You can either run the exe file and paste in the MIDI file path, or you can drag MIDI files onto the exe, and convert them that way. I find the 2nd method easier.
A text file will appear, and you want to open the text file and copy everything in that text file. 

In Scratch, since this is a modded program, the original Scratch project wouldn't be able to read the contents of the text file properly. Instead, make sure you are in ScratchPFA since it accepts the modded code. Simply start the Scratch Project and choose the Import function. Then, simply paste everything from the text file into the text box and press enter.

If you drag multiple files onto the exe file, you'll get a file called \_\_AllSongs.txt. However, Scratch PFA currently only accepts one song at a time. This multiple MIDI feature will be available soon, so stay tuned!

You may need to install DotNet Core Runtime: https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.10-windows-x64-installer

## Credits
FlynnD273 for the original code
Eden for the tempo events implementation