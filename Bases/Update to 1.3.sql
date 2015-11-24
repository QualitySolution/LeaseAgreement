SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

USE 'LeaseAgreement';

--- Обновляем возможно неверно заведенные места.

UPDATE contract_places 
LEFT JOIN contracts ON contract_places.contract_id = contracts.id 
SET contract_places.start_date = contracts.start_date
WHERE contract_places.start_date is NULL ;

UPDATE contract_places 
LEFT JOIN contracts ON contract_places.contract_id = contracts.id 
SET contract_places.end_date = contracts.end_date
WHERE contract_places.end_date is NULL ;

--- Создаем таблицы.

CREATE TABLE IF NOT EXISTS `place_tags` (
  `id` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `tag_id` INT(11) UNSIGNED NOT NULL,
  `place_id` INT(11) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_place_tags_tags_id_idx` (`tag_id` ASC),
  INDEX `fk_place_tags_place_id_idx` (`place_id` ASC),
  CONSTRAINT `fk_place_tags_place_id`
    FOREIGN KEY (`place_id`)
    REFERENCES `places` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_place_tags_tags_id`
    FOREIGN KEY (`tag_id`)
    REFERENCES `tags` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
AUTO_INCREMENT = 1
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_general_ci;

CREATE TABLE IF NOT EXISTS `tags` (
  `id` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 1
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_general_ci;

ALTER TABLE `users` 
CHANGE COLUMN `admin` `admin` TINYINT(1) NOT NULL DEFAULT FALSE ,
CHANGE COLUMN `edit_slips` `edit_slips` TINYINT(1) NOT NULL DEFAULT FALSE ;

CREATE TABLE IF NOT EXISTS `plans` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NULL DEFAULT NULL,
  `image` MEDIUMBLOB NULL DEFAULT NULL,
  `filename` VARCHAR(45) NULL DEFAULT NULL,
  `has_labels` TINYINT(1) NULL DEFAULT 0,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 1
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_general_ci;

CREATE TABLE IF NOT EXISTS `polygons` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `floor_id` INT(10) UNSIGNED NOT NULL,
  `vertices` VARCHAR(1000) NULL DEFAULT NULL,
  `place_id` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_polygons_plan_id_idx` (`floor_id` ASC),
  INDEX `fk_polygons_place_id_idx` (`place_id` ASC),
  CONSTRAINT `fk_polygons_floor_id`
    FOREIGN KEY (`floor_id`)
    REFERENCES `floors` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_polygons_place_id`
    FOREIGN KEY (`place_id`)
    REFERENCES `places` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
AUTO_INCREMENT = 1
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_general_ci;

CREATE TABLE IF NOT EXISTS `reserve` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `date` DATE NOT NULL,
  `comment` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 1
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_general_ci;

CREATE TABLE IF NOT EXISTS `reserve_items` (
  `id` INT(10) NOT NULL AUTO_INCREMENT,
  `place_id` INT(10) UNSIGNED NOT NULL,
  `reserve_id` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_reserve_places_place_idx` (`place_id` ASC),
  INDEX `fk_reserve_places_reserve_idx` (`reserve_id` ASC),
  CONSTRAINT `fk_reserve_places_places`
    FOREIGN KEY (`place_id`)
    REFERENCES `places` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_reserve_places_reserve`
    FOREIGN KEY (`reserve_id`)
    REFERENCES `reserve` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 1
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_general_ci;

CREATE TABLE IF NOT EXISTS `floors` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NULL DEFAULT NULL,
  `plan_id` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_floors_plan_id_idx` (`plan_id` ASC),
  CONSTRAINT `fk_floors_plan_id`
    FOREIGN KEY (`plan_id`)
    REFERENCES `plans` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB
AUTO_INCREMENT = 1
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_general_ci;

-- Обновляем версию базы.
UPDATE base_parameters SET str_value = '1.3' WHERE name = 'version';


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
