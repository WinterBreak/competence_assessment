DO $$
DECLARE
    v_position_id integer;
    v_department_id integer;
    v_boss_id bigint;
    v_role_id integer;
    v_user_id bigint;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM security.role) THEN
        INSERT INTO security.role(name, description) VALUES
        ('Аттестуемый', 'Доступ к прохождению оценки'),
        ('Проверяющий', 'Доступ к проверке ответов к задания профессионального тестирования'),
        ('Администратор', 'Доступ к администрированию системы');
    END IF;

    IF NOT EXISTS (SELECT 1 FROM security.position) THEN
        INSERT INTO security.position(code, name) VALUES
        (gen_random_uuid(), 'Ведущий программист'),
        (gen_random_uuid(), 'Программист'),
        (gen_random_uuid(), 'Руководитель группы разработки N'),
        (gen_random_uuid(), 'Аналитик'),
        (gen_random_uuid(), 'Руководитель департамента'),
        (gen_random_uuid(), 'Специалист по тестированию 4 категории'),
        (gen_random_uuid(), 'Ведущий специалист по управлению персоналом');
    END IF;

    IF NOT EXISTS (SELECT 1 FROM security.department) THEN
        INSERT INTO security.department(code, name) VALUES
        (gen_random_uuid(), 'Технологический департамент'),
        (gen_random_uuid(), 'Департамент бизнес-анализа'),
        (gen_random_uuid(), 'Департамент управления персоналом');
    END IF;
         
    IF NOT EXISTS (SELECT 1 FROM security.user) THEN
        SELECT id INTO v_position_id FROM security.position WHERE name = 'Руководитель группы разработки N';
        SELECT id INTO v_department_id FROM security.department WHERE name = 'Технологический департамент';
        INSERT INTO security.user(id_position, id_department, email, first_name, second_name, last_name, boss_id) VALUES
            (v_position_id, v_department_id, 'ivanov@example.com', 'Иван',
             'Иванович', 'Иванов', NULL);
         
        SELECT id INTO v_position_id FROM security.position WHERE name = 'Программист';
        SELECT id INTO v_department_id FROM security.department WHERE name = 'Технологический департамент';
        SELECT id INTO v_boss_id FROM security.user WHERE email = 'ivanov@example.com';
        INSERT INTO security.user(id_position, id_department, email, first_name, second_name, last_name, boss_id) VALUES
            (v_position_id, v_department_id, 'prog@example.com', 'Программист',
             'Программистович', 'Программистов', v_boss_id);

        SELECT id INTO v_position_id FROM security.position WHERE name = 'Ведущий программист';
        SELECT id INTO v_department_id FROM security.department WHERE name = 'Технологический департамент';
        SELECT id INTO v_boss_id FROM security.user WHERE email = 'ivanov@example.com';
            INSERT INTO security.user(id_position, id_department, email, first_name, second_name, last_name, boss_id) VALUES
            (v_position_id, v_department_id, 'leadprog@example.com', 'Дмитрий',
            'Дмитриевич', 'Дмитриев', v_boss_id);

        SELECT id INTO v_position_id FROM security.position WHERE name = 'Руководитель департамента';
        SELECT id INTO v_department_id FROM security.department WHERE name = 'Департамент бизнес-анализа';
        INSERT INTO security.user(id_position, id_department, email, first_name, second_name, last_name, boss_id) VALUES
            (v_position_id, v_department_id, 'head@example.com', 'Татьяна',
             'Ивановна', 'Руководителева', NULL);

        SELECT id INTO v_position_id FROM security.position WHERE name = 'Аналитик';
        SELECT id INTO v_department_id FROM security.department WHERE name = 'Департамент бизнес-анализа';
        SELECT id INTO v_boss_id FROM security.user WHERE email = 'head@example.com';
            INSERT INTO security.user(id_position, id_department, email, first_name, second_name, last_name, boss_id) VALUES
            (v_position_id, v_department_id, 'analyst@example.com', 'Мария',
            'Александровна', 'Аналитикова', v_boss_id);

        SELECT id INTO v_position_id FROM security.position WHERE name = 'Специалист по тестированию 4 категории';
        SELECT id INTO v_department_id FROM security.department WHERE name = 'Технологический департамент';
        SELECT id INTO v_boss_id FROM security.user WHERE email = 'ivanov@example.com';
            INSERT INTO security.user(id_position, id_department, email, first_name, second_name, last_name, boss_id) VALUES
            (v_position_id, v_department_id, 'test@example.com', 'Светлана',
            'Павловна', 'Тестерова', v_boss_id);

        SELECT id INTO v_position_id FROM security.position WHERE name = 'Ведущий специалист по управлению персоналом';
        SELECT id INTO v_department_id FROM security.department WHERE name = 'Департамент управления персоналом';
        INSERT INTO security.user(id_position, id_department, email, first_name, second_name, last_name, boss_id) VALUES
            (v_position_id, v_department_id, 'hr@example.com', 'Елена',
             'Ивановна', 'Персоналова', NULL);
    END IF;
      
    IF NOT EXISTS (SELECT 1 FROM security.users_to_roles) THEN
        SELECT id INTO v_role_id FROM security.role WHERE name = 'Аттестуемый';
        SELECT id INTO v_user_id FROM security.user WHERE email = 'ivanov@example.com';
        INSERT INTO security.users_to_roles(user_id, role_id) VALUES
            (v_user_id, v_role_id);

        SELECT id INTO v_user_id FROM security.user WHERE email = 'prog@example.com';
        INSERT INTO security.users_to_roles(user_id, role_id) VALUES
            (v_user_id, v_role_id);
        
        SELECT id INTO v_user_id FROM security.user WHERE email = 'leadprog@example.com';
        INSERT INTO security.users_to_roles(user_id, role_id) VALUES
            (v_user_id, v_role_id);
        
        SELECT id INTO v_user_id FROM security.user WHERE email = 'head@example.com';
        INSERT INTO security.users_to_roles(user_id, role_id) VALUES
            (v_user_id, v_role_id);

        SELECT id INTO v_user_id FROM security.user WHERE email = 'analyst@example.com';
        INSERT INTO security.users_to_roles(user_id, role_id) VALUES
            (v_user_id, v_role_id);
        
        SELECT id INTO v_user_id FROM security.user WHERE email = 'test@example.com';
        INSERT INTO security.users_to_roles(user_id, role_id) VALUES
            (v_user_id, v_role_id);


        SELECT id INTO v_role_id FROM security.role WHERE name = 'Проверяющий';
        SELECT id INTO v_user_id FROM security.user WHERE email = 'ivanov@example.com';
        INSERT INTO security.users_to_roles(user_id, role_id) VALUES
            (v_user_id, v_role_id);

        SELECT id INTO v_user_id FROM security.user WHERE email = 'head@example.com';
        INSERT INTO security.users_to_roles(user_id, role_id) VALUES
            (v_user_id, v_role_id);


        SELECT id INTO v_role_id FROM security.role WHERE name = 'Администратор';
        SELECT id INTO v_user_id FROM security.user WHERE email = 'hr@example.com';
        INSERT INTO security.users_to_roles(user_id, role_id) VALUES
            (v_user_id, v_role_id);
    END IF;
END $$;