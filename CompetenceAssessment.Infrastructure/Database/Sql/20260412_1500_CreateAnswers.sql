CREATE TABLE IF NOT EXISTS assessment.answer (
                                                 id bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
                                                 task_id bigint NOT NULL,
                                                 answer text NOT NULL,
                                                 is_correct boolean NOT NULL,
                                                 CONSTRAINT answer_pk PRIMARY KEY (id)
    );

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = 'task_answer_task_fk'
        AND conrelid = 'assessment.answer'::regclass
    ) THEN
ALTER TABLE assessment.answer
    ADD CONSTRAINT task_answer_task_fk
        FOREIGN KEY (task_id)
            REFERENCES assessment.task (id);
END IF;
END $$;

ALTER TABLE assessment.answer OWNER TO postgres;
