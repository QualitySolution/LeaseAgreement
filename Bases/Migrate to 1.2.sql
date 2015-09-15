#Переносим арендованные места из договоров в отдельную таблицу.

INSERT INTO contract_places (contract_id, place_id, start_date, end_date)
SELECT id, place_id, start_date, ifnull(cancel_date, end_date) FROM contracts WHERE place_id IS NOT NULL