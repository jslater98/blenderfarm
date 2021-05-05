#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>

/*
 * stitchFrames takes a set of parameters, including a path to a set of frames, and creates and runs a command to stitch those frames into an mp4
 *
 * to compile:  gcc stitchFrames.c -o stitchFrames
 *     to run:  ./stitchFrames frameRate inPath format outPath
 * 
 *  frameRate:  frame rate the .blend file was rendered at (used as frame rate for video)
 *     inPath:  path to location of frames
 *     format:  file format of rendered images
 *    outPath:  path to store completed video (including name of video)
 *
 *    example:  ./stitchFrames 24 /home/justin.slater/tests PNG /home/justin.slater/campfireScene
 *    command:  ffmpeg -framerate 24 -pattern_type glob -i '/home/justin.slater/tests/*.png' -pix_fmt yuv420p /home/justin.slater/campfireScene.mp4
 * 
*/

int main(int argc, char *argv[]) {
    char format[16];
    strcpy(format, argv[3]); //store format arg in string for conversion
    char form[16];  //new string for converted (lowercase) format arg because c.
    for (int i = 0; i < strlen(argv[3]) + 1; i++) { 
        form[i] = tolower(format[i]);  //convert format arg to lowercase
    }
    if (strcmp(form, "open_exr") == 0) {  //change "open_exr" to "exr"
        strcpy(form, "exr");
    }
    char command[512];
    sprintf(command, "ffmpeg -framerate %s -pattern_type glob -i '%s/*.%s' -pix_fmt yuv420p %s.mp4", argv[1], argv[2], form, argv[4]); //create command
    char *cmd = command;  //need pointer to command for system() because c.
    system(cmd);  //run command
    return 0;
}