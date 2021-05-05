import paramiko
import os
import stat
import time
import math
from pathlib import posixpath
from stat import S_ISDIR

protected_filenames = ["blender_version"]

def clearEnv(sftp, path="/project/blender/"):
  emptyDir = True
  for f in sftp.listdir_attr(path):
    if f.filename in protected_filenames:
      emptyDir = False
    else:
      filepath = posixpath.join(path, f.filename)
      if not stat.S_ISDIR(f.st_mode):
        sftp.remove(filepath)
      elif clearEnv(sftp, filepath):
        sftp.rmdir(filepath)
  return emptyDir

def setEnv(sftp, pathFrom=posixpath.join(os.getcwd(), "node_files")):
  for f in os.listdir(pathFrom):
    if f in protected_filenames:
      continue
    fullPath = posixpath.join(pathFrom, f)
    if os.path.isdir(fullPath):
      try:
        sftp.chdir(f)
      except IOError:
        sftp.mkdir(f)
        sftp.chdir(f)
      setEnv(sftp, fullPath)
      sftp.chdir("..")
    else:
      sftp.put(fullPath, f)


### MAIN FUNCTION BELOW ###
totalStartTime = math.floor(time.time())

with open("config/renderNodes.txt") as f:
  nodes = {line.rstrip(): 0 for line in f}
  
for node in nodes:
  
  with paramiko.Transport((node, 22)) as transport:
    transport.connect(username="blenderssh", password="5h62sT9Aq4")
    with paramiko.SFTPClient.from_transport(transport) as sftp:
    
      print("Removing files in %s ... " % node, end="", flush=True)
      startTime = math.floor(time.time() * 1000)
      clearEnv(sftp)
      endTime = math.floor(time.time() * 1000)
      print("Done (%s ms)" % (endTime - startTime))
      
      print("Creating files in %s ... " % node, end="", flush=True)
      startTime = math.floor(time.time() * 1000)
      setEnv(sftp)
      endTime = math.floor(time.time() * 1000)
      print("Done (%s ms)" % (endTime - startTime))

  print("Setting perms for %s ... " % node, end="", flush=True)
  startTime = math.floor(time.time() * 1000)
  with paramiko.SSHClient() as ssh:
    ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())
    ssh.connect(node, username="blenderssh", password="5h62sT9Aq4")
    ssh.exec_command("chmod -R 750 .")
  endTime = math.floor(time.time() * 1000)
  print("Done (%s ms)" % (endTime - startTime), end="\n\n")
      
totalEndTime = math.floor(time.time())

print("All updates complete - total time elapsed: %s sec" % (totalEndTime - totalStartTime))
