DO $$
BEGIN
    ALTER TABLE security."user"
        ADD COLUMN IF NOT EXISTS password_hash text,
        ADD COLUMN IF NOT EXISTS security_stamp text,
        ADD COLUMN IF NOT EXISTS concurrency_stamp text,
        ADD COLUMN IF NOT EXISTS normalized_email text,
        ADD COLUMN IF NOT EXISTS normalized_user_name text,
        ADD COLUMN IF NOT EXISTS user_name text;

    UPDATE security."user"
    SET user_name = email
    WHERE user_name IS NULL;

    UPDATE security."user"
    SET 
        normalized_email = UPPER(email),
        normalized_user_name = UPPER(user_name)
    WHERE normalized_email IS NULL 
       OR normalized_user_name IS NULL;

    UPDATE security."user"
    SET password_hash = 'AQAAAAIAAYagAAAAEFjQ/8HZ3AFEGrILQYzVM9YoVBVFSFSpLkcus8RPOB6czJInqPKktUYBGysdfz6uiA=='
    WHERE password_hash IS NULL;

    UPDATE security."user"
    SET 
        security_stamp = gen_random_uuid()::text,
        concurrency_stamp = gen_random_uuid()::text
    WHERE security_stamp IS NULL 
       OR concurrency_stamp IS NULL;

END $$;