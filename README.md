# EncryptedMessenger
Клиент-серверное приложение с использованием протокола строгой аутентификации.
Регистрация пользователя проходит на стороне сервера. На стороне клиента происходит только аутентификция.
Для взаимодействия клиента с сервером используется TCP-сокет, при этом пароль не перемещается по открытой сети
## Регистрация:
- Регистрация новых пользователей возможна только на сервере
## Аутентификация:
- При попытке аутентификации на сервер отправляется логин. 
- Сервер отправляет клиенту хэш временной метки.
- Клиент хэширует хэш пароля и хэш временной метки, полученной от сервера, и отправляет на сервер.
- Сервер сверяет хэш хэша пароля, хранящегося на сервере, и хэша временной метки с хэшем, который пришел от клиента.
- Если хэши совпадают, аутентификация пройдена
## Фичи
- Электронно-цифровая подпись
- Алгоритм Диффи-Хеллмана
- RC4 потоковый шифр
## Сетевое взаимодействие:
- TCP сокеты
## Сервер:
- SQLite бд с аккаунтами
- Регистрация новых аккаунтов
- Лог запросов и ответов
## Клиент:
- Окно авторизации
