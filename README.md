How the idea appeared
=====================
Some time ago I needed to free up some space on my Google Drive. 
So I decided to extract all the pictures I have there. I uses Google Takeout process for this.
Later I bought more space on Google Drive so I decided to put extracted images back there.

But it seems that when you upload picture to Google photos, and then you amend the Taked Date image attribute, Google does not change the image file itself.
It just creates some meta information connected to that image file somewhere.
During the Takeout process you get images which maybe still with the wrong Date Taken attribute. 
Google also suppolies meta information in the corresponding *.json files. 

So that every image.jpeg has a corresponding image.jpeg.json file when you make a Takeout process.
When you put the image.jpeg back to Google Photos, Google sees the opriginal Taken Date image attribute, and I could not find any means to supply Google back with the information from the *.json file.

So I decided to modify the original image.jpeg files with the metadata supplied in *.json file and then upload the updated image.jpeg to Google Photos. 

How this program works
======================
This command line executable makes the following: 

1. Takes all the JPEG/JPG/jpeg/jpg files in the current directory
2. For each of the file it seeks the corresponding *.json file, created by Google, in the same directory.
3. It extracts "photoTakenTime" / "timestamp" attribute from the *.json file
4. It creates the new _u.jpeg file with the meta-attributes set to the correct taken date property.

There may be some exceptions which I did not catch. 
Also it works on Windows, not sure about *nix platforms. 

Anton Permyakov
07.01.2025
Budva
