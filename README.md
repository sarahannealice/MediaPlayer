# Assignment1_SarahNewman
**MEDIA PLAYER/MP3 TAGGER**  
Design and write a program allows users to select and play an MP3 audio file, as well as edit and save some of the
MP3 file’s tagged metadata. On application start, the user can open a file dialog, browse to an MP3 audio file, and open it.  
When an MP3 has been opened, the user can use either a menu option or toolbar buttons to control the music’s playback state
(Play, Pause or Stop). After a song is selected, the primary song metadata (Title, Artist, Album, Year) shall be
displayed in a “Now Playing” screen. Once a song has been selected, the user should be able to toggle the display between the “Now Playing” screen and a tag-editing screen. If the user makes changes and saves any tag data, the changes should be written back into the MP3 file’s tag metadata.


**TECHNICAL REQUIREMENTS**  
Your solution should be built to include the following technical specs:
* WPF application using XAML and C#.
* Use CommandBindings for the media and application controls
* Use at least one User Control (Suggested use: Now Playing and Tag Editor screens)
* Use at least three layout managers to create an intuitive and flexible user interface.
* At minimum, the app should be able to read and write the following tag data: Song Title, Artist, Album, Year. Other tags can be used as desired.
* Implement reasonable exception handling to avoid program crashes.


**TECHNICAL RESOURCES**  
For accessing and editing ID3 tag metadata from the Mp3, it is suggested that you install the TagLib-Sharp package
shown below. This third-party package allows reading and writing of ID3 tag data in MP3 files. Documentation and
examples can be found on their website: http://taglib.org/api/
Important Note: If you refer to work or code from a website or other resources, whether you copy any code or
not, it MUST be cited in your work. The references provided are not intended for you to just copy-paste... you are
expected to cite anything from resource sites, and will be asked to explain how they work.
