DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM assessment.template_type) THEN
        INSERT INTO assessment.template_type(code, name) VALUES
        (0, 'Не выбрано'),
        (1, 'Тестирование'),
        (2, 'Анкета');
END IF;
END $$;