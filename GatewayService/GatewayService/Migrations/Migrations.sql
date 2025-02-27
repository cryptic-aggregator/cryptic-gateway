-- alter table gateway."user"
--    rename column password_md5 to password_hash;

-- truncate table gateway."user" cascade;

-- alter table gateway."user"
--     alter column password_hash type text using password_hash::text;