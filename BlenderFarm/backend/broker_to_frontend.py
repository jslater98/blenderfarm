import math
import psycopg2
import psycopg2.extras
import json
import smtplib
import ssl
import sys
import getpass
import os
import glob
from shutil import rmtree
from pathlib import posixpath
from config.config import config
from dbOperations import *

def handleRequest():
  reqType = getArg("reqType")
  email = getArg("email")
  password = getArg("password")

  if reqType == "create_account":       return reqCreateAccount(email, password)

  ### TO ACCESS METHODS BELOW, USER MUST HAVE ACCOUNT
  userID = login(email, password)
  if userID == None:                    raise Exception("User does not exist (or request type does not exist)")
  
  ### CHECK IF METHODS BELOW SHOULD BE ACCESSED AS MIMIC ID (ADMIN FEATURE)
  if getArg("mimicID", -1) != -1:
    if not isAdmin(userID):             raise Exception("Non-admin account can't have mimic ID be anything other than -1")
    if not isUser(mimicID):             raise Exception("Mimic ID references a user that does not exist")       
    userID = mimicID
      
  if reqType == "verify_account":       return reqVerifyAccount(userID)
  
  ### TO ACCESS METHODS BELOW, USER MUST BE VERIFIED
  if not isVerified(userID):            raise Exception("User is not verified (or request type does not exist)")
  if reqType == "create_job":           return reqCreateJob(userID)
  if reqType == "delete_job":           return reqDeleteJob(userID)
  if reqType == "pause_job":            return reqPauseJob(userID)
  if reqType == "unpause_job":          return reqUnpauseJob(userID)
  if reqType == "get_jobs":             return reqGetJobs(userID)

  ### TO ACCESS METHODS BELOW, USER MUST BE ADMIN
  if not isAdmin(userID):               raise Exception("User is not admin (or request type does not exist)")
  if reqType == "get_accounts":         return reqGetAccounts()
  if reqType == "delete_account":       return reqDeleteAccount()
  if reqType == "privilege_account":    return reqPrivilegeAccount()
  if reqType == "admin_account":        return reqAdminAccount()
  
  ### THERE ARE NO MORE METHODS TO ACCESS
  raise Exception("Request type does not exist")

############### REQUEST METHODS ###############
 
def reqCreateAccount(email, password):
  try:
    userID = dbAddUser(cur, email, password)
  except ValueError as ve:
    raise Exception(str(ve))
  except:
    raise Exception("User already exists (or there was a connection error accessing the database)")
  vCode = dbGetUserByID(cur, userID)["verificationcode"]
  sendVerificationEmail(email, vCode)
  return userID

def reqCreateJob(userID):
  startFrame = getArg("startFrame")
  endFrame = getArg("endFrame")
  stepLength = getArg("stepLength", 1)
  
  numFramesToRender = 1 + math.floor((endFrame - startFrame) / stepLength)
  numPartitions = math.ceil(numFramesToRender / FRAMES_PER_PARTITION)
  jumpLength = numPartitions * stepLength
  
  jobID = dbAddJob(cursor          = cur,
                   startFrame      = startFrame,
                   endFrame        = endFrame,
                   stepLength      = stepLength,
                   jumpLength      = jumpLength,
                   userID          = userID,
                   title           = getArg("title", "Untitled"),
                   fps             = getArg("fps", 30),
                   version         = getArg("version", "2.92"),
                   engine          = getArg("engine", "CYCLES"),
                   outputImageType = getArg("outputImageType", "PNG"),
                   outputVideoType = getArg("outputVideoType", "MPEG"))

  for i in range(0, numPartitions):
    partitionStartFrame = startFrame + (i * stepLength)
    partitionNumFrames = math.floor(numFramesToRender / numPartitions)
    if(partitionNumFrames * numPartitions + i < numFramesToRender):
      partitionNumFrames += 1
    dbAddPartition(cur, partitionStartFrame, partitionNumFrames, jobID)
    
  os.makedirs("%s/data/%s/frames" % (SHARED_DIR_PATH, jobID))
  os.chmod("%s/data/%s" % (SHARED_DIR_PATH, jobID), DEFAULT_PERMS)
  os.chmod("%s/data/%s/frames" % (SHARED_DIR_PATH, jobID), DEFAULT_PERMS)
  
  dbUpdateJob(cur, jobID, isFragmented = True)
  return jobID

def reqVerifyAccount(userID):
  if getArg("verificationCode").lower() != dbGetUserByID(cur, userID)["verificationcode"].lower():
    raise Exception("Verification code does not match")
  dbUpdateUser(cur, userID, isVerified = True)
  
def reqDeleteJob(userID):
  jobID = getArg("jobID")
  verifyJobOwnership(jobID, userID)
  dbDeleteJob(cur, jobID)
  
def reqPauseJob(userID):
  jobID = getArg("jobID")
  verifyJobOwnership(jobID, userID)
  dbUpdateJob(cur, jobID, isPaused = True)
  
def reqUnpauseJob(userID):
  jobID = getArg("jobID")
  verifyJobOwnership(jobID, userID)
  srcPath = "%s/data/%s/src.blend" % (SHARED_DIR_PATH, jobID)
  if not os.path.exists(srcPath):
    for f in glob.glob("%s/data/%s/*.blend" % (SHARED_DIR_PATH, jobID)):
      os.rename(f, srcPath)
      break
    if not os.path.exists(srcPath):
      raise Exception("Must upload .blend file to %s before unpausing job" % srcPath)
  
  
  os.chmod(srcPath, DEFAULT_PERMS)
  dbUpdateJob(cur, jobID, isPaused = False)

def reqGetJobs(userID):     return dbGetJobByUserID(cur, userID)
def reqGetAccounts():       return dbGetAllUsers(cur)
def reqDeleteAccount():     dbDeleteUser(getArg("userID"))
def reqPrivilegeAccount():  dbUpdateUser(cur, getArg("userID"), isPrivileged = True)
def reqAdminAccount():      dbUpdateUser(cur, getArg("userID"), isAdmin = True)

###### AUTHENTICATION / UTILITY METHODS #######

def login(email, password): return dbAttemptUserLogin(cur, email, password)
def isUser(userID):         return dbGetUserByID(cur, userID) is not None
def isVerified(userID):     return dbGetUserByID(cur, userID)['isverified']
def isAdmin(userID):        return dbGetUserByID(cur, userID)['isadmin']

def verifyJobOwnership(jobID, userID):
  if not (dbGetJobByID(cur, jobID)["account_id"] == userID):
    raise Exception("User not authorized to access job")
    
def verifyCaller():
  if getpass.getuser() != "blenderssh":
    raise Exception("Invalid User: only blenderssh may make requests")

def sendVerificationEmail(recipient, verificationCode):
  port = 465
  smtp_server = "smtp.gmail.com"
  sender = "blender.render.ndsu@gmail.com"
  password = "5h62sT9Aq4"
  subject = "Verify your Blender Render account"
  text = "Your verification code is: " + verificationCode
  message = "Subject: {}\n\n{}".format(subject, text)
  context = ssl.create_default_context()
  
  with smtplib.SMTP_SSL(smtp_server, port, context=context) as server:
    server.login(sender, password)
    server.sendmail(sender, recipient, message)
    
def getArg(argName, argDefault = None):
  argVal = inputDict.get(argName)
  if argVal is None:
    if argDefault is None:
      raise Exception("Input JSON missing expected attribute: " + argName)
    return argDefault
  return argVal

################ DEBUG METHODS ################

def verifyDebugMode():
  if IS_PRODUCTION:
    raise Exception("Debug methods are not available in production code")
    
def reset():
  verifyDebugMode()
  dbResetSchema(cur)
  dbAddUser(cur, "admin@ndsu.edu", "Passw0rd", isVerified = True, isAdmin = True)
  resetPath = SHARED_DIR_PATH + "/data"
  if os.path.exists(resetPath):
    rmtree(resetPath)
  os.mkdir(resetPath, DEFAULT_PERMS)
  
def getTestInput():
  verifyDebugMode()
  with open("config/testInput.txt", "r") as file:
    rtn = file.read()
  print(rtn)
  return rtn
  

############## MAIN CODE BELOW ###############

IS_PRODUCTION = False         # Used to enable/disable flag arguments (eg. -reset and -test)
DEFAULT_PERMS = 0o750         # Permissions in /opt/lablibs/blender
FRAMES_PER_PARTITION = 1      # Maximum number of frames assigned at once to a render node
SHARED_DIR_PATH = "/opt/lablibs/blender"

outputDict = {
  "success": True,
  "errorMsg": None,
  "data": None
}
try:
  verifyCaller()
  with psycopg2.connect(**config("database.ini")) as con:
    with con.cursor(cursor_factory = psycopg2.extras.DictCursor) as cur:
      if "-reset" in sys.argv:
        reset()
      else:
        if "-test" in sys.argv:
          inputString = getTestInput()
        else:
          inputString = sys.argv[1]
        inputDict = json.loads(inputString)
        outputDict["data"] = handleRequest()
        
    con.commit()
  
except Exception as e:
  outputDict["success"] = False
  outputDict["errorMsg"] = str(e)
  raise e


  
print(json.dumps(outputDict, default=str))

