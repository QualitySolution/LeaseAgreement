use leaseagreement;
-- Исправление некорректных записей в базе 
-- Если дата конца аренды == NULL, то заменяем её на дату окончания/расторжения договора.
-- Если дата начала аренды == NULL, то удаляем
UPDATE contract_places as places
join contracts 
on contracts.id=places.contract_id 
set places.end_date = IFNULL(contracts.cancel_date,contracts.end_date)  
where ISNULL(places.end_date);

DELETE FROM contract_places
WHERE ISNULL(contract_paces.start_date)
