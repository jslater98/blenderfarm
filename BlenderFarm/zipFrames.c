#include <stdio.h>
#include <stdlib.h>
#include <string.h>

/*
 * compressFrames takes a folder of rendered frames and compresses it into a .zip
 *
 * to compile:  gcc zipFrames.c -o stitchFrames
 *     to run:  ./zipFrames fileName path
 * 
 *   fileName:  full path of .zip file to be created (including job name)
 *       path:  path to folder to be zipped (folder containing rendered images)
 *
 *    example:  ./zipFrames /someDirectory/storeZipsHere/campfireScene /home/justin.slater/tests
 *    command:  zip -r -j /someDirectory/storeZipsHere/campfireScene.zip /home/justin.slater/tests
 *
*/

int main(int argc, char *argv[]) {
	char command[256];
	sprintf(command, "zip -r -j %s.zip %s", argv[1], argv[2]); //create command
	char *cmd = command; //need pointer to command for system() because c.
	system(cmd); //run command
    return 0;
}	