#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

int main(int argc, char *argv[]) {

/* 
 * this function takes 7 parameters in the order listed below, and creates and runs a render command with them
 *
 *           jobID:  ID number associated with a given job (used to find .blend file)
 *      startFrame:  first frame to render
 *       numFrames:  total number of frames to render for given partition
 * frameJumpLength:  jump value to be used
 *         version:  version of blender to be used
 *          engine:  render engine to be used (currently only supports CYCLES)
 * outputImageType:  file format used for rendered frames (PNG, JPEG, OPEN_EXR)
 *  
*/
    //variables for storing args
    char jobID[16];
    char start[16];
    char num[16];
    char jump[16];
    char version[16];
    char engine[32];
    char outputImageType[16];

    char command[512];     //render command to be run on render nodes
    char localPath[128];   //path to .blend file to be rendered in local memory (/project/blender/blend_files/jobID.blend)
	char sharedPath[128];  //path to job folder in shared memory (/opt/lablibs/blender/data/jobID)

    //store args in respective char variables
    sprintf(jobID, "%s", argv[1]);
    sprintf(start, "%s", argv[2]);
    sprintf(num, "%s", argv[3]);
    sprintf(jump, "%s", argv[4]);
    sprintf(version, "%s", argv[5]);
    sprintf(engine, "%s", argv[6]);
    sprintf(outputImageType, "%s", argv[7]);

    //convert start, num and jump to ints (startFrame, numFrames and frameJumpLength)
    int startFrame = atoi(start);
    int numFrames = atoi(num);
    int frameJumpLength = atoi(jump);
    int endFrame;

    //create endFrame val based on startFrame and frameJumpLength vals
	if (numFrames == 1) endFrame = startFrame;
	else {
        if (frameJumpLength == 0) frameJumpLength = 1;
		endFrame = startFrame + (numFrames-1)*frameJumpLength;  
	}

    //create local and shared path vars
    sprintf(localPath, "/project/blender/blend_files/%s.blend", jobID);
	sprintf(sharedPath, "/opt/lablibs/blender/data/%s", jobID);

    //check for existence of .blend file in local memory
	if (access(localPath, F_OK) != 0) {  //if .blend file not in local mem, copy it to local mem from shared mem
		char cpCmd[512];
		sprintf(cpCmd, "cp %s/*.blend %s", sharedPath, localPath);
		system(cpCmd);
	}

    //create and run render command
	sprintf(command, "/project/blender/blender_version/%s/blender -b %s -o %s/frames/frame_########### -F %s -x 1 -E %s -s %d -e %d -j %d -a -- --cycles-device CUDA+CPU", version, localPath, sharedPath, outputImageType, engine, startFrame, endFrame, frameJumpLength);
	system(command);
    return 0;
}