DROP TABLE IF EXISTS Fragment;
DROP TABLE IF EXISTS Job;
DROP TABLE IF EXISTS Account;

DROP TYPE IF EXISTS BLENDERVERSION;
DROP TYPE IF EXISTS BLENDERENGINE;
DROP TYPE IF EXISTS BLENDEROUTPUTIMAGETYPE;
DROP TYPE IF EXISTS BLENDEROUTPUTVIDEOTYPE;

CREATE TYPE BLENDERVERSION AS ENUM ('2.76', '2.77', '2.78', '2.80', '2.81', '2.82', '2.83', '2.90', '2.91', '2.92');
CREATE TYPE BLENDERENGINE AS ENUM ('CYCLES'); -- TODO: Add BLENDER_EEVEE eventually
CREATE TYPE BLENDEROUTPUTIMAGETYPE AS ENUM ('PNG', 'JPEG', 'OPEN_EXR', 'TIFF');
CREATE TYPE BLENDEROUTPUTVIDEOTYPE AS ENUM ('MPEG');

CREATE TABLE Account (
  id SERIAL PRIMARY KEY,
  
  -- User's NDSU email address
  email VARCHAR(127)
    UNIQUE
    NOT NULL
    CONSTRAINT ndsu_domain_email CHECK (email LIKE '%_@ndsu.edu'),
  
  -- Hashed and salted password, salt is stored inside the hash
  passhash CHAR(60) NOT NULL,
  
  -- Randomly generated code sent via email to verify the user has a real NDSU email
  -- Presented in the format XXX-XXX-XXX-XXX, where X is a non-case-sensitive letter
  verificationCode CHAR(15) NOT NULL,
  
  -- Is this user's email verified?
  isVerified BOOLEAN
    NOT NULL
    DEFAULT false,
    
  -- Is this user privileged?
  isPrivileged BOOLEAN
    NOT NULL
    DEFAULT false,
  
  -- Is this user an admin?
  isAdmin BOOLEAN
    NOT NULL
    DEFAULT false,
  
  -- The date and time the user created their account
  creationTime TIMESTAMP
    NOT NULL
    DEFAULt CURRENT_TIMESTAMP(2)
    
);
  
CREATE TABLE Job (
  id SERIAL PRIMARY KEY,
  
  -- The user that uploaded this job
  account_id SERIAL NOT NULL,
  FOREIGN KEY (account_id)
    REFERENCES Account (id)
    ON DELETE CASCADE,
  
  -- The title of the job
  title VARCHAR(63) NOT NULL,
  
  -- The first frame to be rendered
  startFrame INT
    NOT NULL
    CONSTRAINT valid_start_frame CHECK (endFrame >= 0),
  
  -- The last frame to be rendered
  endFrame INT
    NOT NULL
    CONSTRAINT valid_end_frame CHECK (endFrame >= startFrame),
    
  -- Used by fragments to render noncontiguously for load balancing
  jumpLength INT
    NOT NULL
    CONSTRAINT positive_jump_length CHECK (jumpLength >= 1),
  
  -- A stepLength of size n means the user wants every nth frame of the image rendered
  stepLength INT
    NOT NULL
    DEFAULT 1
    CONSTRAINT positive_frame_step CHECK (stepLength >= 1),
    
  fps DECIMAL
    NOT NULL
    DEFAULT 30
    CONSTRAINT positive_fps CHECK (fps > 0),
  
  -- The version of blender used to render this job
  version BLENDERVERSION
    NOT NULL
    DEFAULT '2.92',
  
  -- The engine used to render this job (CYCLES or BLENDER_EEVEE)
  engine BLENDERENGINE
    NOT NULL
    DEFAULT 'CYCLES',
  
  -- The file type of the frames
  outputImageType BLENDEROUTPUTIMAGETYPE
    NOT NULL
    DEFAULT 'PNG',
    
  -- The file type of the video
  outputVideoType BLENDEROUTPUTVIDEOTYPE
    NOT NULL
    DEFAULT 'MPEG',
  
  -- Has the broker program added all of this job's fragments to the database?
  -- This is only used in the event of a system crash mid-fragmentation
  isFragmented BOOLEAN
    NOT NULL
    DEFAULT false,
  
  -- Has the user paused processing of this job, meaning render nodes will not recieve NEW fragments from this job
  isPaused BOOLEAN
    NOT NULL
    DEFAULT false,
  
  -- The date and time this job was uploaded by the user
  uploadTime TIMESTAMP
    NOT NULL
    DEFAULt CURRENT_TIMESTAMP(2),
  
  -- The date and time this job was completed. Until completed, this value will be NULL
  completedTime TIMESTAMP
);

CREATE TABLE Fragment (
  id SERIAL PRIMARY KEY,
  
  -- The job this fragment is a part of
  job_id SERIAL NOT NULL,
  FOREIGN KEY (job_id)
    REFERENCES Job (id)
    ON DELETE CASCADE,
  
  -- The first frame this fragment will be rendering
  startFrame INT NOT NULL,
  
  -- The total number of frames this fragment will be rendering
  numFrame INT NOT NULL,
  
  -- Is this fragment done being rendered?
  isComplete BOOLEAN
    NOT NULL
    DEFAULT false
);
