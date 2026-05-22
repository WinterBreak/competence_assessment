DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM assessment.scale) THEN
        INSERT INTO assessment.scale( max_score, code) VALUES
        (10, 10),
        (5, 5);
END IF;
END $$;