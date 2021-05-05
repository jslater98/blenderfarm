import psycopg2
import psycopg2.extras
import asyncio
import asyncssh
import sys
import paramiko
import os
import functools
from concurrent.futures import ThreadPoolExecutor
from random import randint
from config.config import config
from dbOperations import *
from datetime import datetime, timezone

###### ASYNC FUNCTIONS ######

async def useNode(node, cur):
  while True:
    try:
      partitions = await loop.run_in_executor(executor,
        functools.partial(dbGetPartitionQueue,
          cursor = cur,
          size = len(nodes)
        ))
      
      for partition in partitions:
        pid = partition[0]
        if pid not in nodes.values():
          nodes[node] = pid
          dprint("%s: Reserving partition %s for processing..." % (node, pid))
          break
      
      if nodes[node] is not None:
        pinfo = await loop.run_in_executor(executor,
          functools.partial(dbGetPartitionRenderInfoByID,
            cursor = cur,
            partitionID = pid
          ))
        
        startF = pinfo[0]
        numF = pinfo[1]
        jump = pinfo[2]
        jid = pinfo[3]
        ver = pinfo[4]
        eng = pinfo[5]
        imgType = pinfo[6]
        
        await runCmd(node,
          "./render %s %s %s %s %s %s %s"
          % (jid, startF, numF, jump, ver, eng, imgType))
        
        await loop.run_in_executor(executor,
          functools.partial(dbUpdatePartition,
            cursor = cur,
            partitionID = pid,
            isComplete = True
          ))
        
        isRendered = await loop.run_in_executor(executor, functools.partial(dbIsJobRendered, cursor = cur, jobID = jid))
        
        if isRendered:
          dprint("%s: Job %s completely rendered, stitching together..." % (node, jid))
          job = await loop.run_in_executor(executor,
            functools.partial(dbGetJobByID,
              cursor = cur,
              jobID = jid
            ))
          
          fps = job["fps"]
          imgFormat = job["outputimagetype"].lower()
          jobDir = "/opt/lablibs/blender/data/%s" % jid
          
          if imgFormat == "open_exr": imgFormat = "exr"
          
          await runCmd(node,
            "ffmpeg -framerate %s -pattern_type glob -i '%s/frames/*.%s' -pix_fmt yuv420p %s/frames.mp4"
            % (fps, jobDir, imgFormat, jobDir))
          
          await runCmd(node,
            "zip -r -j %s/frames.zip %s/frames"
            % (jobDir, jobDir))
          
          await loop.run_in_executor(executor,
            functools.partial(dbUpdateJob,
              cursor = cur,
              jobID = jid,
              completedTime = datetime.now(timezone.utc)
            ))
          
          dprint("%s: Job %s frames and video are ready for download" % (node, jid))
        dprint("%s: Successfully rendered partition %s" % (node, pid))
      else:
        dprint("%s: No available partitions, shutting down node..." % node)
        break
      
    except Exception as e:
      dprint("%s: Shutting down due to error: %s" % (node, str(e)))
      break
      
    nodes[node] = None
    con.commit()
    
  con.commit()
  nodes[node] = -1
  
  safeExit = True
  for node in nodes:
    safeExit = (nodes[node] == -1)
    if not safeExit:
      break
      
  if safeExit:
    print("\n\nALL TASKS ENDED, RUNNING SAFE EXIT (IGNORE EXCEPTION BELOW)\n\n\n\n\n\n")
    await asyncio.sleep(1)
    quit()

async def runCmd(node, cmd):
  dprint("%s: Running %s..." % (node, cmd))
  async with asyncssh.connect(node, port = 22, username = "blenderssh", password = "5h62sT9Aq4", known_hosts=None) as ssh:
    result = await ssh.run(cmd)
    if result.exit_status != 0:
      raise Exception("Node-side error: %s" % result.stderr)
  
####### SYNC FUNCTIONS ######

def dprint(msg):
  if DEBUG_PRINTING: print(msg)
  
####### MAIN FUNCTION #######

nodes = {}
DEBUG_PRINTING = True
executor = ThreadPoolExecutor(1)
    
loop = asyncio.get_event_loop()

with psycopg2.connect(**config("database.ini")) as con:
  with open("config/renderNodes.txt") as f:
    nodes = {line.rstrip(): None for line in f}

  for node in nodes:
    loop.create_task(useNode(node, con.cursor(cursor_factory = psycopg2.extras.DictCursor)))
    
  loop.run_forever()

