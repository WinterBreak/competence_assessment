CREATE SCHEMA security;

ALTER SCHEMA security OWNER TO postgres;

CREATE SCHEMA assessment;

ALTER SCHEMA assessment OWNER TO postgres;


SET search_path TO pg_catalog,public,security,assessment;

CREATE TABLE security."user" (
                                 id bigint NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                 id_position integer,
                                 id_department integer,
                                 email varchar(255),
                                 first_name varchar(255),
                                 second_name varchar(255),
                                 last_name varchar(255),
                                 boss_id bigint,
                                 CONSTRAINT user_id_pk PRIMARY KEY (id)
);

ALTER TABLE security."user" OWNER TO postgres;

CREATE TABLE security.role (
                               id integer NOT NULL GENERATED ALWAYS AS IDENTITY ,
                               name varchar(100) NOT NULL,
                               description varchar(1000),
                               CONSTRAINT role_id_pk PRIMARY KEY (id)
);

ALTER TABLE security.role OWNER TO postgres;

CREATE TABLE security."position" (
                                     id integer NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                     code uuid NOT NULL,
                                     name varchar(255) NOT NULL,
                                     CONSTRAINT position_id_pk PRIMARY KEY (id)
);

ALTER TABLE security."position" OWNER TO postgres;

ALTER TABLE security."user" ADD CONSTRAINT position_fk FOREIGN KEY (id_position)
    REFERENCES security."position" (id) MATCH FULL
    ON DELETE SET NULL ON UPDATE CASCADE;

CREATE TABLE security.department (
                                     id integer NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                     code uuid NOT NULL,
                                     name varchar(255) NOT NULL,
                                     CONSTRAINT department_id_pk PRIMARY KEY (id)
);

ALTER TABLE security.department OWNER TO postgres;

ALTER TABLE security."user" ADD CONSTRAINT department_fk FOREIGN KEY (id_department)
    REFERENCES security.department (id) MATCH FULL
    ON DELETE SET NULL ON UPDATE CASCADE;

CREATE TABLE assessment.competence (
                                      id integer NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                      name varchar(255) NOT NULL,
                                      description varchar(500),
                                      CONSTRAINT competence_id_pk PRIMARY KEY (id)
);

ALTER TABLE assessment.competence OWNER TO postgres;

CREATE TABLE assessment.task (
                                id bigint NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                id_task_type smallint,
                                text text NOT NULL,
                                answer text,
                                CONSTRAINT task_id_pk PRIMARY KEY (id)
);

ALTER TABLE assessment.task OWNER TO postgres;

CREATE TABLE assessment.task_type (
                                     id smallint NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                     code varchar(50) NOT NULL,
                                     name varchar(255) NOT NULL,
                                     CONSTRAINT task_type_pk PRIMARY KEY (id)
);

ALTER TABLE assessment.task_type OWNER TO postgres;

CREATE TABLE assessment.scale (
                                 id smallint NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                 min_score smallint NOT NULL,
                                 max_score smallint NOT NULL,
                                 code smallint NOT NULL,
                                 CONSTRAINT scale_id_pk PRIMARY KEY (id)
);

ALTER TABLE assessment.scale OWNER TO postgres;

CREATE TABLE assessment.assessment_type (
                                           id smallint NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                           code varchar(50) NOT NULL,
                                           name varchar(100) NOT NULL,
                                           CONSTRAINT assessment_type_id PRIMARY KEY (id)
);

ALTER TABLE assessment.assessment_type OWNER TO postgres;

CREATE TABLE assessment.competence_model (
                                            id integer NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                            name varchar(255) NOT NULL,
                                            description varchar(500),
                                            creation_date timestamp NOT NULL,
                                            CONSTRAINT competence_model_id_pk PRIMARY KEY (id)
);

ALTER TABLE assessment.competence_model OWNER TO postgres;

CREATE TABLE assessment.competence_model_detail (
                                                   competence_model_id integer NOT NULL,
                                                   competence_id integer NOT NULL,
                                                   weight decimal(3,2) NOT NULL,
                                                   CONSTRAINT competence_model_details_pk PRIMARY KEY (competence_model_id,competence_id)
);

ALTER TABLE assessment.competence_model_detail OWNER TO postgres;

CREATE TABLE assessment.template (
                                    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                    id_competence_model integer,
                                    id_template_type smallint,
                                    id_scale smallint,
                                    name varchar(255) NOT NULL,
                                    creation_date timestamp NOT NULL,
                                    CONSTRAINT template_id_pk PRIMARY KEY (id)
);

ALTER TABLE assessment.template OWNER TO postgres;

CREATE TABLE assessment.template_type (
                                         id smallint NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                         code varchar(50) NOT NULL,
                                         name varchar(255) NOT NULL,
                                         CONSTRAINT template_type_id_pk PRIMARY KEY (id)
);

ALTER TABLE assessment.template_type OWNER TO postgres;

CREATE TABLE assessment.template_detail (
                                           id bigint NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                           id_template integer,
                                           id_task bigint,
                                           weight decimal(3,2) NOT NULL,
                                           id_competence integer NOT NULL,
                                           CONSTRAINT template_details_pk PRIMARY KEY (id)
);

ALTER TABLE assessment.template_detail OWNER TO postgres;

CREATE TABLE assessment.assessment (
                                      id bigint NOT NULL GENERATED ALWAYS AS IDENTITY ,
                                      id_assessment_type smallint,
                                      id_template integer,
                                      id_user bigint,
                                      start_date timestamp,
                                      end_date timestamp,
                                      is_finished bool NOT NULL,
                                      comment text,
                                      CONSTRAINT assessment_pk PRIMARY KEY (id)
);

ALTER TABLE assessment.assessment OWNER TO postgres;

CREATE TABLE assessment.assessment_result (
                                             assessment_result_id bigint NOT NULL,
                                             task_id bigint,
                                             comment text,
                                             score smallint NOT NULL,
                                             answer text,
                                             id_assessment bigint,
                                             CONSTRAINT result_pk PRIMARY KEY (assessment_result_id)
);

ALTER TABLE assessment.assessment_result OWNER TO postgres;

ALTER TABLE assessment.competence_model_detail ADD CONSTRAINT competence_model_fk FOREIGN KEY (id_competence_model)
    REFERENCES assessment.competence_model (id) MATCH FULL
    ON DELETE SET NULL ON UPDATE CASCADE;

ALTER TABLE assessment.template ADD CONSTRAINT competence_model_fk FOREIGN KEY (id_competence_model)
    REFERENCES assessment.competence_model (id) MATCH FULL
    ON DELETE SET NULL ON UPDATE CASCADE;

ALTER TABLE assessment.template_detail ADD CONSTRAINT template_fk FOREIGN KEY (id_template)
    REFERENCES assessment.template (id) MATCH FULL
    ON DELETE SET NULL ON UPDATE CASCADE;

ALTER TABLE assessment.template_detail ADD CONSTRAINT task_fk FOREIGN KEY (id_task)
    REFERENCES assessment.task (id) MATCH FULL
    ON DELETE SET NULL ON UPDATE CASCADE;

ALTER TABLE assessment.assessment_result ADD CONSTRAINT assessment_fk FOREIGN KEY (id_assessment)
    REFERENCES assessment.assessment (id) MATCH FULL
    ON DELETE SET NULL ON UPDATE CASCADE;

ALTER TABLE assessment.assessment ADD CONSTRAINT user_fk FOREIGN KEY (id_user)
    REFERENCES security."user" (id) MATCH FULL
    ON DELETE SET NULL ON UPDATE CASCADE;

CREATE TABLE security.users_to_roles (
                                         user_id bigint NOT NULL ,
                                         role_id integer NOT NULL ,
                                         CONSTRAINT user_to_roles_pk PRIMARY KEY (user_id,role_id)
);

ALTER TABLE security.users_to_roles OWNER TO postgres;

CREATE TABLE assessment.assessment_inspector (
                                                user_id bigint NOT NULL,
                                                assessment_id bigint NOT NULL,
                                                CONSTRAINT inspector_pk PRIMARY KEY (user_id,assessment_id)
);

ALTER TABLE assessment.assessment_inspector OWNER TO postgres;

ALTER TABLE security."user" ADD CONSTRAINT user_id_boss_id_fk FOREIGN KEY (boss_id)
    REFERENCES security."user" (id) MATCH SIMPLE
    ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE assessment.template_detail ADD CONSTRAINT competence_fk FOREIGN KEY (id_competence)
    REFERENCES assessment.competence (id) MATCH SIMPLE
    ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE assessment.assessment_result ADD CONSTRAINT task_id_fk FOREIGN KEY (task_id)
    REFERENCES assessment.task (id) MATCH SIMPLE
    ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE security.users_to_roles ADD CONSTRAINT users_fk FOREIGN KEY (user_id)
    REFERENCES security."user" (id) MATCH SIMPLE
    ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE security.users_to_roles ADD CONSTRAINT roles_fk FOREIGN KEY (role_id)
    REFERENCES security.role (id) MATCH SIMPLE
    ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE assessment.assessment_inspector ADD CONSTRAINT user_fk FOREIGN KEY (user_id)
    REFERENCES security."user" (id) MATCH SIMPLE
    ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE assessment.assessment_inspector ADD CONSTRAINT assessment_fk FOREIGN KEY (assessment_id)
    REFERENCES assessment.assessment (id) MATCH SIMPLE
    ON DELETE NO ACTION ON UPDATE NO ACTION;


