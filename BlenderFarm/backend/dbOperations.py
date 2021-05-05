import psycopg2
import psycopg2.extras
import os
import bcrypt
import random
import string
import re
from datetime import datetime, timezone

############### CREATE METHODS ###############

# Add a user to database, return its ID (or None, if the insertion failed)
def dbAddUser(
      cursor,
      email,
      password,
      verificationCode = None,
      isVerified = False,
      isPrivileged = False,
      isAdmin = False,
      creationTime = datetime.now(timezone.utc)):
    
  verifyEmail(email)
  verifyPassword(password)

  if verificationCode is None:
    verificationCode = generateVerification()
    
  verifyVerificationCode(verificationCode)

  passhash = getHashedPassword(password)
  
  sql = """
    INSERT INTO Account
      (
      email,
      passhash,
      verificationCode,
      isVerified,
      isPrivileged,
      isAdmin,
      creationTime
      )
              
    VALUES (%s, %s, %s, %s, %s, %s, %s)
    RETURNING id;
  """
  
  cursor.execute(sql,
      (
      email,
      passhash,
      verificationCode,
      isVerified,
      isPrivileged,
      isAdmin,
      creationTime
      ))
  
  return cursor.fetchone()[0]
  
# Add a job to the database, return its ID
def dbAddJob(
      cursor,
      startFrame,
      endFrame,
      jumpLength,
      userID,
      title = "Untitled",
      stepLength = 1,
      fps = 30,
      version = "2.92",
      engine = "CYCLES",
      outputImageType = "PNG",
      outputVideoType = "MPEG",
      isFragmented = False,
      isPaused = True,
      uploadTime = datetime.now(timezone.utc),
      completedTime = None):
  
  sql = """
    INSERT INTO Job
      (
      title,
      startFrame,
      endFrame,
      jumpLength,
      account_id,
      stepLength,
      fps,
      version,
      engine,
      outputImageType,
      outputVideoType,
      isFragmented,
      isPaused,
      uploadTime,
      completedTime
      )
             
    VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
    RETURNING id;
  """
  
  cursor.execute(sql,
      (
      title,
      startFrame,
      endFrame,
      jumpLength,
      userID,
      stepLength,
      fps,
      version,
      engine,
      outputImageType,
      outputVideoType,
      isFragmented,
      isPaused,
      uploadTime,
      completedTime
      ))
  
  return cursor.fetchone()[0]

# Add a partition to the database, return its ID
def dbAddPartition(
      cursor,
      startFrame,
      numFrame,
      jobID,
      isComplete = False):
  
  sql = """
    INSERT INTO Fragment
      (
      startFrame,
      numFrame,
      job_id,
      isComplete
      )
                   
    VALUES (%s, %s, %s, %s)
    RETURNING id;
  """
  
  cursor.execute(sql,
      (
      startFrame,
      numFrame,
      jobID,
      isComplete
      ))
  
  return cursor.fetchone()[0]

################ READ METHODS ################

def dbGetUserByID(cursor, userID):
  cursor.execute("SELECT * FROM Account WHERE id = %s;", (userID,))
  return cursor.fetchone()

def dbGetJobByID(cursor, jobID):
  cursor.execute("SELECT * FROM Job WHERE id = %s;", (jobID,))
  return cursor.fetchone()

def dbGetPartitionByID(cursor, partitionID):
  cursor.execute("SELECT * FROM Fragment WHERE id = %s;", (partitionID,))
  return cursor.fetchone()

def dbAttemptUserLogin(cursor, email, password):
  try:
    verifyEmail(email)
    verifyPassword(password)
  except:
    return None
    
  cursor.execute("SELECT id, passhash FROM Account WHERE email = %s;", (email,))
  result = cursor.fetchone()
  
  if result is None:
    return None

  if not checkPassword(password, result["passhash"]):
    return None

  return result['id']

def dbGetJobByUserID(cursor, userID):
  cursor.execute("SELECT * FROM Job WHERE account_id = %s;", (userID,))
  return cursor.fetchall()

def dbGetPartitionByJobID(cursor, jobID):
  cursor.execute("SELECT * FROM Partition WHERE job_id = %s;", (jobID,))
  return cursor.fetchall()

def dbGetAllUsers(cursor):
  cursor.execute("SELECT * FROM Account;")
  return cursor.fetchall()
  
def dbGetPartitionQueue(cursor, size):
  sql = """
    SELECT Fragment.id
    
    FROM ((Fragment
      INNER JOIN Job ON Fragment.job_id = Job.id)
      INNER JOIN Account ON Job.account_id = Account.id)
    
    WHERE Job.isPaused = false
      AND Job.isFragmented = true
      AND fragment.isComplete = false
      
    ORDER BY Account.isPrivileged DESC,
      Fragment.id ASC
      
    LIMIT %s;
  """
  
  cursor.execute(sql, (size,))
  return cursor.fetchall()
  
def dbGetPartitionRenderInfoByID(cursor, partitionID):
  sql = """
    SELECT
      Fragment.startFrame,
      Fragment.numFrame,
      Job.jumpLength,
      Fragment.job_id,
      Job.version,
      Job.engine,
      Job.outputImageType
           
    FROM (Fragment INNER JOIN Job ON Fragment.job_id = Job.id)
    
    WHERE Fragment.id = %s;    
  """
    
  cursor.execute(sql, (partitionID,))
  return cursor.fetchone()
  
def dbIsJobRendered(cursor, jobID):
  sql = """
    SELECT * FROM Fragment
    WHERE job_id = %s
    AND isComplete = false
    LIMIT 1;
  """
  cursor.execute(sql, (jobID,))
  return cursor.fetchone() is None

############### UPDATE METHODS ###############

def dbUpdateUser(
      cursor,
      userID,
      email = None,
      password = None,
      verificationCode = None,
      isVerified = None,
      isPrivileged = None,
      isAdmin = None,
      creationTime = None):
  
  if email is not None:
    verifyEmail(email)
    performUpdate(cursor, "Account", userID, "email", email)
    
  if password is not None:
    verifyPassword(password)
    performUpdate(cursor, "Account", userID, "passhash", getHashedPassword(password))
    
  if verificationCode is not None:
    verifyVerificationCode(verificationCode)
    performUpdate(cursor, "Account", userID, "verificationCode", verificationCode)
    
  performUpdate(cursor, "Account", userID, "isVerified", isVerified)
  performUpdate(cursor, "Account", userID, "isPrivileged", isPrivileged)
  performUpdate(cursor, "Account", userID, "isAdmin", isAdmin)
  performUpdate(cursor, "Account", userID, "creationTime", creationTime)
  
def dbUpdateJob(
      cursor,
      jobID,
      title = None,
      userID = None,
      isFragmented = None,
      isPaused = None,
      uploadTime = None,
      completedTime = None
      # startFrame = None,      (Backend requires this information remains constant)
      # endFrame = None,        (Backend requires this information remains constant)
      # jumpLength = None,      (Backend requires this information remains constant)
      # stepLength = None,      (Backend requires this information remains constant)
      # fps = None,             (Backend requires this information remains constant)
      # version = None,         (Backend requires this information remains constant)
      # engine = None,          (Backend requires this information remains constant)
      # outputImageType = None, (Backend requires this information remains constant)
      # outputVideoType = None, (Backend requires this information remains constant)
      ):
  
  performUpdate(cursor, "Job", jobID, "title", title)
  performUpdate(cursor, "Job", jobID, "account_id", userID)
  performUpdate(cursor, "Job", jobID, "isFragmented", isFragmented)
  performUpdate(cursor, "Job", jobID, "isPaused", isPaused)
  performUpdate(cursor, "Job", jobID, "uploadTime", uploadTime)  
  performUpdate(cursor, "Job", jobID, "completedTime", completedTime)
    
def dbUpdatePartition(
      cursor,
      partitionID,
      isComplete = None
      # startFrame = None,      (Backend requires this information remains constant)
      # numFrame = None,        (Backend requires this information remains constant)
      # jobID = None,           (Backend requires this information remains constant)
      ):
  performUpdate(cursor, "Fragment", partitionID, "isComplete", isComplete)
  
  
############### DELETE METHODS ###############
    
def dbDeleteUser(cursor, userID):
  cursor.execute("DELETE FROM User WHERE id = %s;", (userID,))
  
def dbDeleteJob(cursor, jobID):
  cursor.execute("DELETE FROM Job WHERE id = %s;", (jobID,))
  
def dbDeletePartition(cursor, partitionID):
  cursor.execute("DELETE FROM Fragment WHERE id = %s;", (partitionID,))

# Return the database to empty tables
# TESTING PURPOSES ONLY, DO NOT RUN THIS IN PRODUCTION
def dbResetSchema(cursor):
  script_dir = os.path.dirname(__file__)
  rel_path = "config/schema.sql"
  abs_file_path = os.path.join(script_dir, rel_path)
  with open(abs_file_path, "r") as f:
    cursor.execute(f.read())

############### UTILITY METHODS ##############

def performUpdate(cursor, tableName, updatedID, attributeName, updatedAttribute):
  if updatedAttribute is not None:
    cursor.execute("UPDATE "+tableName+" SET "+attributeName+" = %s WHERE id = %s;", (updatedAttribute, updatedID))

def getHashedPassword(plaintextPassword):
  return bcrypt.hashpw(plaintextPassword.encode("ascii"), bcrypt.gensalt(4)).decode("ascii")

def checkPassword(plaintextPassword, hashedPassword):
  return bcrypt.checkpw(plaintextPassword.encode("ascii"), hashedPassword.encode("ascii"))

def generateVerification():
  str = "".join(random.choice(string.ascii_uppercase) for i in range(3))
  for x in range(3):
    str += "-"
    str += "".join(random.choice(string.ascii_uppercase) for i in range(3))
  return str

def verifyEmail(email):
  isEmail = (re.match("^.+@(\[?)[a-zA-Z0-9-.]+.([a-zA-Z]{2,3}|[0-9]{1,3})(]?)$", email) is not None)
  isNDSU = email.endswith("@ndsu.edu")
  if not (isEmail and isNDSU):
    raise ValueError("Invalid email (Must be an @ndsu.edu email address)")

def verifyPassword(password):
  containsUpper = bool(re.match(r'\w*[A-Z]\w*', password))
  containsLower = bool(re.match(r'\w*[a-z]\w*', password))
  containsNumber = bool(re.match(r'\w*[0-9]\w*', password))
  longEnough = bool(len(password) >= 8)
  if not (containsUpper and containsLower and containsNumber and longEnough):
    raise ValueError("Invalid password (Must be 8+ characters and contain uppercase, lowercase, and number)")
    
def verifyVerificationCode(vCode):
  if not bool(re.match(r'^[A-Za-z]{3}(-[A-Za-z]{3}){3}$', vCode)):
    raise Exception("Invalid verification code (Must be of the format XXX-XXX-XXX-XXX-XXX, where X is a letter)")
