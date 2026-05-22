DO $$
BEGIN

ALTER TABLE security."user"
    ADD COLUMN IF NOT EXISTS access_failed_count integer NOT NULL DEFAULT 0,
    ADD COLUMN IF NOT EXISTS lockout_enabled boolean NOT NULL DEFAULT false,
    ADD COLUMN IF NOT EXISTS lockout_end timestamp with time zone NULL,
    ADD COLUMN IF NOT EXISTS two_factor_enabled boolean NOT NULL DEFAULT false,
    ADD COLUMN IF NOT EXISTS phone_number text NULL,
    ADD COLUMN IF NOT EXISTS phone_number_confirmed boolean NOT NULL DEFAULT false;

END $$;