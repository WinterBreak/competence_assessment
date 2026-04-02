DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM assessment.assessment_type) THEN
        INSERT INTO assessment.assessment_type(code, name) VALUES
        (0, 'Не выбрано'),
        (1, 'Тестирование'),
        (2, 'Анкетирование'),
        (3, 'Оценка 360 градусов');
END IF;
END $$;