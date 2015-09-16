ALTER TABLE `LeaseAgreement`.`contracts` 
DROP FOREIGN KEY `fk_place_id`;

ALTER TABLE `LeaseAgreement`.`contracts` 
ADD COLUMN `place_id` INT(10) UNSIGNED NULL DEFAULT NULL AFTER `org_id`,
DROP INDEX `fk_place_id_idx` ,
ADD INDEX `fk_place_id_idx` (`place_id` ASC);

ALTER TABLE `LeaseAgreement`.`contracts` 
ADD CONSTRAINT `fk_place_id`
  FOREIGN KEY (`place_id`)
  REFERENCES `LeaseAgreement`.`places` (`id`)
  ON DELETE CASCADE
  ON UPDATE CASCADE;

UPDATE contracts SET place_id = 
(SELECT places.id FROM places WHERE places.type_id = contracts.place_type_id AND places.place_no = contracts.place_no);

ALTER TABLE `LeaseAgreement`.`contracts` 
DROP COLUMN `place_no`,
DROP COLUMN `place_type_id`;

CREATE TABLE IF NOT EXISTS `leaseagreement`.`history_changeset` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `datetime` DATETIME NOT NULL,
  `user_id` INT(10) UNSIGNED NULL DEFAULT NULL,
  `operation` ENUM('Create','Change','Delete') NOT NULL,
  `object_name` VARCHAR(45) NOT NULL,
  `object_id` INT(10) UNSIGNED NOT NULL,
  `object_title` VARCHAR(100) NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_changeset_user_id_idx` (`user_id` ASC),
  INDEX `ix_changeset_operation` (`operation` ASC),
  INDEX `ix_changeset_object_id` (`object_name` ASC, `object_id` ASC),
  CONSTRAINT `fk_changeset_user_id`
    FOREIGN KEY (`user_id`)
    REFERENCES `leaseagreement`.`users` (`id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_general_ci;

CREATE TABLE IF NOT EXISTS `leaseagreement`.`history_changes` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `changeset_id` INT(10) UNSIGNED NOT NULL,
  `type` ENUM('Added','Changed','Removed', 'Unchanged') NOT NULL DEFAULT 'Unchanged',
  `path` VARCHAR(80) NOT NULL,
  `old_value` TEXT NULL DEFAULT NULL,
  `old_id` INT(10) UNSIGNED NULL DEFAULT NULL,
  `new_value` TEXT NULL DEFAULT NULL,
  `new_id` INT(10) UNSIGNED NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_changesets_id_idx` (`changeset_id` ASC),
  UNIQUE INDEX `unique` (`changeset_id` ASC, `path` ASC),
  CONSTRAINT `fk_changesets_id`
    FOREIGN KEY (`changeset_id`)
    REFERENCES `leaseagreement`.`history_changeset` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_general_ci;

UPDATE base_parameters SET str_value = '1.1' WHERE name = 'version';
