ALTER TABLE `contracts` 
ADD COLUMN `transfer_date` DATE NULL DEFAULT NULL AFTER `start_date`;

ALTER TABLE `stead` 
CHANGE COLUMN `contract_no` `contract_no` VARCHAR(25) NULL DEFAULT NULL ;