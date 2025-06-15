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
├── backend/   # ASP.NET Core Web API  
├── frontend/  # Angular-приложение  
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

---

### 🧪 SSL (HTTPS) Использование
Backend и frontend настроены на работу с локальными SSL-сертификатами.

Чтобы избежать предупреждений в браузере и ошибок при работе с HTTPS, выполните команду:

```
dotnet dev-certs https --trust
```

Она создаст и доверит локальный сертификат для ASP.NET Core.

Если вы используете Angular dev-server с сертификатами из папки frontend/ssl, убедитесь, что браузер доверяет этим сертификатам.

---

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

---

## 📸 Скриншоты

### 🏠 Главная страница
<img src="https://github.com/user-attachments/assets/713365be-1aa1-4507-98df-a8f6c8015767" width="800"/>

### 📄 Футер
<img src="https://github.com/user-attachments/assets/5d3db8c7-a51e-491d-b7f5-04550ee1b4bc" width="800"/>

### 🛍️ Модальное окно товара (карточка)
<img src="https://github.com/user-attachments/assets/234ac457-0812-47db-9cec-1c1fe1eb5a87" width="800"/>

### 🔐 Страница входа
<img src="https://github.com/user-attachments/assets/c543b3fc-d268-48d4-98e2-7717073196a7" width="800"/>

### 🧾 Страница регистрации
<img src="https://github.com/user-attachments/assets/57f1d760-85d0-45f1-b4c1-b2f7ce743d07" width="800"/>

### 👤 Личный кабинет
<img src="https://github.com/user-attachments/assets/f44c4afb-b5d9-4b4a-a90c-6181365cc1ed" width="800"/>

### ⚙️ Админ-панель
<img src="https://github.com/user-attachments/assets/3db297ef-728d-4864-9974-1302afd3c7d1" width="800"/>

### 🛒 Корзина
<img src="https://github.com/user-attachments/assets/5e207067-c5f8-48c1-a7a6-c40720b3646c" width="800"/>

### 📦 Оформление заказа
<img src="https://github.com/user-attachments/assets/45094ac6-c666-4e61-bf1c-3e0bd712e90b" width="800"/>

### ➕ Добавление товаров (админ-панель)
<img src="https://github.com/user-attachments/assets/659f80f3-5e52-4cee-8923-5bc91d028e07" width="800"/>

### 💳 Оплата картой
<img src="https://github.com/user-attachments/assets/5abc35e4-2dc2-46f1-881e-9bab42946e7e" width="800"/>

---

### 📚 Лицензия  
Этот проект создан в рамках дипломной работы и предназначен для учебных целей.

---

### 📞 Контакты  
Разработчик: Dmitry Shepetov  
Email: dmitryshepetov@gmail.com  

---
