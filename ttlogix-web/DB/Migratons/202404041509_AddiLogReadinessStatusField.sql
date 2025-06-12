ALTER TABLE TT_PartMaster
ADD iLogReadinessStatus nvarchar(14) NOT NULL
DEFAULT 'Not registered' WITH VALUES
CONSTRAINT TT_PartMaster_iLogReadinessStatus_CK
CHECK (iLogReadinessStatus IN ('Registered', 'Not registered', 'Prototype'));