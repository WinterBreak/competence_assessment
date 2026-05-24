DO $$
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'assessment' 
        AND table_name = 'assessment' 
        AND column_name = 'is_finished'
    ) THEN
        IF NOT EXISTS (
            SELECT 1 
            FROM information_schema.columns 
            WHERE table_schema = 'assessment' 
            AND table_name = 'assessment' 
            AND column_name = 'state'
        ) THEN

ALTER TABLE assessment.assessment
    ADD COLUMN state smallint;
END IF;

UPDATE assessment.assessment
SET state = CASE
                WHEN is_finished = true THEN 3
                ELSE 1
    END
WHERE state IS NULL;

ALTER TABLE assessment.assessment
    ALTER COLUMN state SET NOT NULL;

ALTER TABLE assessment.assessment
    ALTER COLUMN state SET DEFAULT 1;

ALTER TABLE assessment.assessment
DROP COLUMN is_finished;
END IF;
END $$;