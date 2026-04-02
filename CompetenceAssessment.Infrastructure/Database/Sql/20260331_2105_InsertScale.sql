DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM assessment.scale) THEN
        INSERT INTO assessment.scale(min_score, max_score, code) VALUES
        (1, 10, 10),
        (1, 5, 5);
END IF;
END $$;