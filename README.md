# 🐝 HoneyShop – Интернет-магазин медовой продукции

Добро пожаловать в HoneyShop — полнофункциональное приложение интернет-магазина, разработанное на Angular и ASP.NET Core. В нём реализована современная архитектура, удобная панель администратора, оформление заказов, отслеживание статусов и многое другое.

---

## 🚀 Технологии

**Frontend:**
- Angular Standalone
- Tailwind CSS
- RxJS
- Chart.js
- Swiper.js
- ngx-toastr

**Backend:**
- ASP.NET Core Web API
- Entity Framework Core (Code First)
- JWT + Refresh Token (в HttpOnly cookie)
- MSSQL Server

---

## 📁 Структура проекта

HoneyShop/
├── backend/ # ASP.NET Core Web API
├── frontend/ # Angular-приложение
└── README.md

---

## ⚙️ Установка и запуск

### 📦 Backend (.NET)

```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
```
⚠️ Убедитесь, что строка подключения к базе данных указана в dbsettings.json.

### 🌐 Frontend (Angular)

```bash
cd frontend
npm install
ng serve
```
Откройте http://localhost:4200 в браузере.

### 🧪 SSL (HTTPS) Использование
Backend и frontend настроены на работу с локальными SSL-сертификатами.

Чтобы избежать предупреждений в браузере и ошибок при работе с HTTPS, выполните команду:

```
dotnet dev-certs https --trust
```

Она создаст и доверит локальный сертификат для ASP.NET Core.

Если вы используете Angular dev-server с сертификатами из папки frontend/ssl, убедитесь, что браузер доверяет этим сертификатам.

**👤 Возможности пользователей**
- Регистрация и вход
- Редактирование профиля
- Добавление товаров в корзину (localStorage)
- Оформление заказа с выбором способа оплаты и доставки
- Просмотр истории заказов
- Адаптивный дизайн

**🔐 Административная панель**
- Управление товарами и категориями
- Изменение статусов заказов
- Просмотр истории изменений заказов
- Графики и аналитика

**🛡️ Безопасность**
- Access Token хранится в памяти (RxJS)
- Refresh Token в HttpOnly cookie
- Защита от XSS
- Роли пользователей (пользователь / администратор)

📸 Скриншоты

📚 Лицензия
Этот проект создан в рамках дипломной работы и предназначен для учебных целей.

📞 Контакты
Разработчик: Dmitry Shepetov
Email: dmitryshepetov@gmail.com
