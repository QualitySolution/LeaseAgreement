USE `leaseagreement`;

-- Добавляем новые поля

ALTER TABLE `leaseagreement`.`users` 
CHANGE COLUMN `admin` `admin` TINYINT(1) NOT NULL DEFAULT FALSE ,
CHANGE COLUMN `edit_slips` `edit_slips` TINYINT(1) NOT NULL DEFAULT FALSE ,
ADD COLUMN `deactivated` TINYINT(1) NOT NULL DEFAULT 0 AFTER `login`,
ADD COLUMN `email` VARCHAR(60) NULL DEFAULT NULL AFTER `deactivated`;

CREATE TABLE IF NOT EXISTS `leaseagreement`.`contract_places` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `contract_id` INT(10) UNSIGNED NOT NULL,
  `place_id` INT(10) UNSIGNED NOT NULL,
  `start_date` DATE NULL DEFAULT NULL,
  `end_date` DATE NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_contract_places_1_idx` (`contract_id` ASC),
  INDEX `fk_contract_places_2_idx` (`place_id` ASC),
  CONSTRAINT `fk_contract_places_1`
    FOREIGN KEY (`contract_id`)
    REFERENCES `leaseagreement`.`contracts` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_contract_places_2`
    FOREIGN KEY (`place_id`)
    REFERENCES `leaseagreement`.`places` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_general_ci;


-- -----------------------------------------------------
-- Placeholder table for view `leaseagreement`.`current_leased_places`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `leaseagreement`.`current_leased_places` (`place_id` INT, `contract_id` INT);


USE `leaseagreement`;

-- -----------------------------------------------------
-- View `leaseagreement`.`current_leased_places`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `leaseagreement`.`current_leased_places`;
USE `leaseagreement`;
CREATE  OR REPLACE VIEW `current_leased_places` AS 
SELECT contract_places.place_id, contract_places.contract_id FROM contract_places
LEFT JOIN contracts ON contracts.id = contract_places.contract_id
WHERE contracts.draft = '0' AND contract_places.start_date IS NOT NULL
AND contract_places.start_date <= curdate() 
AND (contract_places.end_date IS NULL OR contract_places.end_date >= curdate())
;

-- Переносим арендованные места из договоров в отдельную таблицу.

INSERT INTO contract_places (contract_id, place_id, start_date, end_date)
SELECT id, place_id, start_date, ifnull(cancel_date, end_date) FROM contracts WHERE place_id IS NOT NULL;

-- Удаляем старую связку.
ALTER TABLE `leaseagreement`.`contracts` 
DROP FOREIGN KEY `fk_place_id`;

ALTER TABLE `leaseagreement`.`contracts` 
DROP COLUMN `place_id`,
DROP INDEX `fk_place_id_idx` ;

-- Обновляем версию базы.
UPDATE base_parameters SET str_value = '1.2' WHERE name = 'version';
