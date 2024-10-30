# Реализация: Используем паттерн Transaction Outbox для асинхронной и надежной доставки
Сервис Exc.Api:
1) Ручка Add/AddRandomData сервиса Exc.Api пишет в базу
2) Джоба читает базу и шлет в кролика превращая синхронный ответ (rest) в асинхронный (amqp)
3) При этом читает и смотрит кол-во необработанных сообщений в базе, группирует по (bankId, userId) и шлет в longterm/shortterm очередь банка.

Cервис Exc.Banking:
1) Тестовые данные
2) Контракты

Cервис Exc.TransactionProcessor:
1) Консюмит longterm/shortterm очереди банков с общим prefetch_count для каждого банка.
2) Процессит транзакции Банку.
3) Шлет асинхронный ответ для вытесенения транзакций из outbox таблицы.

## Условности и допущения
0) Есть Warmup для предсоздания очередей
1) Нет валидации и опций.
2) Вместо БД заглушки.
3) Не совсем корректное разбиение по проектам, что бы не плодить их.
4) Нет обработки исключений.
.. N) Нет много чего.


Трейс логов, с поочередной обработкой:
```
      Consume from: 33333333-3333-4e75-bf09-29ef0683f00e_shortterm, {"Id":"29d79442-13bf-4a93-9651-443bc90c699d","UserId":"abfff0e5-6c2b-44b3-a9ca-66d92a91b51c","BankId":"33333333-3333-4e75-bf09-29ef0683f00e"}
info: ConsumerJob[0]
      Consume from: 44444444-4444-4841-b2f1-2f30ef3e62f9_shortterm, {"Id":"6ca4fcc3-8698-4752-b700-a3fa50f15e18","UserId":"2996fb59-737f-4213-8a18-59467a27c62e","BankId":"44444444-4444-4841-b2f1-2f30ef3e62f9"}
info: ConsumerJob[0]
      Consume from: 33333333-3333-4e75-bf09-29ef0683f00e_longterm, {"Id":"931767c4-3c11-4aa9-98d7-3414a2a81884","UserId":"d367255f-696d-48d4-b815-2bce44001ce5","BankId":"33333333-3333-4e75-bf09-29ef0683f00e"}
info: ConsumerJob[0]
      Consume from: 44444444-4444-4841-b2f1-2f30ef3e62f9_longterm, {"Id":"aeb32598-6085-4af4-8ec2-9d581a7275fc","UserId":"5c934ba7-e1d1-4807-8bb0-df2de61525d0","BankId":"44444444-4444-4841-b2f1-2f30ef3e62f9"}
info: ConsumerJob[0]
      Consume from: 33333333-3333-4e75-bf09-29ef0683f00e_longterm, {"Id":"6eaa664f-b4b5-4f3b-945b-1f6eebb8bed2","UserId":"d367255f-696d-48d4-b815-2bce44001ce5","BankId":"33333333-3333-4e75-bf09-29ef0683f00e"}
info: ConsumerJob[0]
      Consume from: 44444444-4444-4841-b2f1-2f30ef3e62f9_shortterm, {"Id":"4ed1f65e-8324-45ac-a6d8-beb8d9bb033c","UserId":"ebc34c5a-0fb2-4d25-97f9-b7e18023dac0","BankId":"44444444-4444-4841-b2f1-2f30ef3e62f9"}
info: ConsumerJob[0]
      Consume from: 33333333-3333-4e75-bf09-29ef0683f00e_shortterm, {"Id":"7380428b-2bd9-4de2-8075-3ef309dce9ae","UserId":"21ab66b5-92a2-40b1-9ba3-c5a587b15b14","BankId":"33333333-3333-4e75-bf09-29ef0683f00e"}
info: ConsumerJob[0]
      Consume from: 44444444-4444-4841-b2f1-2f30ef3e62f9_longterm, {"Id":"1bdfcbd2-94d4-4fbc-9657-604b7099a5ff","UserId":"5c934ba7-e1d1-4807-8bb0-df2de61525d0","BankId":"44444444-4444-4841-b2f1-2f30ef3e62f9"}
```

